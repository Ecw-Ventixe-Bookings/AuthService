

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    [MaxLength(100)]
    [ProtectedPersonalData]
    public override string? Email { get; set; }

    [MaxLength(100)]
    [ProtectedPersonalData]
    public override string? UserName { get; set; }

    [MaxLength(30)]
    [ProtectedPersonalData]
    public override string? PhoneNumber { get; set; }
}
