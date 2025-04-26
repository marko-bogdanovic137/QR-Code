using System.ComponentModel.DataAnnotations;

namespace PraksaApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
