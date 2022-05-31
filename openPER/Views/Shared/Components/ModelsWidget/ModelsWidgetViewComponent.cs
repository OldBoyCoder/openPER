using Microsoft.AspNetCore.Mvc;
using openPER.Models;
using System.Collections.Generic;

namespace openPER.Views.Shared.Components.ModelsWidget
{
    public class ModelsWidgetViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<ModelModel> models)
        {
            return View("Default", models);
        }
    }
}
