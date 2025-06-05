

using Service.Dtos;

namespace Service.Interfaces;

public interface IAuthService
{
    public Task<string> Login(UserLoginDto dto);
    public Task Logout();
    public PublicKeyinfo GetPublicKey();
    Task<bool> Register(UserRegisterDto dto);
    Task<bool> VerifyEmailAsync(VerifyEmailDto dto);
}
