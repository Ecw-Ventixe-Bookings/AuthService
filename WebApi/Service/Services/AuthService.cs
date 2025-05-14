using Microsoft.AspNetCore.Identity;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;

namespace WebApi.Service.Services;

public class AuthService(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signinManager,
    TokenGenerator tokenGen,
    IConfiguration config) : IAuthService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signinManager;
    private readonly TokenGenerator _tokenGen = tokenGen;
    private readonly IConfiguration _config = config;

    public async Task<string> Login(UserLoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded) 
            return string.Empty;

        var userEntity = await _userManager.FindByEmailAsync(dto.Email);
        if (userEntity == null) 
            return string.Empty;

        return _tokenGen.GenerateRsaToken(userEntity.Id, userEntity.Email!);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public string GetPublicKey()
    {
        return _config["JWT:PublicKey"];
    }
}
