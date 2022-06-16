using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Views.Shared.Components.SearchWidget
{
    public class SearchWidgetViewComponent:ViewComponent
    {
        IVersionedRepository _rep;
        public SearchWidgetViewComponent(IVersionedRepository rep)
        {
            _rep = rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SearchViewModel();
            int releaseCode = Helpers.ReleaseHelpers.GetCurrentReleaseNumber(HttpContext);
            model.VinSearch.Models = _rep.GetAllVinModels(releaseCode);
            return View("Default", model);
        }

    }
}
