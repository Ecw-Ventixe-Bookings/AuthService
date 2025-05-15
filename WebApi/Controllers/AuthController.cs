using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var jwt = await _authService.Login(dto);

        if (!string.IsNullOrEmpty(jwt))
        {   
            return Ok(jwt);
        }
        return BadRequest();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpGet("jwtpk")]
    public IActionResult GetPublicKey()
    {
        var pubKeyInfo = _authService.GetPublicKey();
        return Ok(pubKeyInfo);
    }
}
