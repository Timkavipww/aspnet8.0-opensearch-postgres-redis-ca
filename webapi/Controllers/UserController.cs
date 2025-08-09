using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Contracts;

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
    [HttpPost("/")]
    public async Task<ActionResult> AddUser(CreateUserDTO user, CancellationToken cts)
    {
        var newuser = new User
        {
            Email = user.Email,
            Name = user.Name
        };

        await _context.AddAsync(newuser, cts);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
