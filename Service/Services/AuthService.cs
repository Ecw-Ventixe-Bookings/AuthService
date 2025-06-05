

using Service.Dtos;
using Service.Interfaces;

namespace Service.Services;

internal class AuthService : IAuthService
{
    public PublicKeyinfo GetPublicKey()
    {
        throw new NotImplementedException();
    }

    public Task<string> Login(UserLoginDto dto)
    {
        throw new NotImplementedException();
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Register(UserRegisterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyEmailAsync(VerifyEmailDto dto)
    {
        throw new NotImplementedException();
    }
}
