using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace openPER.Helpers
{
    public static class LanguageSupport
    {
        static List<(string ePerCode, string culture)> cultures = new List<(string ePerCode, string culture)>
        {
            ("0", "it"),
            ("1", "fr"),
            ("2", "es"),
            ("3", "en"),
            ("4", "de"),
            ("5", "pt"),
            ("6", "pl"),
            ("7", "nl"),
            ("9", "da"),
            ("B", "cs"),
            ("D", "el"),
            ("J", "ja"),
            ("K", "zh"),
            ("N", "ru"),
            ("T", "tr")
        };
        public static CultureInfo ConvertLanguageCodeToCulture(string languageCode)
        {
            var culture = CultureInfo.GetCultureInfo("en");
            if (cultures.Exists(x => x.ePerCode == languageCode))
                return CultureInfo.GetCultureInfo(cultures.Find(x => x.ePerCode == languageCode).culture);
            return culture;
        }
        public static string ConvertCultureToLanguageCode(CultureInfo culture)
        {
            if (cultures.Exists(x => x.culture == culture.TwoLetterISOLanguageName))
                return cultures.Find(x => x.culture == culture.TwoLetterISOLanguageName).ePerCode;
            return "3";
        }
        public static string SetCultureBasedOnCookie(HttpContext context)
        {
            var language = "3";
            if (context.Request.Cookies.ContainsKey("PreferredLanguage"))
            {
                language = context.Request.Cookies["PreferredLanguage"];
                var newCulture = Helpers.LanguageSupport.ConvertLanguageCodeToCulture(language);
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
            return language;

        }
    }
}
