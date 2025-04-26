﻿using System.ComponentModel.DataAnnotations;

namespace PraksaApp.Models
{
	public class RegistrationViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

		public string Token { get; set; }
	}
}
