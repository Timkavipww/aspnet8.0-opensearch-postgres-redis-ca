namespace webapi.Contracts;

public class AuthorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
}
