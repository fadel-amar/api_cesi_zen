using System.ComponentModel;
using CesiZen_API.DTO;
using CesiZen_API.Helper;
using CesiZen_API.Mapper;
using CesiZen_API.ModelBlinders;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllPages()
        {
            bool isAdmin = User.IsInRole("Admin");
            IEnumerable<Page>? pages = await _pageService.GetAllPages(isAdmin);
            return Ok(PageMapper.ToResponseListDto(pages));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPageById(int id)
        {
            bool isAdmin = User.IsInRole("Admin");
            Page page = await _pageService.GetPageById(id, isAdmin);
            return Ok(PageMapper.ToResponseFullDto(page));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePage([FromBody] CreatePageDto createPageDto, [CurrentUser] User user)
        {

            Page createdPage = await _pageService.CreatePage(createPageDto, user);

            return StatusCode(201, new
            {
                status = 201,
                message = "Le menu a bien été créé",
                data = PageMapper.ToResponseFullDto(createdPage)
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePage(int id, [FromBody] UpdatePageDto updatedPageDto, [CurrentUser] User user)
        {
            bool updated = await _pageService.UpdatePage(id, updatedPageDto, user);
            Page updatePage = await _pageService.GetPageById(id, true);
            return StatusCode(200, new
            {
                status = 200,
                message = "Le menu a bien été modifié",
                data = PageMapper.ToResponseFullDto(updatePage)
            });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePage(int id)
        {
            await _pageService.DeletePage(id);
            return StatusCode(204);
        }
    }
}
