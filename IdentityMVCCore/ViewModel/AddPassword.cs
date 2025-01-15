using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.ViewModel
{
    public class AddPassword
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Password and COnfirm Password must match")]
        public string ConfirmPassword { get; set; }
    }
}
