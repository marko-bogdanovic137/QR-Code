using System;
using System.ComponentModel.DataAnnotations;

namespace PraksaApp.Models
{
	public class Invite
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Token {  get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }
		[Required]
		public bool IsUsed { get; set; }

	}
}
