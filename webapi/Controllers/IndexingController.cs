using Infrastructure.Opensearch;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

[ApiController]
[Route("indexing")]
public class IndexingController : ControllerBase
{
    private readonly IndexingService _indexingService;

    public IndexingController(IndexingService indexingService)
    {
        _indexingService = indexingService;
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> Reindex(CancellationToken cancellationToken)
    {
        try
        {
            await _indexingService.ReindexAsync(cancellationToken);
            return Ok("Reindexing completed successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during reindexing: {ex.Message}");
        }
    }
}
