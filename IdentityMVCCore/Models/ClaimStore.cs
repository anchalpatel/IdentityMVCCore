using System.Security.Claims;

namespace IdentityMVCCore.Models
{
    public static class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>(){
           new Claim("Create Role", "Create Role"),
           new Claim("Update Role", "Update Role"),
           new Claim("Delete Role", "Delete Role")
        };
    }
}
