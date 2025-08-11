using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace webapi.Controllers;

[ApiController]
[Route("redis")]
public class RedisController : ControllerBase
{
    private readonly IDistributedCache _redis;
    public RedisController
    (
        IDistributedCache redis
    )
    {
        _redis = redis;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllData(CancellationToken cts)
    {
        await _redis.SetStringAsync("key", "value", cts);
        var result = await _redis.GetStringAsync("key", cts);
        return Ok(result);
    }
}
