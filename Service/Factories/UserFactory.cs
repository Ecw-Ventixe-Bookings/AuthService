

using Data.Entities;
using Service.Dtos;

namespace Service.Factories;

public static class UserFactory
{
    public static UserEntity Create(UserRegisterDto dto) =>
        new UserEntity
        {
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
}
