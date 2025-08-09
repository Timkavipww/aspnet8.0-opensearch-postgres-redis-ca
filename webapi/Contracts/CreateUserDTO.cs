using System.Text.Json.Serialization;

namespace webapi.Contracts;

public class CreateUserDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
