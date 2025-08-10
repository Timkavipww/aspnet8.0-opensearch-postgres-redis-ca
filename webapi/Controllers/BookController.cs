using Domain.Entities;
using Domain.Entities.OpensearchModels;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using webapi.Contracts;

namespace webapi.Controllers;

[ApiController]
[Route("books")]
public class BookController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly IOpenSearchClient _opensearch;

    public BookController
    (
        ApplicationDbContext context,
        IOpenSearchClient opensearch)
    {
        _opensearch = opensearch;
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
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> CreateBooksBulk(
    [FromBody] List<CreateBookDTO> dtos,
    CancellationToken cancellationToken)
    {
        if (dtos == null || !dtos.Any())
            return BadRequest("Empty book list.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var books = new List<Book>();

        foreach (var dto in dtos)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Tags = dto.Tags?.ToList() ?? new List<string>()
            };

            if (dto.AuthorIds != null && dto.AuthorIds.Any())
            {
                book.BookAuthors = dto.AuthorIds.Select(authorId => new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                }).ToList();
            }

            books.Add(book);
        }

        await _context.Books.AddRangeAsync(books, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var results = books.Select(book => new BookDTO
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Tags = book.Tags.ToList()
        });

        return Created("", results);
    }
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDTO>>> SearchBooks(
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {

        var response = await _opensearch.SearchAsync<BookSearchModel>(s => s
            .Index("books_items") // индекс, в котором ищем
            .Source(src => src.Includes(i => i
                .Fields(f => f.Title, f => f.Tags, f => f.Description)
            ))
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Wildcard(w => w
                            .Field(f => f.Title)
                            .Value($"*{term.ToLower()}*")
                            .Boost(3)
                        ),
                        sh => sh.Wildcard(w => w
                            .Field(f => f.Tags)
                            .Value($"*{term.ToLower()}*")
                            .Boost(2)
                        ),
                        sh => sh.Wildcard(w => w
                            .Field(f => f.Description)
                            .Value($"*{term.ToLower()}*")
                        )
                    )
                )
            ),
            cancellationToken
        );

        if (!response.IsValid)
        {
            throw new Exception($"Ошибка поиска: {response.OriginalException.Message}", response.OriginalException);
        }

        var books = response.Documents.Select(doc => new BookDTO
        {
            Id = doc.Id,
            Title = doc.Title,
            Description = doc.Description,
            Tags = doc.Tags?.ToList() ?? new List<string>()
        });


        return Ok(books);

    }
    



}
