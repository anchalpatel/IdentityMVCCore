using IdentityMVCCore.EditModel;
using IdentityMVCCore.Models;
using IdentityMVCCore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IdentityMVCCore.Controllers
{
    //[Authorize(Policy = "UpdatePolicy")]
   // [Authorize(Roles = "HR")] //Specifying bith the roles saperately means user must be the member if bith the roles
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRolesViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole() { Name = model.Name};
                IdentityResult result = await roleManager.CreateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
       
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = "Role with " + id + " is not present";
                return RedirectToAction("NotFound");
            }
            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name
            };
            var users = userManager.Users.ToList();
            foreach(var user in users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
            
        }

        [HttpPost]
       
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with " + model.Id + " is not present";
                return RedirectToAction("NotFound");
            }
            else
            {
                role.Name = model.Name;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(model);
            }
           
            

        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with id : " + roleId  +" Not found";
                return RedirectToAction("NotFound");
            }
            var model = new List<UserInRoleView>();
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                var userInRole = new UserInRoleView()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected= false;
                }
                model.Add(userInRole);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserInRoleView> userRoles, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Role with id : " + roleId + " Not found";
                return RedirectToAction("NotFound");
            }
            foreach (var userRole in userRoles)
            {
                var user = await userManager.FindByIdAsync(userRole.UserId);
                IdentityResult result = null;

                if(userRole.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name))){
                   result =  await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRole.IsSelected && await userManager.IsInRoleAsync(user, role.Name)){
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }

               
            }
            return RedirectToAction("EditRole", new {Id = roleId});
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User with Id : " + userId + " do not exists";
                return View("NotFound");
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = userId,
                UserName = user.UserName,
                City = user.city,
                Email = user.Email,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList()
            };
            return View(model); 
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User with Id : " + model.Id + " do not exists";
                return View("NotFound");
            }
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.city = model.City;

            var result = await userManager.UpdateAsync(user);
            if(result.Succeeded)
            {
                return RedirectToAction("GetUsers", "Administration");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        [HttpPost]
        
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User with ID " + id + " not found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("GetUsers");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View("GetUsers");
            }
        }
        [HttpPost]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteROle(string id)
        {
           
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = "User with ID " + id + " not found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("GetRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, error.Description);
                    }
                    return View("GetRoles");
                }
                catch(Exception ex)
                {
                    return View("Error");
                }
            }
        }
        [HttpGet]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> EditRoleOfUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User with id : " + userId +  " Not Found";
                return View("Error");
            }
            else
            {
                var model = new List<RolesForUsers>();
                var roles = roleManager.Roles.ToList();
                foreach (var role in roles)
                {
                    RolesForUsers rolesForUsers = new RolesForUsers()
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                    };
                    if(await userManager.IsInRoleAsync(user, role.Name))
                    {
                        rolesForUsers.IsSelected = true;
                    }
                    else
                    {
                        rolesForUsers.IsSelected = false;
                    }
                    model.Add(rolesForUsers);
                }
                return View("EditRoleOfUser", model);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> EditRoleOfUser(List<RolesForUsers> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = "No user is foudnd with ID : " + userId;
                return View("Error");
            }
            else
            {

                foreach(var role in model)
                {
                    var userRole = await roleManager.FindByIdAsync(role.RoleId);
                    if(userRole == null)
                    {
                        ViewBag.ErrorMessage = "Role " + role.RoleName + " not found";
                        return View("Error");
                    }
                    if(role.IsSelected && !(await userManager.IsInRoleAsync(user, userRole.Name)))
                    {
                        var resullt = await userManager.AddToRoleAsync(user, userRole.Name);
                    }
                    else if(!role.IsSelected && (await userManager.IsInRoleAsync(user, userRole.Name)))
                    {
                        var resullt = await userManager.RemoveFromRoleAsync(user, userRole.Name);
                    }
                }
                return RedirectToAction("EditUser", "Administration", new {userId = userId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageClaims(string userId)
        {
            var user  = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = "User with Id : " + userId + " Not Found";
                return View("Error");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel();

            foreach(var claims in ClaimStore.AllClaims)
            {
                UserClaims userClaims = new UserClaims()
                {
                    ClaimType = claims.Type,
                };
                userClaims.isSelected = existingUserClaims.Any(clm => clm.Type == claims.Type);
                model.Claims.Add(userClaims);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClaims(UserClaimsViewModel model, string userId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {userId} Not Found";
                return View("Error");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            // Remove only relevant claims
            var claimsToRemove = existingUserClaims.Where(c => model.Claims.Any(m => m.ClaimType == c.Type));
            var result = await userManager.RemoveClaimsAsync(user, claimsToRemove);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Add selected claims
            var claimsToAdd = model.Claims.Select(c => new System.Security.Claims.Claim(c.ClaimType, c.isSelected ? "True" : "False")); // Adjust ClaimValue as needed

            result = await userManager.AddClaimsAsync(user, claimsToAdd);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

    }
}
