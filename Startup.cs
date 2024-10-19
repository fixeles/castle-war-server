using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

public class Startup
{
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.Configure<IdentityOptions>(options =>
		{
			// options.User.RequireUniqueEmail = true;
			options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
		});


		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddIdentity<User, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddControllersWithViews();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseRouting(); // Это необходимо добавить перед вызовом UseEndpoints

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
	}
}

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options) { }

	public DbSet<User> Users { get; set; } = null!;
	// Другие DbSet'ы и настройки...
}