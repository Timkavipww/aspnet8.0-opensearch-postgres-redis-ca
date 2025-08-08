namespace Domain.Entities;

public class SearchItem
{
    public Guid Id { get; set; }                   // Уникальный идентификатор
    public string Title { get; set; } = null!;     // Название (поисковое поле)
    public string? Description { get; set; }       // Описание (поисковое поле)
    public List<string> Tags { get; set; } = [];   // Теги (поисковое поле)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}