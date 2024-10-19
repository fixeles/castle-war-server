using Database;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DataController(ApplicationDbContext context) : ControllerBase
	{
		[Authorize]
		[HttpGet("user")]
		public async Task<IActionResult> GetUserData()
		{
			var userId = User.FindFirst("Id")?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var user = await context.Users.FindAsync(int.Parse(userId));
			if (user == null)
				return NotFound();

			return Ok(user.SavedData);
		}
		
		[Authorize]
		[HttpPut("user")]
		public async Task<IActionResult> UpdateUserData([FromBody] string savedData)
		{
			var userId = User.FindFirst("Id")?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();

			var user = await context.Users.FindAsync(int.Parse(userId));
			if (user == null)
				return NotFound();

			user.SavedData = savedData;
			await context.SaveChangesAsync();

			return Ok(user);
		}
	}
}