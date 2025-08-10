using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Contracts;

namespace webapi.Controllers;

[ApiController]
[Route("books")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить список книг с их авторами
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks(CancellationToken cancellationToken)
    {
        var books = await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Tags = b.Tags.ToList(),
                Authors = b.BookAuthors
                    .Select(ba => new AuthorDTO
                    {
                        Id = ba.Author.Id,
                        Name = ba.Author.Name
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(books);
    }

    /// <summary>
    /// Получить список книг без авторов
    /// </summary>
    [HttpGet("without-authors")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksWithoutAuthors(CancellationToken cancellationToken)
    {
        var books = await _context.Books
            .Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Tags = b.Tags.ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(books);
    }

    /// <summary>
    /// Создать книгу
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookDTO>> CreateBook([FromBody] CreateBookDTO dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description
        };

        // Если есть теги
        if (dto.Tags != null && dto.Tags.Any())
        {
            book.Tags = dto.Tags.ToList();
        }

        // Если есть авторы
        if (dto.AuthorIds != null && dto.AuthorIds.Any())
        {
            book.BookAuthors = dto.AuthorIds.Select(authorId => new BookAuthor
            {
                BookId = book.Id,
                AuthorId = authorId
            }).ToList();
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        var result = new BookDTO
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Tags = book.Tags.ToList()
        };

        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, result);
    }
}
