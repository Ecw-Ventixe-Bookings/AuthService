

using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDataContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
}
