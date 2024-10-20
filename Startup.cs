using System.Text;
using Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("AllowAllOrigins",
				builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader());
		});

		var key = Encoding.UTF8.GetBytes(Constants.JwtSecretKey);
		services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = true;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidIssuer = Constants.Issuer,
					ValidAudience = Constants.Audience
				};
			});

		services.Configure<IdentityOptions>(options =>
		{
			options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
		});


		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddIdentity<User, IdentityRole>()
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

		services.AddControllersWithViews();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseCors("AllowAllOrigins");
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.Use(async (context, next) =>
		{
			Console.WriteLine($"Request: {context.Request.Method}");
			await next.Invoke();
			Console.WriteLine($"Response: {context.Response.StatusCode}");
		});

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
	}
}