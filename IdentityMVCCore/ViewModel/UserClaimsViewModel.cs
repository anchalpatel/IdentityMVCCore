﻿using IdentityMVCCore.Models;

namespace IdentityMVCCore.ViewModel
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel() {
            Claims = new List<UserClaims>();
        }
        public string UserId { get; set; }
        public List<UserClaims> Claims { get; set; }
    }
}
