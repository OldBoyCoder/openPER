using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;
using System.Collections.Generic;
namespace openPER.Views.Shared.Components.SearchWidget
{
    public class SearchWidgetViewComponent:ViewComponent
    {
        IVersionedRepository rep;
        public SearchWidgetViewComponent(IVersionedRepository _rep)
        {
            rep = _rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SearchViewModel();
            int releaseCode = -1;
            if (HttpContext.Request.Cookies.ContainsKey("Release"))
            {
                releaseCode = int.Parse(HttpContext.Request.Cookies["Release"]);
            }
            model.VinSearch.Models = rep.GetAllModels(releaseCode);

            return View("Default", model);
        }

    }
}
