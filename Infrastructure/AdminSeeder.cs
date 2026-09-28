using HotelManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelManagementSystem.Infrastructure;

public static class AdminSeeder
{
    // An administrator is created only when these values are supplied through
    // environment variables. No administrator credential is kept in source control.
    public static async Task SeedAsync(HmsDbContext db, IConfiguration configuration)
    {
        var email = configuration["Admin:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (password.Length < 12 || !password.Any(char.IsUpper) || !password.Any(c => !char.IsLetterOrDigit(c)))
            throw new InvalidOperationException("ADMIN_PASSWORD must have at least 12 characters, one uppercase letter, and one symbol.");

        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email);
        if (user is null)
        {
            db.Users.Add(new AppUser
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
                EmailConfirmed = true
            });
        }
        else if (user.Role == "Admin" && !user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
        }
        else
        {
            // Never silently promote an existing traveller or manager account.
            throw new InvalidOperationException("ADMIN_EMAIL already belongs to a non-admin account. Choose a dedicated administrator email.");
        }

        await db.SaveChangesAsync();
    }
}
