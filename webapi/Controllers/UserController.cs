using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public UserController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    [HttpGet("/")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();

        return users == null ? NotFound("No users found.") : Ok(users);
    }
}
