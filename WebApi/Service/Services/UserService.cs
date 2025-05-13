using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using WebApi.Data.Entities;
using WebApi.Service.Dtos;
using WebApi.Service.Interfaces;
using WebApi.Service.Models;

namespace WebApi.Service.Services;
public class UserService(UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{

    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<bool> CreateUserAsync(UserCreateDto dto)
    {
        var entity = new UserEntity
        {
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(entity, dto.Password);
        return result.Succeeded;
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
