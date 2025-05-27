using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Service.Interfaces;
using WebApi.Service.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("authServiceConnection") ?? throw new ArgumentNullException("Connectionstring for AuthServe could not be found");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? throw new NullReferenceException("JWT Issuer is not present");
var jwtAudience = builder.Configuration["JWT:Audience"] ?? throw new NullReferenceException("JWT Audience is not present");
var jwtKeyBase64 = builder.Configuration["JWT:PublicKey"] ?? throw new NullReferenceException("JWT Public key is not present");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDataContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<UserEntity, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = true;
    opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<AppDataContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        using RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(jwtKeyBase64), out _);

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();
app.MapOpenApi();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
