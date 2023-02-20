using Microsoft.AspNetCore.Mvc;
using openPER.ViewModels;
using openPERRepositories.Interfaces;

namespace openPER.Views.Shared.Components.LanguageChoiceWidget
{

    public class LanguageChoiceWidgetViewComponent : ViewComponent
    {
        readonly IRepository _rep;
        public LanguageChoiceWidgetViewComponent(IRepository rep)
        {
            _rep = rep;
        }
        public IViewComponentResult Invoke(string language)
        {
            var model = new SessionOptionsViewModel
            {
                Languages = _rep.GetAllLanguages()
            };
            model.CurrentLanguage = language;
            model.Action = RouteData.Values["action"];
            model.Controller = RouteData.Values["controller"];
            model.RouteData = new Microsoft.AspNetCore.Routing.RouteValueDictionary(); ;
            foreach (var item in RouteData.Values)
            {
                model.RouteData.Add(item.Key, item.Value);
            }
            return View("Default", model);
        }

    }
}
