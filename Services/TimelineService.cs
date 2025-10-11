using System.Text.Json;
using Microsoft.JSInterop;
using Timeline.Models;
using System.Text;

namespace Timeline.Services;

public class TimelineService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private List<Person> _people = new();
    private const string StorageKey = "timeline_people";
    private const string VersionKey = "timeline_version";
    private const string CurrentVersion = "2.0"; // Increment this to force data reload

    public event Action? OnChange;

    public TimelineService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        // Check if we need to force reload due to version change
        var storedVersion = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", VersionKey);
        
        if (storedVersion != CurrentVersion)
        {
            // Clear old data and force reload
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", VersionKey, CurrentVersion);
        }

        await LoadFromLocalStorageAsync();

        if (_people.Count == 0)
        {
            await LoadSampleDataFromJsonAsync();
            await SaveToLocalStorageAsync();
        }
    }

    public async Task ClearDataAndReloadAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", VersionKey, CurrentVersion);
        _people.Clear();
        await LoadSampleDataFromJsonAsync();
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    public IEnumerable<Person> GetPeople() => _people.OrderBy(p => p.BirthDate);

    public IEnumerable<Person> SearchPeople(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetPeople();

        var lower = searchTerm.ToLower();
        return _people
            .Where(p => p.Name.ToLower().Contains(lower) ||
                       p.Description.ToLower().Contains(lower))
            .OrderBy(p => p.BirthDate);
    }

    public async Task AddPersonAsync(Person person)
    {
        _people.Add(person);
        await SaveToLocalStorageAsync();
        NotifyStateChanged();
    }

    public async Task UpdatePersonAsync(Person person)
    {
        var index = _people.FindIndex(p => p.Id == person.Id);
        if (index != -1)
        {
            _people[index] = person;
            await SaveToLocalStorageAsync();
            NotifyStateChanged();
        }
    }

    public async Task DeletePersonAsync(Guid id)
    {
        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person != null)
        {
            _people.Remove(person);
            await SaveToLocalStorageAsync();
            NotifyStateChanged();
        }
    }

    private async Task LoadFromLocalStorageAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                _people = JsonSerializer.Deserialize<List<Person>>(json, options) ?? new List<Person>();
                
                // Post-process to fix any encoding issues in loaded data
                foreach (var person in _people)
                {
                    person.Name = FixEncodingIssues(person.Name);
                    person.Description = FixEncodingIssues(person.Description);
                }
            }
        }
        catch
        {
            _people = new List<Person>();
        }
    }

    private async Task SaveToLocalStorageAsync()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
        var json = JsonSerializer.Serialize(_people, options);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    private async Task LoadSampleDataFromJsonAsync()
    {
        try
        {
            // Add cache-busting parameter to force fresh load
            var cacheBuster = DateTime.Now.Ticks;
            var response = await _httpClient.GetAsync($"sample-data/scientists.json?v={cacheBuster}");
            
            if (response.IsSuccessStatusCode)
            {
                // Get the response as string with proper encoding handling
                var jsonString = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var scientistData = JsonSerializer.Deserialize<List<ScientistData>>(jsonString, options);

                if (scientistData != null)
                {
                    _people = scientistData.Select(s => new Person
                    {
                        Name = FixEncodingIssues(s.Name),
                        BirthDate = DateTime.Parse(s.BirthDate),
                        DeathDate = string.IsNullOrEmpty(s.DeathDate) ? null : DateTime.Parse(s.DeathDate),
                        Description = FixEncodingIssues(s.Description)
                    }).ToList();
                }
            }
            else
            {
                // Fallback to hardcoded data if JSON file can't be loaded
                LoadSampleDataFallback();
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error loading JSON: {ex.Message}");
            // Fallback to hardcoded data if there's any error
            LoadSampleDataFallback();
        }
    }

    private string FixEncodingIssues(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Fix common encoding issues
        var fixes = new Dictionary<string, string>
        {
            { "Schr�dinger", "Schrödinger" },
            { "Schrdinger", "Schrödinger" },
            { "Schr?dinger", "Schrödinger" },
            { "Schr\u00f6dinger", "Schrödinger" } // This should already be correct but just in case
        };

        var result = input;
        foreach (var fix in fixes)
        {
            result = result.Replace(fix.Key, fix.Value);
        }

        return result;
    }

    private void LoadSampleDataFallback()
    {
        _people = new List<Person>
        {
            new Person
            {
                Name = "Isaac Newton",
                BirthDate = new DateTime(1643, 1, 4),
                DeathDate = new DateTime(1727, 3, 31),
                Description = "Laws of motion and universal gravitation"
            },
            new Person
            {
                Name = "Albert Einstein",
                BirthDate = new DateTime(1879, 3, 14),
                DeathDate = new DateTime(1955, 4, 18),
                Description = "Theory of relativity"
            },
            new Person
            {
                Name = "Erwin Schrödinger",
                BirthDate = new DateTime(1887, 8, 12),
                DeathDate = new DateTime(1961, 1, 4),
                Description = "Wave mechanics and quantum theory"
            }
        };
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
