using System.Globalization;
using System.Threading;

namespace openPER.Helpers
{
    public static class Converters
    {
        public static CultureInfo ConvertLanguageCodeToCulture(string languageCode)
        {
            var culture = CultureInfo.GetCultureInfo("en");
            switch (languageCode)
            {
                case "0": // Italian
                    culture = CultureInfo.GetCultureInfo("it"); ;
                    break;
                case "1": // French
                    culture = CultureInfo.GetCultureInfo("fr"); ;
                    break;
                case "2": // Spanish
                    culture = CultureInfo.GetCultureInfo("es"); ;
                    break;
                case "3": // English
                    culture = CultureInfo.GetCultureInfo("en"); ;
                    break;
                case "4": // German
                    culture = CultureInfo.GetCultureInfo("de"); ;
                    break;
                case "5": // Portuguese
                    culture = CultureInfo.GetCultureInfo("pt"); ;
                    break;
                case "6": // Polish
                    culture = CultureInfo.GetCultureInfo("pl"); ;
                    break;
                case "7": // Dutch
                    culture = CultureInfo.GetCultureInfo("nl"); ;
                    break;
                case "9": // Danish
                    culture = CultureInfo.GetCultureInfo("da"); ;
                    break;
                case "B": // Czech
                    culture = CultureInfo.GetCultureInfo("cs"); ;
                    break;
                case "D": // Greek
                    culture = CultureInfo.GetCultureInfo("el"); ;
                    break;
                case "J": // Japanese
                    culture = CultureInfo.GetCultureInfo("ja"); ;
                    break;
                case "K": // Chinese
                    culture = CultureInfo.GetCultureInfo("zh"); ;
                    break;
                case "N": // Russian
                    culture = CultureInfo.GetCultureInfo("ru"); ;
                    break;
                case "T": // Turkish
                    culture = CultureInfo.GetCultureInfo("tr"); ;
                    break;

                default:
                    break;


            }

            return culture;
        }
    }
}
