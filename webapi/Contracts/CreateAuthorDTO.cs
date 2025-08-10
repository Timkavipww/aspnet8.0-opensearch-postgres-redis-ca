using System.Text.Json.Serialization;
using Domain.Entities;

namespace webapi.Contracts;

public class CreateAuthorDTO
{
    public string Name { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
    
}
