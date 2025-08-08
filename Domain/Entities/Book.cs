namespace Domain.Entities;


public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Author> Authors { get; set; } = new List<Author>();

    public List<string> Tags { get; set; } = new(); // Примеры: "роман", "19 век", "реализм"
}