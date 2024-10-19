using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Database;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ApplicationDbContext context) : ControllerBase
{
	private readonly PasswordHasher<User> _passwordHasher = new();

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] LoginDTO loginDTO)
	{
		if (context.Users.Any(u => u.UserName == loginDTO.Username))
			return Conflict("Nickname already exists.");

		var newUser = new User();
		newUser.UserName = loginDTO.Username;
		newUser.PasswordHash = _passwordHasher.HashPassword(newUser, loginDTO.Password);
		context.Users.Add(newUser);
		await context.SaveChangesAsync();

		return Ok(new { Token = GenerateJwtToken(newUser) });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
	{
		var user = await context.Users.SingleOrDefaultAsync(p => p.UserName == loginDTO.Username);
		if (user == null)
			return Unauthorized();

		var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDTO.Password);
		if (result == PasswordVerificationResult.Failed)
			return Unauthorized();

		return Ok(new { Token = GenerateJwtToken(user) });
	}

	private string GenerateJwtToken(User user)
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim("Id", user.Id)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SymmetricSecurityKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: null,
			audience: null,
			claims: claims,
			expires: DateTime.Now.AddDays(2),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}