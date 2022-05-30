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
            if (HttpContext.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                model.CurrentLanguage = HttpContext.Request.Cookies["PreferredLanguage"];
            }
            return View("Default", model);
        }

    }
}
