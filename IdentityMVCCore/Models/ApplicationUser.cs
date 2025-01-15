using Microsoft.AspNetCore.Identity;

namespace IdentityMVCCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string city { get; set; }
    }
}
