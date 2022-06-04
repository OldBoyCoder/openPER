using openPER.Models;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public static class ControllerHelpers
    {
        public static void ResetReleaseCookie(Microsoft.AspNetCore.Http.HttpContext httpContext, int releaseCode)
        {
            httpContext.Response.Cookies.Append("Release", releaseCode.ToString());
        }
    }
}
