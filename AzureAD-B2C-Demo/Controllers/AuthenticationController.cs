using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzureAD_B2C_Demo.Controllers
{
    [Produces("application/json")]
    public class AuthenticationController : Controller
    {
        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IActionResult SignUp()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Configuration["Authentication:AzureAdB2C:SignUpPolicyName"]); // "B2C_1_sign_up"
        }

        [HttpPost]
        public async Task SignOut()
        {
            // Sign out the user from the local cookie-based authentication session.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out the user from any currently active authentication session in the Azure AD.
            var scheme = User.FindFirst("tfp").Value;
            await HttpContext.SignOutAsync(scheme); // scheme = "B2C_1_sign_up".
        }

        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Configuration["Authentication:AzureAdB2C:SignInPolicyName"]); // "B2C_1_sign_in"
        }

        public IActionResult EditProfile()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, Configuration["Authentication:AzureAdB2C:ProfileEditingPolicyName"]); // "B2C_1_edit_profile"
        }
    }
}