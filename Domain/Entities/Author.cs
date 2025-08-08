namespace Domain.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Book> Books { get; set; } = new List<Book>();

    public List<string> Tags { get; set; } = new(); // Примеры: "классик", "романист", "русский"
}
