using Microsoft.AspNetCore.Mvc;
using PraksaApp.Models;
using PraksaApp.Data;
using PraksaApp.Services;

namespace PraksaApp.Controllers
{
	public class InviteController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly EmailSender _emailSender;

		public InviteController(ApplicationDbContext context, EmailSender emailSender)
		{
			_context = context;
			_emailSender = emailSender;
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(InviteViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var invite = new Invite
			{
				Email = model.Email,
				Token = Guid.NewGuid().ToString(),
				CreatedAt = DateTime.UtcNow,
				IsUsed = false
			};

			_context.Invites.Add(invite);
			_context.SaveChanges();

			var registrationUrl = Url.Action(
				"Register",
				"Account",
				new { email = invite.Email, token = invite.Token },
				protocol: Request.Scheme);

			await _emailSender.SendEmailAsync(
				model.Email,
				"Service invitation",
				$"<p>You've been invited. Click link below to register:</p<p><a href='{registrationUrl}'>{registrationUrl}</a></p>");

            TempData["InviteLink"] = registrationUrl;

            return RedirectToAction("InviteSent");
		}

		public IActionResult InviteSent()
		{
			return View();
		}
	}
}
