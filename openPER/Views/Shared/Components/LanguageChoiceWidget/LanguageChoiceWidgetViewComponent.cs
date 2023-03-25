using Microsoft.AspNetCore.Http;
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
                Languages = _rep.GetAllLanguages(),
                CurrentLanguage = language,
                Action = RouteData.Values["action"],
                Controller = RouteData.Values["controller"],
                RouteData = new Microsoft.AspNetCore.Routing.RouteValueDictionary()
            };
            foreach (var item in RouteData.Values)
            {
                model.RouteData.Add(item.Key, item.Value);
            }

            foreach (var q in HttpContext.Request.Query)
            {
                model.RouteData.Add(q.Key, q.Value);
            }
            return View("Default", model);
        }

    }
}
