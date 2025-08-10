using System.Text.Json.Serialization;
using Domain.Entities;

namespace webapi.Contracts;

public class CreateBookDTO
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<Guid> AuthorIds { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
