using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;

namespace openPER.Views.Shared.Components.LanguageChoiceWidget
{

    public class LanguageChoiceWidgetViewComponent:ViewComponent
    {
        IRepository rep;
        public LanguageChoiceWidgetViewComponent(IRepository _rep)
        {
            rep = _rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new LanguagesViewModel();
            model.Languages = rep.GetAllLanguages();
            return View("Default", model);
        }

    }
}
