using Domain.Constants.Indicies;
using Domain.Entities;
using Domain.Entities.OpensearchModels;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;

namespace Infrastructure.Opensearch;

public class IndexingService
{
    private readonly ApplicationDbContext _context;
    private readonly IOpenSearchClient _opensearch;
    public IndexingService
    (
        ApplicationDbContext context,
        IOpenSearchClient opensearch
    )
    {
        _context = context;
        _opensearch = opensearch;
    }
    public async Task ReindexAsync(CancellationToken cts)
    {
        var books = await _context.Books.ToListAsync(cts);
        var authors = await _context.Authors.ToListAsync(cts);

        if (!books.Any())
            throw new Exception("no data to reindex");

        var booksData = NormalizeData(books);
        var authorsData = NormalizeData(authors);

        // Проверка и создание BOOK_INDEX
        var bookIndexExists = await _opensearch.Indices.ExistsAsync(OpenSearchIndicies.BOOK_INDEX, null, cts);
        if (!bookIndexExists.Exists)
        {
            var createBookIndex = await _opensearch.Indices.CreateAsync(OpenSearchIndicies.BOOK_INDEX,
                c => c.Map<BookSearchModel>(m => m.AutoMap()), cts);

            if (!createBookIndex.IsValid)
                throw new Exception("Failed to create book index: " + createBookIndex.DebugInformation);
        }

        // Проверка и создание AUTHOR_INDEX
        var authorIndexExists = await _opensearch.Indices.ExistsAsync(OpenSearchIndicies.AUTHOR_INDEX, null, cts);
        if (!authorIndexExists.Exists)
        {
            var createAuthorIndex = await _opensearch.Indices.CreateAsync(OpenSearchIndicies.AUTHOR_INDEX,
                c => c.Map<AuthorSearchModel>(m => m.AutoMap()), cts);

            if (!createAuthorIndex.IsValid)
                throw new Exception("Failed to create author index: " + createAuthorIndex.DebugInformation);
        }

        var bookIndex = await _opensearch.Indices.ExistsAsync(OpenSearchIndicies.BOOK_INDEX, null, cts);
        var authorIndex = await _opensearch.Indices.ExistsAsync(OpenSearchIndicies.AUTHOR_INDEX, null, cts);
        if (bookIndex.Exists && authorIndex.Exists)
        {
            var bulkIndexResponse1 = await _opensearch
                .BulkAsync(b => b.Index(OpenSearchIndicies.AUTHOR_INDEX).IndexMany(authorsData));

            if (!bulkIndexResponse1.IsValid)
                throw new Exception(
                    $"Failed to index books: {bulkIndexResponse1.ServerError?.Error?.Reason}"
                    );
            var bulkIndexResponse2 = await _opensearch
                .BulkAsync(b => b.Index(OpenSearchIndicies.BOOK_INDEX).IndexMany(booksData));

            if (!bulkIndexResponse2.IsValid)
                throw new Exception(
                    $"Failed to index books: {bulkIndexResponse2.ServerError?.Error?.Reason}"
                    );
        }

    }

    private IEnumerable<BookSearchModel> NormalizeData(IEnumerable<Book> values)
    {
        return values.Select(item => new BookSearchModel
        {
            Id = item.Id,
            Authors = item.BookAuthors.Select(ba => new AuthorNestedModel
            {
                Id = ba.Author.Id,
                Name = ba.Author.Name
            }).ToList(),
            Description = item.Description,
            Tags = item.Tags.ToList(),
            Title = item.Title
        });

    }
    private IEnumerable<AuthorSearchModel> NormalizeData(IEnumerable<Author> values)
    {
        return values.Select(item => new AuthorSearchModel
        {
            Id = item.Id,
            Books = item.BookAuthors.Select(ba => new BookNestedModel
            {
                Id = ba.Book.Id,
                Title = ba.Book.Title
            }).ToList(),
            Name = item.Name,
            Tags = item.Tags.ToList()
        });

    }
    
}
