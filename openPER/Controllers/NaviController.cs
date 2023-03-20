using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPERHelpers;

namespace openPER.Controllers
{
    public class NaviController : Controller
    {
        [Route("/eper/navi")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            // Examine th query string to work out the routing
            var key = HttpContext.Request.Query["KEY"][0];
            if (key == "PARTDRAWDATA")
            {
                //                [Route("Detail/{language}/{MakeCode}/{SubMakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}/{Scope}")]
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);

                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                var modelCode = HttpContext.Request.Query["COMM_MODEL"][0];
                var catalogueCode = HttpContext.Request.Query["CAT_COD"][0];
                var groupCode = HttpContext.Request.Query["GRP_COD"][0];
                var subGroupCode = HttpContext.Request.Query["SGRP_COD"][0];
                var subSubGroupCode = HttpContext.Request.Query["SGS_COD"][0];
                var drawingNumber = "0";
                if (HttpContext.Request.Query.ContainsKey("DRW_NUM"))
                    drawingNumber = (int.Parse(HttpContext.Request.Query["DRW_NUM"][0] ?? string.Empty) - 1).ToString();
                var scope = "SubSubGroup";
                return RedirectToAction("Detail", "Drawings", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode, SubSubGroupCode = subSubGroupCode, DrawingNumber = drawingNumber, Scope = scope });
            }
            if (key == "HOME")
            {
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);
                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                return RedirectToAction("Index", "Models", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode });
            }
            if (key == "VERSION")
            {
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);
                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                var modelCode = HttpContext.Request.Query["COMM_MODEL"][0];
                return RedirectToAction("Index", "Catalogues", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode });
            }
            if (key == "GROUP")
            {
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);
                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                var modelCode = HttpContext.Request.Query["COMM_MODEL"][0];
                var catalogueCode = HttpContext.Request.Query["CAT_COD"][0];
                return RedirectToAction("Index", "Groups", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode });
            }
            if (key == "SUBGROUP")
            {
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);
                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                var modelCode = HttpContext.Request.Query["COMM_MODEL"][0];
                var catalogueCode = HttpContext.Request.Query["CAT_COD"][0];
                var groupCode = HttpContext.Request.Query["GRP_COD"][0];
                return RedirectToAction("Index", "SubGroups", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode });
            }
            if (key == "SUBGROUP_7")
            {
                var language = HttpContext.Request.Query["LANGUAGE"][0];
                language = LanguageSupport.GetIso639CodeFromString(language);
                var makeCode = HttpContext.Request.Query["MAKE"][0];
                var subMakeCode = HttpContext.Request.Query["SBMK"][0];
                var modelCode = HttpContext.Request.Query["COMM_MODEL"][0];
                var catalogueCode = HttpContext.Request.Query["CAT_COD"][0];
                var groupCode = HttpContext.Request.Query["GRP_COD"][0];
                var subGroupCode = HttpContext.Request.Query["SGRP_COD"][0];
                return RedirectToAction("Index", "SubSubGroups", new { Language = language, MakeCode = makeCode, SubMakeCode = subMakeCode, ModelCode = modelCode, CatalogueCode = catalogueCode, GroupCode = groupCode, SubGroupCode = subGroupCode });
            }

            return RedirectToAction("Index", "Home");
        }
        //http://eper.fiatforum.com/eper/navi?MOD_COD=183&COUNTRY=012&GRP_COD=102&CAT_COD=PK&SBMK=F&DRIVE=D&MAKE=F&COMM_MODEL=BAR&ALL_FIG=0&LANGUAGE=3&PREVIOUS_KEY=PARTDRAWDATA&NEW_HTTP=TRUE&ALL_LIST_PART=0&SB_CODE=-1&KEY=PARTDRAWDATA&PRINT_MODE=0&EPER_CAT=SP&GUI_LANG=3&WINDOW_ID=1&SGRP_COD=1&SGS_COD=0&DRW_NUM=3
        //https://localhost:44329/eper/navi?MOD_COD=183&COUNTRY=012&GRP_COD=102&CAT_COD=PK&SBMK=F&DRIVE=D&MAKE=F&COMM_MODEL=BAR&ALL_FIG=0&LANGUAGE=3&PREVIOUS_KEY=PARTDRAWDATA&NEW_HTTP=TRUE&ALL_LIST_PART=0&SB_CODE=-1&KEY=PARTDRAWDATA&PRINT_MODE=0&EPER_CAT=SP&GUI_LANG=3&WINDOW_ID=1&SGRP_COD=1&SGS_COD=0&DRW_NUM=3
        //        

    }
}
