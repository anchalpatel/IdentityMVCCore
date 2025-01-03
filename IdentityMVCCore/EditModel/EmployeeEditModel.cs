using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.EditModel
{
    public class EmployeeEditModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "FirstName")]
        public string? Firstname { get; set; }
        [Required]
        [Display(Name = "LastName")]
        public string? Lastname { get; set; }

        public decimal? Salary { get; set; }

        public string? Address { get; set; }
    }
}
