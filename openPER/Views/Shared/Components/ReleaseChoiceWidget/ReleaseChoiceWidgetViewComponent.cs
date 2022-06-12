using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;

namespace openPER.Views.Shared.Components.ReleaseChoiceWidget
{
    public class ReleaseChoiceWidgetViewComponent: ViewComponent
    {
        readonly IVersionedRepository _rep;
        readonly IConfiguration _config;
        public ReleaseChoiceWidgetViewComponent
            (IVersionedRepository rep, IConfiguration config)
        {
            _rep = rep;
            _config = config;
        }
        public IViewComponentResult Invoke()
        {
            // TODO work out what to do about repository release number
            var s = _config.GetSection("Releases").Get<ReleaseModel[]>();
            var model = new SessionOptionsViewModel
            {
                Versions = new List<VersionModel>()
            };
            foreach (var release in s)
            {
                var v = new VersionModel
                {
                    Release = release.Release,
                    Description = release.Description
                };
                model.Versions.Add(v);

            }

            model.CurrentVersion = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);
            return View("Default", model);
        }
    }
}
