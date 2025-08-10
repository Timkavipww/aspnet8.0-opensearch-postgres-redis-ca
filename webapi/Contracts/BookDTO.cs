using System.Text.Json.Serialization;

namespace webapi.Contracts;

public class BookDTO
{

    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<AuthorDTO> Authors { get; set; } = new();
    public List<string> Tags { get; set; } = new();
       
}
