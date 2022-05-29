using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using System.Collections.Generic;

namespace openPER.Views.Shared.Components.VinSearchWidget
{
    public class VinSearchWidgetViewComponent:ViewComponent
    {
        IRepository rep;
        public VinSearchWidgetViewComponent(IRepository _rep)
        {
            rep = _rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new VinSearchViewModel();
            model.Models = rep.GetAllModels();

            return View("Default", model);
        }
    }
}
