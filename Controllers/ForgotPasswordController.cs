using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PraksaApp.Models;
using PraksaApp.Services;

namespace PraksaApp.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailSender _emailSender;

        public ForgotPasswordController(UserManager<IdentityUser> userManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email address not found.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "ForgotPassword",
                new { Email = model.Email, Token = token }, protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset your PraksaApp password",
                $"<p>You requested a password reset. Click the link below to reset your password:</p><p><a href='{resetLink}'>{resetLink}</a></p>");

            return RedirectToAction("PasswordResetLinkSent");
        }

        public IActionResult PasswordResetLinkSent()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest();

            var model = new ResetPasswordViewModel { Email = email, Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

			var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            Console.WriteLine("succeded error - ", result.Succeeded);

            if (result.Succeeded)
                return RedirectToAction("ResetSuccess");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        public IActionResult ResetSuccess()
        {
            return View();
        }
    }
}
