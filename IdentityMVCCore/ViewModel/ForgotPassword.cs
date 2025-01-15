using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.ViewModel
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
