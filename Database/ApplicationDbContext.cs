using Microsoft.EntityFrameworkCore;
using Models;

namespace Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; } = null!;
}