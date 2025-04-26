using Microsoft.AspNetCore.Identity;

namespace PraksaApp.Data
{
	public static class DbInitializer
	{
		public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string adminEmail = "admin@admin.com";
			string adminPassword = "Admin_123";

			Console.WriteLine("Seeding roles and admin...");

			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
				if (result.Succeeded)
				{
					Console.WriteLine("Admin role created successfully.");
				}
				else
				{
					Console.WriteLine("Failed to create Admin role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
			else
			{
				Console.WriteLine("Admin role already exists.");
			}

			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				var newAdmin = new IdentityUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true
				};
				var result = await userManager.CreateAsync(newAdmin, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(newAdmin, "Admin");
					Console.WriteLine("Admin user created and added to Admin role.");
				}
				else
				{
					Console.WriteLine("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}
	}
}
