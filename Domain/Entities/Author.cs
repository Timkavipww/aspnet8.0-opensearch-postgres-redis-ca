namespace Domain.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public List<string> Tags { get; set; } = new List<string>();
}
