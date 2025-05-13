using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entities;

namespace WebApi.Data;

public class AppDataContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
}
