using Microsoft.EntityFrameworkCore;
using Models;

namespace Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; } = null!;
}