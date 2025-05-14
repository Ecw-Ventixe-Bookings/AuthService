using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Service.Interfaces;
using WebApi.Service.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("authServiceConnection") ?? throw new ArgumentNullException("Connectionstring for AuthServe could not be found");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDataContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<UserEntity, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDataContext>()
.AddDefaultTokenProviders();
    

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
