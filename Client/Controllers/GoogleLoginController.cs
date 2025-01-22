using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class GoogleLoginController : Controller
    {
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });

        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claims => new
            {
                claims.Issuer,
                claims.OriginalIssuer,
                claims.Type,
                claims.Value
            });
            return Json(claims);
        }
    }
}
