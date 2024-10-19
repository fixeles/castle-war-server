using Database;
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
	}
}