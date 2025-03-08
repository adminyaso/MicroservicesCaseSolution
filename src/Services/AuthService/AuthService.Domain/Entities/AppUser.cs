using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        // Ek alanlar...
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}