using Microsoft.AspNetCore.Identity;

namespace AlgeibaGoAPI.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
    }
}
