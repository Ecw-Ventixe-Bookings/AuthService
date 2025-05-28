using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var result = await _userService.CreateUserAsync(dto);

        return result ? Ok() : Problem("Problem creating user");
    }

    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        
        var result = await _userService.VerifyEmailAsync(dto);
        return result ? Ok() : Problem("Problem verifying Email");
    }

    [HttpGet("{identifier?}")]
    public async Task<IActionResult> GetUser(string? identifier)
    {
        if (identifier == null)
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        var userById = await _userService.GetUserByIdAsync(identifier);
        if (userById != null)
            return Ok(userById);

        var userByEmail = await _userService.GetUserByEmailAsync(identifier);
        if (userByEmail != null)
            return Ok(userByEmail);

        return NotFound($"User with identifier '{identifier}' was not found");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserCreateDto dto)
    {
        if (ModelState.ContainsKey("Password")) ModelState.Remove("Password");

        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var result = await _userService.UpdateUserAsync(dto, id);
        return result ? Ok("User updated successfully") : Problem("Problem updating user");
    }

    [HttpDelete("{id?}")]
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return BadRequest("ID needs to be provided");

        var result = await _userService.DeleteUserAsync(id);
        return result ? Ok() : Problem("Problem deleting user");
    }
}
