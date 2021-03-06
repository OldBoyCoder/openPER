using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openPER.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVersionedRepository _rep;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IVersionedRepository repository, IConfiguration config,
            IMapper mapper)
        {
            _logger = logger;
            _rep = repository;
            _config = config;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            var releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var breadcrumb = new BreadcrumbModel();
            _rep.PopulateBreadcrumbDescriptions(releaseCode, breadcrumb, language);

            var model = new MakesViewModel
            {
                Makes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                ReleaseCode = releaseCode,
                Navigation = new NavigationViewModel
                {
                    Breadcrumb = _mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                    SideMenuItems = new SideMenuItemsViewModel
                    {
                        AllMakes = _mapper.Map<List<MakeModel>, List<MakeViewModel>>(_rep.GetAllMakes(releaseCode)),
                    }
                }

            };
            model.Navigation.Breadcrumb.ReleaseCode = releaseCode;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SetLanguage(SessionOptionsViewModel model)
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
                SameSite = SameSiteMode.Strict

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            }; HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage, cookieOptions);
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        [Route("Home/SetLanguage/{LanguageCode}")]
        public IActionResult SetLanguage(string languageCode)
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
                SameSite = SameSiteMode.Strict

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            };
            HttpContext.Response.Cookies.Append("PreferredLanguage", languageCode, cookieOptions);
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        [Route("Home/SetRelease/{ReleaseCode}")]
        public IActionResult SetRelease(int releaseCode)
        {
            //            Request.GetTypedHeaders().Referer.ToString()
            var cookieOptions = new CookieOptions
            {
                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = true,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                SameSite = SameSiteMode.Strict

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            }; HttpContext.Response.Cookies.Append("Release", releaseCode.ToString(), cookieOptions);
            //HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString());
            var pathParts = Request.GetTypedHeaders().Referer.ToString().Split('/').ToArray();
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (int.TryParse(pathParts[i], out int oldRelease))
                {
                    pathParts[i] = releaseCode.ToString();
                    break;
                }
            }

            return Redirect(string.Join('/', pathParts));
        }

        [HttpPost]
        public IActionResult Index(SessionOptionsViewModel model)
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
            HttpContext.Response.Cookies.Append("PreferredLanguage", model.CurrentLanguage, cookieOptions);
            HttpContext.Response.Cookies.Append("Release", model.CurrentVersion.ToString(), cookieOptions);
            var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(model.CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            return RedirectToAction("Index", "Makes", new { ReleaseCode = model.CurrentVersion });
        }

    }
}
