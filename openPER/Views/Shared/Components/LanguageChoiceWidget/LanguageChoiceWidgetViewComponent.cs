using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Views.Shared.Components.LanguageChoiceWidget
{

    public class LanguageChoiceWidgetViewComponent:ViewComponent
    {
        readonly IVersionedRepository _rep;
        public LanguageChoiceWidgetViewComponent(IVersionedRepository rep)
        {
            _rep = rep;
        }
        public IViewComponentResult Invoke()
        {
            // TODO work out what to do about repositor release number
            var model = new SessionOptionsViewModel
            {
                Languages = _rep.GetAllLanguages(18)
            };
            if (HttpContext.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                model.CurrentLanguage = HttpContext.Request.Cookies["PreferredLanguage"];
            }
            return View("Default", model);
        }

    }
}
