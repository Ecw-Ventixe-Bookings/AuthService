using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;
using WebApi.Service.Models;

namespace WebApi.Service.Services;
public class UserService(
    UserManager<UserEntity> userManager, 
    RoleManager<IdentityRole> roleManager,
    IConfiguration conf) : IUserService
{

    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _conf = conf;

    public async Task<bool> CreateUserAsync(UserCreateDto dto)
    {
        var entity = new UserEntity
        {
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(entity, dto.Password);

        if (result.Succeeded)
        {
            //  Send verification email through azure service bus <sendEmail>
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(entity);
            var esbConnection = _conf["ESB:Connection"];
            var queue = _conf["ESB:queue"];

            var client = new ServiceBusClient(esbConnection);
            var sender = client.CreateSender(queue);

            var textContent = @$"
                Use the following code to confirm your Email: {code}

                If you did not initiate this request, you can safely discard this email.
            ";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"" />
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px; color: #333333;"">
                    <p>Use the following code to confirm your Email: {code} </p>
                </body>
            ";

            var bodyMessage = new EsbMessage(entity.Email, textContent, htmlContent);

            var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(bodyMessage));
            serviceBusMessage.Subject = $"Your code: {code}";
            serviceBusMessage.ContentType = "application/json";

            await sender.SendMessageAsync(serviceBusMessage);
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }

        

        return result.Succeeded;
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailDto dto)
    {
        try
        {
            var entity = await _userManager.FindByEmailAsync(dto.email);
            var decodedToken = WebUtility.UrlDecode(dto.code);
            var result = await _userManager.ConfirmEmailAsync(entity, decodedToken);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            return false;
        }        
    }

    public async Task<IEnumerable<UserModel>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Select(u => new UserModel
        {
            Id = u.Id,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
        });
    }

    public async Task<UserModel> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);


        if (user != null)
            return new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

        return null!;
    }

    public async Task<UserModel> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
            return new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

        return null!;
    }

    public async Task<bool> UpdateUserAsync(UserCreateDto dto, string userId)
    {
        var entity = await _userManager.FindByIdAsync(userId);
        if (entity == null)
            return false;

        entity.Email = dto.Email;
        entity.UserName = dto.Email;
        entity.PhoneNumber = dto.PhoneNumber;

        var result = await _userManager.UpdateAsync(entity);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
