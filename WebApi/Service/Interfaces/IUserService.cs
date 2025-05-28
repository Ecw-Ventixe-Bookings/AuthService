using WebApi.Service.Dtos;
using WebApi.Service.Models;

namespace WebApi.Service.Interfaces;

public interface IUserService
{
    public Task<bool> CreateUserAsync(UserCreateDto dto);
    public Task<IEnumerable<UserModel>> GetUsersAsync();
    public Task<UserModel> GetUserByIdAsync(string userId);
    public Task<UserModel> GetUserByEmailAsync(string email);
    public Task<bool> UpdateUserAsync(UserCreateDto dto, string userId);
    public Task<bool> DeleteUserAsync(string userId);
    public Task<bool> VerifyEmailAsync(VerifyEmailDto dto);
}
