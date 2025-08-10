namespace Domain.Entities.OpensearchModels;

public class BookSearchModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    // Список авторов книги
    public List<AuthorNestedModel> Authors { get; set; } = new();

    // Список тегов книги
    public List<string> Tags { get; set; } = new();
}
