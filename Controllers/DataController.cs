using System.Globalization;
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
	public class DataController(AppDbContext db) : ControllerBase
	{
		[HttpPost("user")]
		public async Task<IActionResult> SyncUserData([FromForm] SaveDataDTO dto)
		{
			var userId = User.FindFirst("Id")?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized(Constants.Responces.UserNotFound);

			var user = await db.Users.FindAsync(userId);
			if (user == null)
				return NotFound(Constants.Responces.UserNotFound);


			float dtoPlaytime = float.Parse(dto.Playtime, CultureInfo.InvariantCulture);
			if (dto.ForceClientData || dtoPlaytime > user.Playtime) //todo: add some validation
			{
				user.SavedData = dto.Data;
				user.Playtime = dtoPlaytime;
				await db.SaveChangesAsync();
			}

			return Ok(user.SavedData);
		}
	}
}