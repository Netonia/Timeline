using System.Text.Json;
using Microsoft.JSInterop;
using Timeline.Models;

namespace Timeline.Services;

public class TimelineService
{
    private readonly IJSRuntime _jsRuntime;
    private List<Person> _people = new();
    private const string StorageKey = "timeline_people";

    public event Action? OnChange;

    public TimelineService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        await LoadFromLocalStorageAsync();
        
        if (_people.Count == 0)
        {
            LoadSampleData();
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

    private void LoadSampleData()
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
                Name = "Erwin Schrödinger",
                BirthDate = new DateTime(1887, 8, 12),
                DeathDate = new DateTime(1961, 1, 4),
                Description = "Wave mechanics"
            },
            new Person
            {
                Name = "Niels Bohr",
                BirthDate = new DateTime(1885, 10, 7),
                DeathDate = new DateTime(1962, 11, 18),
                Description = "Atomic structure and quantum theory"
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
                Name = "Richard Feynman",
                BirthDate = new DateTime(1918, 5, 11),
                DeathDate = new DateTime(1988, 2, 15),
                Description = "Quantum electrodynamics"
            }
        };
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
