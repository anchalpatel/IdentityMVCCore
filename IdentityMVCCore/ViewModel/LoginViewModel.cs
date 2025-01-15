using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.ViewModel
{
    public class LoginViewModel
    {
  
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        [ValidateNever]
        public string ReturnUrl { get; set; }
        [ValidateNever]
        public List<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
