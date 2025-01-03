using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.ViewModel
{
    public class EmployeeViewModel
    {
        [Required]
        [Display(Name ="FirstName")]
        public string? Firstname { get; set; }
        [Required]
        [Display(Name = "LastName")]
        public string? Lastname { get; set; }

        public decimal? Salary { get; set; }

        public string? Address { get; set; }
    }
}
