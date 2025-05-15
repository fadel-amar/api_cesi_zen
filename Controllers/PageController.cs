using System.ComponentModel;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService ;
        PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllPages([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {

            var (totalPages, pages) = await _pageService.GetAllAsync(pageNumber, pageSize, filter);

            return Ok( new
            {
                Pages = pages,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });
        }


    }
}
