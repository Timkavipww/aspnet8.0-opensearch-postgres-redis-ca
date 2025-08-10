using Domain.Entities;
using Domain.Entities.OpensearchModels;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Contracts;

namespace webapi.Controllers;

[ApiController]
[Route("authors")]
public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthorController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить список авторов без книг
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors(CancellationToken cancellationToken)
    {
        var authors = await _context.Authors
            .Select(a => new AuthorDTO
            {
                Id = a.Id,
                Name = a.Name,
                Tags = a.Tags.ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(authors);
    }

    /// <summary>
    /// Получить авторов с их книгами
    /// </summary>
    [HttpGet("with-books")]
    public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthorsWithBooks(CancellationToken cancellationToken)
    {
        var authors = await _context.Authors
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
            .Select(a => new AuthorDTO
            {
                Id = a.Id,
                Name = a.Name,
                Tags = a.Tags.ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(authors);
    }

    /// <summary>
    /// Создать автора
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AuthorDTO>> CreateAuthor([FromBody] CreateAuthorDTO dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        // Если есть теги
        if (dto.Tags != null && dto.Tags.Any())
        {
            author.Tags = dto.Tags.ToList();
        }

        _context.Authors.Add(author);
        await _context.SaveChangesAsync(cancellationToken);

        var result = new AuthorDTO
        {
            Id = author.Id,
            Name = author.Name,
            Tags = author.Tags.ToList()
        };

        return CreatedAtAction(nameof(GetAuthors), new { id = author.Id }, result);
    }
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<AuthorDTO>>> CreateAuthorsBulk(
    [FromBody] List<CreateAuthorDTO> dtos,
    CancellationToken cancellationToken)
    {
        if (dtos == null || !dtos.Any())
            return BadRequest("Empty author list.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authors = new List<Author>();

        foreach (var dto in dtos)
        {
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Tags = dto.Tags?.ToList() ?? new List<string>()
            };

            authors.Add(author);
        }

        await _context.Authors.AddRangeAsync(authors, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var results = authors.Select(author => new AuthorDTO
        {
            Id = author.Id,
            Name = author.Name,
            Tags = author.Tags.ToList()
        });

        return Created("", results);
    }


}
