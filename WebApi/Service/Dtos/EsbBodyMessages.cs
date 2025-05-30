namespace WebApi.Service.Dtos;

public record  EsbMsgSendEmail(string To, string TextContent, string HtmlContent);

public record EsbMsgCreateAccount(
    Guid id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? PostalCode,
    string? City
    );