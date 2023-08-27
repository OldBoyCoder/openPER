using System.Globalization;

namespace openPERHelpers
{
    public static class LanguageSupport
    {
        static readonly List<(string ePerCode, string culture)> Cultures = new()
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
            ("T", "tr"),
            ("R", "ro"),
            ("S", "sr"),
            ("L", "sk"),
            ("H", "hu"),
            ("V", "sv"),
        };
        public static CultureInfo ConvertLanguageCodeToCulture(string languageCode)
        {
            var culture = CultureInfo.GetCultureInfo("en");
            if (Cultures.Exists(x => x.ePerCode == languageCode))
                return CultureInfo.GetCultureInfo(Cultures.Find(x => x.ePerCode == languageCode).culture);
            if (Cultures.Exists(x => x.culture == languageCode))
                return CultureInfo.GetCultureInfo(Cultures.Find(x => x.culture == languageCode).culture);
            return culture;
        }
        // ReSharper disable once UnusedMember.Global
        public static string ConvertCultureToLanguageCode(CultureInfo culture)
        {
            if (Cultures.Exists(x => x.culture == culture.TwoLetterISOLanguageName))
                return Cultures.Find(x => x.culture == culture.TwoLetterISOLanguageName).ePerCode;
            return "3";
        }
        public static string SetCultureBasedOnRoute(string language)
        {
            var newCulture = ConvertLanguageCodeToCulture(language);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
            return language;
        }

        public static string GetIso639CodeFromString(string inputCode)
        {
            if (inputCode.Length == 2)
            {
                // Already an ISO code so just make sure it is one we know
                if (Cultures.Count(x => x.culture == inputCode) > 0)
                    return inputCode;
                // Otherwise fallback to English
                return "en";
            }
            if (inputCode.Length == 1)
            {
                if (Cultures.Exists(x => x.ePerCode == inputCode))
                    return Cultures.Find(x => x.ePerCode == inputCode).culture;
                // Otherwise fallback to English
                return "en";
            }
            return "en";
        }

        public static string GetFiatLanguageCodeFromString(string inputCode)
        {
            if (inputCode.Length == 2)
            {
                // AN ISO code so find the ePer code for this
                if (Cultures.Any(x => x.culture == inputCode))
                    return Cultures.Find(x => x.culture == inputCode).ePerCode; ;
                // Otherwise fallback to English
                return "3";
            }
            if (inputCode.Length == 1)
            {
                // Already a Fiat code so just make sure it is one we know
                if (Cultures.Any(x => x.ePerCode == inputCode))
                    return inputCode;
                // Otherwise fallback to English
                return "3";
            }

            return "3";
        }
    }
}
