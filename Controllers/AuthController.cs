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
public class AuthController(ApplicationDbContext db) : ControllerBase
{
	private readonly PasswordHasher<User> _passwordHasher = new();

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromForm] LoginDTO loginDTO)
	{
		if (db.Users.Any(u => u.UserName == loginDTO.Username))
			return Conflict("Nickname already exists.");

		var newUser = new User();
		newUser.UserName = loginDTO.Username;
		newUser.PasswordHash = _passwordHasher.HashPassword(newUser, loginDTO.Password);
		db.Users.Add(newUser);
		await db.SaveChangesAsync();

		return Ok(new { Token = GenerateJwtToken(newUser) });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
	{
		var user = await db.Users.SingleOrDefaultAsync(p => p.UserName == loginDTO.Username);
		if (user == null)
			return Unauthorized(Constants.Responces.IncorrectLogin);

		var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDTO.Password);
		if (result == PasswordVerificationResult.Failed)
			return Unauthorized(Constants.Responces.IncorrectLogin);

		return Ok(new { Token = GenerateJwtToken(user) });
	}

	private string GenerateJwtToken(User user)
	{
		var claims = new[] { new Claim("Id", user.Id) };
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtSecretKey));

		var token = new JwtSecurityToken(
			audience: Constants.Audience,
			issuer: Constants.Issuer,
			claims: claims,
			expires: DateTime.Now.AddDays(2),
			signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}