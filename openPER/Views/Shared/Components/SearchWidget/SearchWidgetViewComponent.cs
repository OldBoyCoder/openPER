using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;
namespace openPER.Views.Shared.Components.SearchWidget
{
    public class SearchWidgetViewComponent:ViewComponent
    {
        IRepository rep;
        public SearchWidgetViewComponent(IRepository _rep)
        {
            rep = _rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SearchViewModel();
            model.VinSearch.Models = rep.GetAllModels();

            return View("Default", model);
        }

    }
}
