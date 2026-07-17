using Microsoft.AspNetCore.Identity;

namespace company_profile.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
