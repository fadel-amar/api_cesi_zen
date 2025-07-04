﻿using CesiZen_API.DTO;
using CesiZen_API.Mappers;
using CesiZen_API.ModelBlinders;
using CesiZen_API.Models;
using CesiZen_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_API.Controllers
{


    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService), "UserService n'est pas défini");
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
            var result = await _userService.GetAllUsers(pageNumber, pageSize, filter);
            IEnumerable<User> users = result.Users;
            int totalUsers = result.TotalCount;
            return Ok(UserMapper.ToResponseListDto(users, pageNumber, pageSize, totalUsers));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            User? user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(
                 new
                 {
                     status = 404,
                     message = "Aucun utilisateur a été trouvé"
                 });
            }
            return Ok(UserMapper.toResponseFullDto(user));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyAccount([CurrentUser] User user)
        {
            User? currentUser = await _userService.GetUserById(user.Id);
            return Ok(UserMapper.toResponseFullDto(currentUser));
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] PatchDTO userDto)
        {
            User? user = await _userService.UpdateUser(id, userDto);
            return Ok(UserMapper.toResponseFullDto(user));
        }

        [HttpPut("me")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateMyAccount([CurrentUser] User user, UpdateMyAccontDTO updateMyAccontDTO)
        {
            User? userUpdated = await _userService.UpdateMyAccount(user, updateMyAccontDTO);
            return Ok(UserMapper.toResponseFullDto(userUpdated));

        }

        [HttpDelete("me")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteMyAccount([CurrentUser] User user)
        {
            _userService.DeleteUser(user.Id);
            return Ok(new { message = "Votre compte et vos informations ont été supprimé" });
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _userService.DeleteUser(id);
            return Ok(new { message = "Votre compte et vos informations ont été supprimé" });
        }

        [HttpPut("me/reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetMyPassword([CurrentUser] User user, ResetMyPasswordDTO resetMyPasswordDTO)
        {
            bool status = await _userService.ResetMyPassword(user, resetMyPasswordDTO);
            return Ok(new { message = "Votre mot de passe à bien été changé" });
        }

    }
}

