using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Factories;
using WebApi.Service.Interfaces;

namespace WebApi.Service.Services;

public class AuthService(
    UserManager<UserEntity> userManager,
    TokenGenerator tokenGen,
    IConfiguration conf) : IAuthService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly TokenGenerator _tokenGen = tokenGen;
    private readonly IConfiguration _conf = conf;

    public async Task<bool> Register(UserRegisterDto dto)
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
            var createdEntity = await _userManager.FindByEmailAsync(dto.Email);
            if (createdEntity is null) return false;

            var esbConnection = _conf["ESB:Connection"];
            var client = new ServiceBusClient(esbConnection);


            //  Send verification email through azure service bus <sendEmail>
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(createdEntity);
            var emailSender = client.CreateSender("sendemail");

            var htmlContent = EmailContentProvider.EmailComfirmationHtml(dto.FirstName, dto.LastName, code);
            var textContent = EmailContentProvider.EmailComfirmationText(dto.FirstName, dto.LastName, code);

            var emailSenderBody = new EsbMsgSendEmail(entity.Email, textContent, htmlContent);

            var emailSenderserviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(emailSenderBody));
            emailSenderserviceBusMessage.Subject = $"Your code: {code}";
            emailSenderserviceBusMessage.ContentType = "application/json";

            
            await emailSender.SendMessageAsync(emailSenderserviceBusMessage);
            await emailSender.DisposeAsync();

            // Send Account information to AccountService through Azure Service Bus <createaccount>
            var accountSender = client.CreateSender("createaccount");
            var accountSenderBody = new EsbMsgCreateAccount(
                Guid.Parse(createdEntity.Id),
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.Address,
                dto.PostalCode,
                dto.City);

            var accountSenderServiceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(accountSenderBody));
            accountSenderServiceBusMessage.ContentType = "application/json";

            await accountSender.SendMessageAsync(accountSenderServiceBusMessage);
            await accountSender.DisposeAsync();

            // Dispose ESB client
            await client.DisposeAsync();
        }

        return result.Succeeded;
    }

    public async Task<string> Login(UserLoginDto dto)
    {
        var entity = await _userManager.FindByEmailAsync(dto.Email);
        
        if (
            entity == null || 
            await _userManager.CheckPasswordAsync(entity, dto.Password) == false ||
            entity.EmailConfirmed == false) 
                return string.Empty;

        return _tokenGen.GenerateRsaToken(entity.Id, entity.Email!);
    }

    public async Task Logout()
    {
        //  Should render the token invalid (remove the token in frontend)
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

    public PublicKeyinfo GetPublicKey()
    {
        var publicKey = _conf["JWT:PublicKey"];
        var issuer = _conf["JWT:Issuer"];
        var audience = _conf["JWT:Audience"];

        return new()
        {
            PublicKey = publicKey,
            Issuer = issuer,
            Audience = audience
        };
    }
}
