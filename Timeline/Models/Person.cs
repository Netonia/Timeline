namespace Timeline.Models;

public class Person
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string Description { get; set; } = string.Empty;

    public string LifeSpan => DeathDate.HasValue 
        ? $"{BirthDate.Year}–{DeathDate.Value.Year}" 
        : $"{BirthDate.Year}–Present";

    public bool IsAlive => !DeathDate.HasValue;
}
