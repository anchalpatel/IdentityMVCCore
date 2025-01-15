using IdentityMVCCore.Models;
using IdentityMVCCore.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConection")));
builder.Services.AddSingleton<IAuthorizationHandler, AdminCanHandleOnlyOtherAdminRolesAndClaims>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
builder.Services.AddSingleton<DataProtectorPurposeString>();
builder.Logging.AddConsole();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeletePolicy", policy =>
        policy.RequireClaim("Delete Role", "True"));

    options.AddPolicy("CreatePolicy", policy =>
        policy.RequireClaim("Create Role", "True"));
    //options.InvokeHandlersAfterFailure = false;
    options.AddPolicy("UpdatePolicy", policy =>
        policy.AddRequirements(new ManagingAdminRolesAndClaimsRequirement()));

    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(5));
//policy.RequireAssertion(context => 
//context.User.IsInRole("Admin") &&
//context.User.HasClaim(c => c.Type == "Update Role" && c.Value == "True") ||
//context.User.IsInRole("SuperAdmin")));

builder.Services.AddAuthentication().AddGoogle(
    options =>
    {
        options.ClientId = "927260286727-a4c6208lsvpsd60f9a6as6e3kr4b9vui.apps.googleusercontent.com";
        options.ClientSecret = "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";
    })
    .AddFacebook(options =>
    {
        options.ClientId = "1687057275490235";
        options.ClientSecret = "d113d6068c82bdf02e97070b6091c4cd";
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailCOnfirmationToken";
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<EmployeeContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<CustomEmailCnfirmationTokenProviderClass<ApplicationUser>>("CustomEmailCOnfirmationToken");

builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                      .RequireAuthenticatedUser()
                      .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
