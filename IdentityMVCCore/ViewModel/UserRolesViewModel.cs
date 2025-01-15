using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.ViewModel
{
    public class UserRolesViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
