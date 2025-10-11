using System.Text.Json;
using Microsoft.JSInterop;
using Timeline.Models;

namespace Timeline.Services;

public class TimelineService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private List<Person> _people = new();
    private const string StorageKey = "timeline_people";

    public event Action? OnChange;

    public TimelineService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        await LoadFromLocalStorageAsync();

        if (_people.Count == 0)
        {
            await LoadSampleDataFromJsonAsync();
            await SaveToLocalStorageAsync();
        }
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
                _people = JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
            }
        }
        catch
        {
            _people = new List<Person>();
        }
    }

    private async Task SaveToLocalStorageAsync()
    {
        var json = JsonSerializer.Serialize(_people);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    private async Task LoadSampleDataFromJsonAsync()
    {
        var response = await _httpClient.GetAsync("sample-data/scientists.json");
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var scientistData = JsonSerializer.Deserialize<List<ScientistData>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (scientistData != null)
            {
                _people = scientistData.Select(s => new Person
                {
                    Name = s.Name,
                    BirthDate = DateTime.Parse(s.BirthDate),
                    DeathDate = string.IsNullOrEmpty(s.DeathDate) ? null : DateTime.Parse(s.DeathDate),
                    Description = s.Description
                }).ToList();
            }
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
