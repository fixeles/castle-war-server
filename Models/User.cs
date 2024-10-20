﻿using Microsoft.AspNetCore.Identity;

namespace Models;

public class User : IdentityUser
{
	public string? SavedData { get; set; }
	public float Playtime { get; set; }
}