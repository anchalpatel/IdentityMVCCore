using IdentityMVCCore.Models;
using IdentityMVCCore.Security;
using IdentityMVCCore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace IdentityMVCCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IDataProtectionProvider protector;
        private readonly UserManager<ApplicationUser> userManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger,
                                  IDataProtectionProvider dataProtectionProvider, DataProtectorPurposeString dataPtotectorPurposeString)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.protector = dataProtectionProvider.CreateProtector(dataPtotectorPurposeString.EmployeeIdRoutueValue);
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //GET:/Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, city = model.City };
                var result = await userManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, confirmationUrl);

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("GetUsers", "Administration");
                    }
                    //await signInManager.SignInAsync(user, false);
                    //return RedirectToAction("Index", "Home");
                    ViewBag.ErrorTitle = "Registration successfull";
                    ViewBag.ErrorMessage = "Before you can Login, Please confirm your email by clicking on the confirmation link we have emailed you";
                    return View("Error");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

       
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = "User with id : " + userId + " do not exists";
                return View("Error");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            ViewBag.ErrorMessage = "Email cannot be confirmed";
            return View("Error");
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email : {email} is already in use");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl ?? "/",
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            model.ReturnUrl = returnUrl ?? Url.Content("~/");
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError("", "Email is not confirmed yet");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);  
                if (result.Succeeded)
                {
                   
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (result.IsLockedOut)
                {
                    return View("AccountLockout");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
                return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                returnUrl = returnUrl ?? Url.Content("/");
                //Create an instance of LiginViewModel
                var loginViewModel = new LoginViewModel()
                {
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                };

                //Check if any error is there and if any error occured then redirect user to login page
                if (remoteError != null)
                {
                    ModelState.AddModelError("", "An eror occured while external login " + remoteError);
                    return View("Login", loginViewModel);
                }

                //Now if the error is not occured then get the user information using signInManager.GetExternalLoginInfoAsync
                var info = await signInManager.GetExternalLoginInfoAsync();

                //If info is null then give login error
                if (info == null)
                {
                    ModelState.AddModelError("", "Error occured while Login");
                    return View("Login", loginViewModel);
                }

                //Get user email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                ApplicationUser user = null;

                if (email != null)
                {
                    user = await userManager.FindByEmailAsync(email);
                    if (user != null && !user.EmailConfirmed)
                    {
                        ModelState.AddModelError("", "Email is not confirmed yet");
                        return View("Login", loginViewModel);
                    }
                }
                //if the user is already logged in (i.e. if there is alredy an entry in AspNetUserLogins table) then sign-in user with external login provider
                var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (signInResult.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                //If result is not succeded it means user do not have local account
                else
                {


                    if (email != null)
                    {
                        //Create new user without password if we do not have a user already
                        if (user == null)
                        {
                            user = new ApplicationUser()
                            {
                                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                                city = "Unknown"
                            };
                            var result = await userManager.CreateAsync(user);
                            if (!result.Succeeded)
                            {
                                ViewBag.ErrorTitle = "User creation failed";
                                ViewBag.ErrorMessage = "Please try again or contact support.";
                                return View("Error");
                            }

                            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                            var confirmationUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                            logger.Log(LogLevel.Warning, confirmationUrl);
                            ViewBag.ErrorTitle = "Registration successfull";
                            ViewBag.ErrorMessage = "Before you can Login, Please confirm your email by clicking on the confirmation link we have emailed you";
                            return View("Error");
                        }

                        await userManager.AddLoginAsync(user, info);
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    ViewBag.ErrorTitle = "Email claim not recieved from provide " + info.LoginProvider;
                    ViewBag.ErrorMessage = "Please contanct for supprot on aanchalpatel4404@gmail.com";
                    return View("Error");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred in ExternalLoginCallback.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
            
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Forgotpassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Forgotpassword(ForgotPassword model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && (await userManager.IsEmailConfirmedAsync(user)))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if(email==null || token == null)
            {
                ViewBag.ErrorMessage = "Email and Token are required";
                return View("Error");
            }
            ResetPasswordViewModel model = new ResetPasswordViewModel(){ Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if(await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return RedirectToAction("ResetPassword", new {email = model.Email, token = model.Token});
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            bool hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return View("AddPassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "user Not Found";
                    return View("Error");
                }
                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    return View("ChangePasswordConfirmation");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            bool hasPassword = await userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return View("ChangePassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found";
                    return View("Error");
                }
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if(result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    return View("AddPasswordConfirmation");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction();
        }
    }
}
