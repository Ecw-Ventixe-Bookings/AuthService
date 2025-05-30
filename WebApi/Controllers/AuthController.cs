using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;
using WebApi.Service.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;


    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Register(dto);

        return result ? Ok() : Problem("Problem creating user");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwt = await _authService.Login(dto);

        return string.IsNullOrEmpty(jwt)
            ? BadRequest()
            : Ok(jwt);
    }


    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok();
    }


    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid input", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

        var result = await _authService.VerifyEmailAsync(dto);
        return result ? Ok() : Problem("Problem verifying Email");
    }


    [HttpGet("jwtpk")]
    public IActionResult GetPublicKey()
    {
        var pubKeyInfo = _authService.GetPublicKey();
        return Ok(pubKeyInfo);
    }
}
