using Microsoft.AspNetCore.Http;

namespace openPER.Controllers
{
    public static class ControllerHelpers
    {
        public static void ResetReleaseCookie(Microsoft.AspNetCore.Http.HttpContext httpContext, int releaseCode)
        {
            var cookieOptions = new CookieOptions
            {
                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = true,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                //SameSite = SameSiteMode.None

                // The client should follow its default cookie policy.
                SameSite = SameSiteMode.Strict
            };
            httpContext.Response.Cookies.Append("Release", releaseCode.ToString(), cookieOptions);
        }
    }
}
