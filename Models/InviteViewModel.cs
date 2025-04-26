using System.ComponentModel.DataAnnotations;

namespace PraksaApp.Models
{
	public class InviteViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
	}
}

