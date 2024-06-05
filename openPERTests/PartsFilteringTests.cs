using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using openPER.Controllers;
using openPER.Helpers;
using openPER.ViewModels;
using openPERModels;
using openPERRepositories.Interfaces;
using openPERRepositories.Repositories;
using System.Reflection;
using System.Text.RegularExpressions;
using VinSearcher;

namespace openPERTests
{
    [TestClass]
    public class PartsFilteringTests
    {
        readonly IRepository _rep;
        readonly IMapper _mapper;
        readonly IConfiguration _cfg;
        private readonly string _pathToVindataCh;
        private readonly string _pathToVindataRt;


        public PartsFilteringTests()
        {
            //  var cfg = new ConfigurationBuilder().AddInMemoryCollection(m).Build();
            _cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>((_) => _cfg);
            services.AddTransient<IRepository, Release84Repository>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var sP = services.BuildServiceProvider();
            _rep = sP.GetService<IRepository>();
            _mapper = sP.GetService<IMapper>();
            var s = _cfg.GetSection("Releases").Get<ReleaseModel[]>();
            var release = s?.FirstOrDefault(x => x.Release == 84);
            if (release == null) return;
            _pathToVindataCh = release.VinDataCh;
            _pathToVindataRt = release.VinDataRt;

        }

        public PartsFilteringTests(IRepository rep, IMapper mapper, IConfiguration config)
        {
            _rep = rep;
            _mapper = mapper;
        }
        private DrawingsViewModel GetDrawingViewModel(string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string vin = "", string mvs = "")
        {
            var c = new DrawingsController(_rep, _mapper, _cfg);
            var model = c.BuildDrawingViewModel(language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, scope, vin, mvs, _ => drawingNumber);
            return model;

        }
        [TestMethod]
        public void BarchettaWiperNoVin()
        {

            var r = GetPartsTableFromUrl($"https://eper.fiatforum.com/en/Drawings/Detail/F/F/BAR/PK/555/14/0/0/SubSubGroup");
            var expectedParts = new List<(string, bool)> { ("46724563", true), ("46443749", true), ("51742734", true), ("5911194", true), ("5911194", true), ("46735258", true), ("46449583", true), ("46735254", true), ("46449584", true), ("16285927", true), ("12647701", true), ("16100811", true), ("46735008", true), ("10718127", true), ("46734873", true), ("12366054", true), ("12374389", true), ("60805088", true), ("46401657", true), ("12642750", true) };
            openPERAssertions.PartsListMatch(expectedParts, r.Parts);
        }
        [TestMethod]
        public async Task BarchettaWiperVin()
        {
            var url = $"https://eper.fiatforum.com/en/Drawings/Detail/F/F/BAR/PK/555/14/0/0/SubSubGroup?MVS=183.829.0.0&VIN=ZFA18300000040583#";
            //var oldPath = await GenerateePerUrl(url);
            var r = GetPartsTableFromUrl(url);
            var expectedParts = new List<(string, bool)> { ("46724563", false), ("46443749", false), ("51742734", true), ("5911194", true), ("5911194", true), ("46735258", false), ("46449583", true), ("46735254", false), ("46449584", true), ("16285927", true), ("12647701", true), ("16100811", true), ("46735008", true), ("10718127", true), ("46734873", true), ("12366054", true), ("12374389", true), ("60805088", false), ("46401657", true), ("12642750", true) };
            openPERAssertions.PartsListMatch(expectedParts, r.Parts);
        }
        [TestMethod]
        public async Task PandaFuelTankNoVin()
        {

            var url = $"https://eper.fiatforum.com/en/Drawings/Detail/F/F/GIN/33/102/0/10/0/SubSubGroup";
            var r = GetPartsTableFromUrl(url);
            //var oldPath = await GenerateePerUrl(url);
            var expectedParts = new List<(string, bool)> { ("51860540", true), ("51860543", true), ("51885814", true), ("51886360", true), ("51886364", true), ("51938320", true), ("51938321", true), ("51938322", true), ("51838394", true), ("51864288", true), ("51864288", true), ("7760269", true), ("811900002", true), ("11129885", true), ("11566224", true), ("15180311", true) };
            openPERAssertions.PartsListMatch(expectedParts, r.Parts);
        }
        [TestMethod]
        public async Task NuovoPandaWithVinFuelTank()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=0&MOD_COD=319&VEHICLE_NO=3229617&MONTH=10&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MVS=319130001000&YEAR=2013&COLOUR=381&MAKE=F&CHASSIS_NO=ZFA31200003229852&DAY_OF_MONTH=21&CAT_COD=33&GRP_COD=102&SGS_COD=10200-010&ENGINE_NO=182605&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&DRW_NUM=1%7C0";
            var req = GetRequestDataFromEperUrl(url);
            //await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51860540", false), ("51860543", false), ("51885814", false), ("51886360", true), ("51886364", false), ("51938320", false), ("51938321", false), ("51938322", false), ("51838394", true), ("51864288", false), ("51864288", false), ("7760269", true), ("811900002", true), ("11129885", true), ("11566224", true), ("15180311", true), };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);


        }
        [TestMethod]
        public async Task NuovoPandaNoVinFuelTank()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=1&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10201-020&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&DRW_NUM=2%7C0";
            var req = GetRequestDataFromEperUrl(url);
            //            await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51886678", true), ("51886679", true), ("51932170", true), ("46751026", true), ("46804425", true), ("51944916", true), ("51979328", true), ("18750221", true), ("51886896", true), };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);
        }
        [TestMethod]
        public async Task NuovoPandaWithMVSFuelTank()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=0&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MVS=319130001000&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10200-010&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            var req = GetRequestDataFromEperUrl(url);
            Console.WriteLine(GetEperUrlFromKeys(req));
            // await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51860540", false), ("51860543", false), ("51885814", false), ("51886360", true), ("51886364", false), ("51938320", false), ("51938321", true), ("51938322", false), ("51838394", true), ("51864288", false), ("51864288", false), ("7760269", true), ("811900002", true), ("11129885", true), ("11566224", true), ("15180311", true), };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);
        }
        [TestMethod]
        public async Task NuovoPandaWithVinFuelTank2()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=0&VEHICLE_NO=03000220&MONTH=9&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MVS=31911L051870&YEAR=2011&COLOUR=376&MAKE=F&CHASSIS_NO=ZFA31200003000439&DAY_OF_MONTH=26&CAT_COD=33&GRP_COD=102&SGS_COD=10200-010&ENGINE_NO=4279741&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            var req = GetRequestDataFromEperUrl(url);
            Console.WriteLine(GetEperUrlFromKeys(req));
//            await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51860540", false), ("51860543", false), ("51885814", false), ("51886360", false), ("51886364", true), ("51938320", false), ("51938321", false), ("51938322", false), ("51838394", true), ("51864288", false), ("51864288", false), ("7760269", true), ("811900002", true), ("11129885", false), ("11566224", true), ("15180311", true), };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);
        }
        [TestMethod]
        public async Task GrandPuntoWithVinIsolationsAndPaddings()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=22&MOD_COD=183&VEHICLE_NO=01253242&MONTH=3&COMM_MODEL=FPUN&SBMK=F&COUNTRY=99&DRIVE=S&MVS=199.434.1.0&YEAR=2007&COLOUR=535&MAKE=F&CHASSIS_NO=ZFA19900000184331&DAY_OF_MONTH=24&CAT_COD=2Y&GRP_COD=705&SGS_COD=70522/00&ENGINE_NO=1121922&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            var req = GetRequestDataFromEperUrl(url);
            Console.WriteLine(GetEperUrlFromKeys(req));
//            await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51814148", true), ("51828458", false), ("51757232", false), ("51757232", false), ("51814149", false), ("51861111", false), ("51869555", true), ("7799951", true), ("46804433", true), ("51776319", true), ("51776387", true), ("51779613", true), ("51834786", false) };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);
        }
        //http://127.0.0.1:7080/navi?SGRP_COD=1&COMM_MODEL=IDEA&SBMK=F&COUNTRY=99&DRIVE=S&MVS=235712301000&MAKE=F&CAT_COD=4D&GRP_COD=706&SGS_COD=70601/04&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0
        [TestMethod]
        public async Task IdeaWithMvsFrontSeats()
        {
            var url = "http://127.0.0.1:7080/navi?SGRP_COD=1&COMM_MODEL=IDEA&SBMK=F&COUNTRY=99&DRIVE=S&MVS=235712301000&MAKE=F&CAT_COD=4D&GRP_COD=706&SGS_COD=70601/04&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            var req = GetRequestDataFromEperUrl(url);
            Console.WriteLine(GetEperUrlFromKeys(req));
//                        await GetExpectedResultsFromEperUrl(url);
            var expectedParts = new List<(string, bool)> { ("51716895", true), ("46846136", true), ("46846137", true), ("46829749", true), ("184752680", true), ("184089780", true), ("15903407", true) };
            var r = GetDrawingViewModel("3", req.MakeCode, req.SubMakeCode, req.ModelCode, req.CatalogueCode, req.GroupCode, req.SubGroupCode, req.SubSubGroupCode, req.DrawingNumber, "SubSubGroup", req.VIN, req.MVS);
            openPERAssertions.PartsListMatch(expectedParts, r.TableData.Parts);
        }
        private TableViewModel GetPartsTableFromUrl(string url)
        {
            var parts = url.Split("/");
            var uri = new UriBuilder(url);
            var q = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var mvs = q.Get("MVS");
            if (mvs == null) mvs = "";
            var vin = q.Get("VIN");
            if (vin == null) vin = "";
            var r = GetDrawingViewModel("3", parts[6], parts[7], parts[8], parts[9], int.Parse(parts[10]), int.Parse(parts[11]), int.Parse(parts[12]), int.Parse(parts[13]), "SubSubGroup",  vin, mvs);
            return r.TableData;
        }
        private static async Task GetExpectedResultsFromEperUrl(string url)
        {
            HttpClient c = new HttpClient();
            string page = await c.GetStringAsync(url);
            if (!page.StartsWith("<html")) page = "<html>" + page;
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            Console.Write("var expectedParts = new List<(string, bool)> {");
            foreach (var row in doc.DocumentNode.SelectNodes("//table[@id='partstable']/tbody/tr"))
            {
                if (row.Attributes["partcompatible"] != null)
                {
                    var compatible = row.Attributes["partcompatible"].Value;
                    var cells = row.SelectNodes("td");
                    var partNumber = cells[2].InnerText.Trim();
                    Console.Write($"(\"{partNumber}\", {compatible}),");

                }
            }
            Console.WriteLine("};");
        }
        private KeysModel GetRequestDataFromEperUrl(string url)
        {
            var rc = new KeysModel();
            var uri = new UriBuilder(url);
            var q = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var mvs = q.Get("MVS");
            rc.MVS = (mvs == null) ? "" : mvs;
            var vin = q.Get("CHASSIS_NO");
            rc.VIN = (vin == null) ? "" : vin;
            rc.MakeCode = q.Get("MAKE");
            rc.SubMakeCode = q.Get("SBMK");
            rc.ModelCode = q.Get("COMM_MODEL");
            rc.CatalogueCode = q.Get("CAT_COD");
            rc.GroupCode = int.Parse(q.Get("GRP_COD"));
            rc.SubGroupCode = int.Parse(q.Get("SGRP_COD"));
            var sgsCode = q.Get("SGS_COD");
            sgsCode = sgsCode.Substring(sgsCode.IndexOfAny(new char[] {':', '/', '-'})+1);
            rc.SubSubGroupCode = int.Parse(sgsCode);
            var drawNum = q.Get("DRW_NUM");
            if (drawNum == null) { rc.DrawingNumber = 0; }
            else
            {
                rc.DrawingNumber = int.Parse(drawNum.Substring(0, 1))-1;
            }
            return rc;

        }
        private string GetEperUrlFromKeys(KeysModel keys)
        {
            return $"https://localhost:44329/en/Drawings/Detail/{keys.MakeCode}/{keys.SubMakeCode}/{keys.ModelCode}/{keys.CatalogueCode}/{keys.GroupCode}/{keys.SubGroupCode}/{keys.SubSubGroupCode}/{keys.DrawingNumber}/SubSubGroup?MVS={keys.MVS}&VIN={keys.VIN}";
        }
        private  async Task<string> GenerateePerUrl(string url)
        {
            //http://127.0.0.1:7080/navi?SGRP_COD=14&VEHICLE_NO=00040582&MONTH=12&COMM_MODEL=BAR&SBMK=F&COUNTRY=99&DRIVE=S&MVS=183.829.0.0&YEAR=1998&COLOUR=402&MAKE=F&CHASSIS_NO=ZFA18300000040583&DAY_OF_MONTH=15&CAT_COD=PK&GRP_COD=555&SGS_COD=55514/00&ENGINE_NO=0296507&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0
            //http://127.0.0.1:7080/navi?SGRP_COD=14&VEHICLE_NO=00040582&MONTH=12&COMM_MODEL=BAR&SBMK=F&COUNTRY=99&DRIVE=S&MVS=183.829.0.0&YEAR=1998&COLOUR=402&MAKE=F&CHASSIS_NO=ZFA18300000040583&DAY_OF_MONTH=15&CAT_COD=PK&GRP_COD=555&SGS_COD=55514/00&ENGINE_NO=0296507&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0


            //http://127.0.0.1:7080/navi?SGRP_COD=0&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10200-010&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0
            //http://127.0.0.1:7080/navi?SGRP_COD=0&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10200-10&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0
            // TODO
            // Drawings with full VIN
            // Drawings with only MVS
            // Drawings with neither
            // Drawings that aren't the first one
            //http://127.0.0.1:7080/navi?SGRP_COD=1&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10201-020&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0
            //vs
            //http://127.0.0.1:7080/navi?SGRP_COD=1&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10201-020&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&DRW_NUM=1%7C0
            //http://127.0.0.1:7080/navi?SGRP_COD=1&MOD_COD=319&COMM_MODEL=GIN&SBMK=F&COUNTRY=99&DRIVE=S&MAKE=F&CAT_COD=33&GRP_COD=102&SGS_COD=10201-020&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&DRW_NUM=2%7C0
            var parts = url.Split("/");
            var uri = new UriBuilder(url);
            var q = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var mvs = q.Get("MVS");
            if (mvs == null) mvs = "";
            var vin = q.Get("VIN");
            if (vin == null) vin = "";
            string rc;
            var grpCode = int.Parse(parts[10]).ToString("000");
            var subGrpCode = int.Parse(parts[11]).ToString("00");
            var subSubGrpCode = int.Parse(parts[12]).ToString("00");
            if (vin !="")
            {
                var x = _rep.FindMatchesForMvsAndVin("3", mvs, vin);
                var result = x[0];
                rc = $"http://127.0.0.1:7080/navi?SGRP_COD={parts[11]}&VEHICLE_NO={result.Organization}&MONTH={result.BuildDate.Substring(4, 2)}&COMM_MODEL={parts[8]}&SBMK={parts[7]}&COUNTRY=99&DRIVE=S&MVS={mvs}&YEAR={result.BuildDate.Substring(0, 4)}&COLOUR={result.InteriorColourCode}&MAKE={parts[6]}&CHASSIS_NO={result.Vin}&DAY_OF_MONTH={result.BuildDate.Substring(6, 2)}&CAT_COD={parts[9]}&GRP_COD={grpCode}&SGS_COD={grpCode}{subGrpCode}/{subSubGrpCode}&ENGINE_NO={result.Motor}&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            }
            else
            {
                var modCode = _rep.GetModCodeForCatalogue(parts[9]);
                subSubGrpCode = int.Parse(parts[12]).ToString("000");
                rc = $"http://127.0.0.1:7080/navi?SGRP_COD=0&MOD_COD={modCode}&COMM_MODEL={parts[8]}&SBMK={parts[7]}&COUNTRY=99&DRIVE=S&MAKE={parts[6]}&CAT_COD={parts[9]}&GRP_COD={grpCode}&SGS_COD={grpCode}{subGrpCode}-{subSubGrpCode}&LANGUAGE=3&ALL_FIG=0&RMODE=DEFAULT&KEY=PARTDRAWDATA&EPER_CAT=SP&GUI_LANG=3&ALL_LIST_PART=0&PRINT_MODE=0&PREVIOUS_KEY=SUBGROUP_7&SB_CODE=-1&WINDOW_ID=1&ALL_LIST_PART=0";
            }
            HttpClient c = new HttpClient();
            string page = await c.GetStringAsync(rc);
            if (!page.StartsWith("<html")) page = "<html>" + page;
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            foreach (var row in doc.DocumentNode.SelectNodes("//table[@id='partstable']/tbody/tr"))
            {
                if (row.Attributes["partcompatible"] != null)
                {
                    var compatible = row.Attributes["partcompatible"].Value;
                    var cells = row.SelectNodes("td");
                    var partNumber = cells[2].InnerText.Trim();
                    Console.Write($"(\"{partNumber}\", {compatible}),");

                }
            }
            return rc;
        }
    }
}