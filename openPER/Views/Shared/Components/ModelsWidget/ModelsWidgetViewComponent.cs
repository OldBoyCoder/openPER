using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using openPERModels;

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
