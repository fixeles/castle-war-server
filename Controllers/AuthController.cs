using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public AuthController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpPost("register")]
	public IActionResult Register([FromBody] User user)
	{
		if (_context.Users.Any(u => u.UserName == user.UserName))
		{
			return Conflict("Nickname already exists.");
		}
		_context.Users.Add(user);
		_context.SaveChanges();
		return Ok("User registered successfully.");
	}

	[HttpPost("login")]
	public IActionResult Login([FromBody] User user)
	{
		var existingUser = _context.Users
			.FirstOrDefault(u => u.UserName == user.UserName && u.PasswordHash == user.PasswordHash);
		if (existingUser == null)
		{
			return Unauthorized("Invalid credentials.");
		}
		return Ok("Login successful.");
	}
}