using Microsoft.AspNetCore.Http;

namespace openPER.Helpers
{
    public static class ReleaseHelpers
    {
        public static int GetCurrentReleaseNumber(HttpContext httpContext)
        {
            int releaseCode = 18;
            if (httpContext.Request.Cookies.ContainsKey("Release"))
            {
                releaseCode = int.Parse(httpContext.Request.Cookies["Release"] ?? "18");
            }
            return releaseCode;
        }
    }
}
