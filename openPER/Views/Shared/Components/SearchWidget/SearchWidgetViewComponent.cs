using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Views.Shared.Components.SearchWidget
{
    public class SearchWidgetViewComponent:ViewComponent
    {
        IRepository _rep;
        public SearchWidgetViewComponent(IRepository rep)
        {
            _rep = rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SearchViewModel();
            model.VinSearch.Models = _rep.GetAllVinModels();
            return View("Default", model);
        }

    }
}
