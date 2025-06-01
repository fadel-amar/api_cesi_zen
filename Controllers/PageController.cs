using System.ComponentModel;
using CesiZen_API.DTO;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllPages([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {

            var (totalPages, pages) = await _pageService.GetAllPages(pageNumber, pageSize, filter);

            return Ok(new
            {
                Pages = pages,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPageById(int id)
        {
            var page = await _pageService.GetPageById(id);
            if (page == null)
            {
                return NotFound();
            }
            return Ok(page);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePage([FromBody] CreatePageDto createPageDto)
        {
            Page newPage = new Page
            {
                Title = createPageDto.Title,
                Content = createPageDto.Content,
            };
            if (createPageDto.link != null)
            {
                newPage.link = createPageDto.link;
                if (createPageDto.type_link != null)
                {
                    newPage.type_link = createPageDto.type_link;
                }
                else
                {
                    return BadRequest("Le type de lien est obligatoire");
                }
            }

            Page createdPage = await _pageService.CreatePage(newPage);
            return CreatedAtAction(nameof(GetPageById), new { id = createdPage.Id }, createdPage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePage(int id, [FromBody] Page page)
        {
            return Ok();
        }



    }
}
