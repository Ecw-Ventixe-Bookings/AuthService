using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;

namespace WebApi.Service.Services;

public class AuthService(
    UserManager<UserEntity> userManager,
    TokenGenerator tokenGen,
    IConfiguration config) : IAuthService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TokenGenerator _tokenGen = tokenGen;
    private readonly IConfiguration _config = config;

    public async Task<string> Login(UserLoginDto dto)
    {
        var entity = await _userManager.FindByEmailAsync(dto.Email);
        
        if (entity == null || await _userManager.CheckPasswordAsync(entity, dto.Password) == false) 
            return string.Empty;

        return _tokenGen.GenerateRsaToken(entity.Id, entity.Email!);
    }

    public async Task Logout()
    {
        //  Should render the token invalid (remove the token in frontend)
    }

    public PublicKeyinfo GetPublicKey()
    {
        var publicKey = _config["JWT:PublicKey"];
        var issuer = _config["JWT:Issuer"];
        var audience = _config["JWT:Audience"];

        return new()
        {
            PublicKey = publicKey,
            Issuer = issuer,
            Audience = audience
        };
    }
}
