using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.ViewModels;

namespace openPER.Views.Shared.Components.LanguageChoiceWidget
{

    public class LanguageChoiceWidgetViewComponent:ViewComponent
    {
        readonly IRepository _rep;
        public LanguageChoiceWidgetViewComponent(IRepository rep)
        {
            _rep = rep;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SessionOptionsViewModel
            {
                Languages = _rep.GetAllLanguages()
            };
            if (HttpContext.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                model.CurrentLanguage = HttpContext.Request.Cookies["PreferredLanguage"];
            }
            return View("Default", model);
        }

    }
}
