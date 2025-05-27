using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Data.Entities;

namespace WebApi.Service.Services;

public class TokenGenerator(IConfiguration config, UserManager<UserEntity> userManger)
{
    private readonly IConfiguration _config = config;
    private readonly UserManager<UserEntity> _userManger = userManger;

    public string GenerateRsaToken(string userId, string Email, string role = "user")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var issuer = _config["JWT:Issuer"];
        var audience = _config["JWT:Audience"];
        var privateKeyBase64 = _config["JWT:PrivateKey"];

        var rsa = RSA.Create();
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        var key = new RsaSecurityKey(rsa);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, Email),
            new(ClaimTypes.Role, role),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateSymetricToken(string userId, string Email, string role = "user")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes( _config["JWT:JwtSecret"]);
        var issuer = _config["JWT:Issuer"];
        var audience = _config["JWT:Audience"];

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, Email),
            new(ClaimTypes.Role, role),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    
}
