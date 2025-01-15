using System.ComponentModel.DataAnnotations;

namespace IdentityMVCCore.Utility
{
    public class ValidateEmailDomain : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidateEmailDomain(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            string[] emailParts = value.ToString().Split('@');
            return emailParts[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
