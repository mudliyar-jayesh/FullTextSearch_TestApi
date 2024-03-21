using Microsoft.AspNetCore.Mvc;
using Services;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly CacheService _cacheService;

        public TestController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string searchText)
        {
            var result = _cacheService.Search(searchText);
            return Ok(result);
        }

        [HttpGet]
        [Route("enqueue")]
        public IActionResult Enqueue(string key, string value)
        {
            _cacheService.Enqueue(key, value);
            return Ok();
        }
    }
}
