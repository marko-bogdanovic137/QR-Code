using Microsoft.AspNetCore.Mvc;
using PraksaApp.Models;
using PraksaApp.Data;
using SQLitePCL;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using Microsoft.AspNetCore.Identity;

namespace PraksaApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ApplicationDbContext _context;

		public AccountController(
			UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager
			, ApplicationDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}

		[HttpGet]
		public IActionResult Register(string email, string token)
		{
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }


            var invite = _context.Invites.FirstOrDefault(i => i.Email == email && i.Token == token);

			if(invite == null || invite.IsUsed)
			{
				return BadRequest("This invite is invalid or has expired.");
			}

			var viewModel = new RegistrationViewModel
			{
				Email = email,
				Token = token
			};
			return View(viewModel); //salje na cshtml
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		//[ActionName("Register")]
		public async Task<IActionResult> Register(RegistrationViewModel model)
		{
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
			{
				Console.WriteLine("Zakucao");
				return View(model);
			}
				
			var existingUser = await _userManager.FindByNameAsync(model.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("", "User already exists");
				return View(model);
			}

			var invite = _context.Invites.FirstOrDefault(i => i.Email == model.Email && i.Token == model.Token);

			if(invite == null)
			{
				return RedirectToAction("InvalidInvite", "Error");
			}

			var user = new IdentityUser
			{
				UserName = model.Email,
				Email = model.Email,
				EmailConfirmed = true
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if(result.Succeeded)
			{
				_context.Invites.Remove(invite);
				await _context.SaveChangesAsync();

				await _signInManager.SignInAsync(user, isPersistent: false);

				return RedirectToAction("Index", "Home");
			}

			foreach(var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View(model);
		}
	}
}
