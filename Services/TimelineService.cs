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
            },
            new Person
            {
                Name = "Marie Curie",
                BirthDate = new DateTime(1867, 11, 7),
                DeathDate = new DateTime(1934, 7, 4),
                Description = "Discovered radium and polonium; pioneer in radioactivity research"
            },
            new Person
            {
                Name = "Charles Darwin",
                BirthDate = new DateTime(1809, 2, 12),
                DeathDate = new DateTime(1882, 4, 19),
                Description = "Theory of evolution by natural selection"
            },
            new Person
            {
                Name = "Nikola Tesla",
                BirthDate = new DateTime(1856, 7, 10),
                DeathDate = new DateTime(1943, 1, 7),
                Description = "Electrical engineering innovations, AC power systems"
            },
            new Person
            {
                Name = "Gregor Mendel",
                BirthDate = new DateTime(1822, 7, 20),
                DeathDate = new DateTime(1884, 1, 6),
                Description = "Father of genetics; laws of inheritance"
            },
            new Person
            {
                Name = "Louis Pasteur",
                BirthDate = new DateTime(1822, 12, 27),
                DeathDate = new DateTime(1895, 9, 28),
                Description = "Germ theory of disease; pasteurization process"
            },
            new Person
            {
                Name = "Max Planck",
                BirthDate = new DateTime(1858, 4, 23),
                DeathDate = new DateTime(1947, 10, 4),
                Description = "Quantum theory originator; Planck's constant"
            },
            new Person
            {
                Name = "Werner Heisenberg",
                BirthDate = new DateTime(1901, 12, 5),
                DeathDate = new DateTime(1976, 2, 1),
                Description = "Uncertainty principle; quantum mechanics"
            },
            new Person
            {
                Name = "Dmitri Mendeleev",
                BirthDate = new DateTime(1834, 2, 8),
                DeathDate = new DateTime(1907, 2, 2),
                Description = "Periodic table of chemical elements"
            },
            new Person
            {
                Name = "Galileo Galilei",
                BirthDate = new DateTime(1564, 2, 15),
                DeathDate = new DateTime(1642, 1, 8),
                Description = "Astronomer; telescope observations; heliocentric theory support"
            },
            new Person
            {
                Name = "Michael Faraday",
                BirthDate = new DateTime(1791, 9, 22),
                DeathDate = new DateTime(1867, 8, 25),
                Description = "Electromagnetic induction; electrolysis laws"
            },
            new Person
            {
                Name = "James Clerk Maxwell",
                BirthDate = new DateTime(1831, 6, 13),
                DeathDate = new DateTime(1879, 11, 5),
                Description = "Electromagnetic theory; Maxwell's equations"
            },
            new Person
            {
                Name = "Stephen Hawking",
                BirthDate = new DateTime(1942, 1, 8),
                DeathDate = new DateTime(2018, 3, 14),
                Description = "Black hole physics; theoretical cosmology"
            },
            new Person
            {
                Name = "Wolfgang Pauli",
                BirthDate = new DateTime(1900, 4, 25),
                DeathDate = new DateTime(1958, 12, 15),
                Description = "Pauli exclusion principle; theoretical physics pioneer"
            }
        };
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
