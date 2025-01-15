using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityMVCCore.Security
{
    public class CustomEmailCnfirmationTokenProviderClass<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailCnfirmationTokenProviderClass(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
