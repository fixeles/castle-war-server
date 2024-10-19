using Database;
using DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[ApiController]
	public class DataController(ApplicationDbContext db) : ControllerBase
	{
		[HttpGet("user")]
		public async Task<IActionResult> GetUserData()
		{
			var userId = User.FindFirst("Id")?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(Constants.Responces.UserNotFound);

			var user = await db.Users.FindAsync(userId);
			if (user == null)
				return NotFound(Constants.Responces.UserNotFound);

			return Ok(user.SavedData);
		}

		[HttpPost("user")]
		public async Task<IActionResult> UpdateUserData([FromForm] SaveDataDTO dto)
		{
			var userId = User.FindFirst("Id")?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(Constants.Responces.UserNotFound);

			var user = await db.Users.FindAsync(userId);
			if (user == null)
				return NotFound(Constants.Responces.UserNotFound);

			user.SavedData = dto.Data;
			await db.SaveChangesAsync();

			return Ok();
		}
	}
}