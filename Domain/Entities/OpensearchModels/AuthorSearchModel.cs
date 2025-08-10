namespace Domain.Entities.OpensearchModels;

public class AuthorSearchModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    // Список книг автора
    public List<BookNestedModel> Books { get; set; } = new();

    // Список тегов автора
    public List<string> Tags { get; set; } = new();
}

public class AuthorNestedModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class BookNestedModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
}