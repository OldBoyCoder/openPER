﻿using openPERModels;
using openPER.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using openPERRepositories.Interfaces;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace openPER.Helpers
{
    public class NavigationHelper
    {

        private static NavigationViewModel InternalPopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep,
            string language, string makeCode, string subMakeCode,
            string modelCode, string catalogueCode, int? groupCode,
            int? subGroupCode, int? subSubGroupCode, int? drawingNumber,
            string scope, string clichePartNumber, int? clicheDrawingNumber, string vin, string mvs)
        {
            var breadcrumb = new BreadcrumbModel
            {
                MakeCode = makeCode,
                SubMakeCode = subMakeCode,
                ModelCode = modelCode,
                CatalogueCode = catalogueCode,
                GroupCode = groupCode,
                SubGroupCode = subGroupCode,
                SubSubGroupCode = subSubGroupCode,
                DrawingNumber = drawingNumber,
                ClichePartNumber = clichePartNumber,
                ClicheDrawingNumber = clicheDrawingNumber,
                Scope = scope
            };
            rep.PopulateBreadcrumbDescriptions(breadcrumb, language);

            var model = new NavigationViewModel
            {
                Breadcrumb = mapper.Map<BreadcrumbModel, BreadcrumbViewModel>(breadcrumb),
                SideMenuItems = new SideMenuItemsViewModel
                {
                    AllMakes = mapper.Map<List<MakeModel>, List<MakeViewModel>>(rep.GetAllMakes())
                },
                Language = language

            };
            if (makeCode != null && subMakeCode != null)
                model.SideMenuItems.AllModels = mapper.Map<List<ModelModel>, List<ModelViewModel>>(rep.GetAllModelsForMake(makeCode, subMakeCode));
            if (makeCode != null && subMakeCode != null && modelCode != null)
                model.SideMenuItems.AllCatalogues = mapper.Map<List<CatalogueModel>, List<CatalogueViewModel>>(rep.GetAllCatalogues(makeCode, subMakeCode, modelCode, language));
            if (catalogueCode != null)
                model.SideMenuItems.AllGroups = mapper.Map<List<GroupModel>, List<GroupViewModel>>(rep.GetGroupsForCatalogue(catalogueCode, language));
            if (catalogueCode != null && groupCode != null)
                model.SideMenuItems.AllSubGroups = mapper.Map<List<SubGroupModel>, List<SubGroupViewModel>>(rep.GetSubGroupsForCatalogueGroup(catalogueCode, groupCode.Value, language));
            model.Filter = new FilterModel
            {
                Mvs = mvs,
                Vin = vin
            };
            if (!string.IsNullOrEmpty(mvs))
            {
                model.Filter = PopulateFilterModel(mapper, rep, language, catalogueCode, mvs, vin);
            }

            model.AllLinks = rep.GetCatalogueHierarchy(language);
            model.UserData = GetUserDataFromCookie(controller);
            if (!string.IsNullOrEmpty(model.Breadcrumb.ModelCode) && !string.IsNullOrEmpty(model.Breadcrumb.CatalogueCode))
                model.Breadcrumb.ForumLink = ForumLinkHelper.GetForumLink(model.Breadcrumb.MakeCode, model.Breadcrumb.ModelCode, model.Breadcrumb.CatalogueCode);
            if (!string.IsNullOrEmpty(model.Breadcrumb.ModelCode) && string.IsNullOrEmpty(model.Breadcrumb.ForumLink))
                model.Breadcrumb.ForumLink = ForumLinkHelper.GetForumLink(model.Breadcrumb.MakeCode, model.Breadcrumb.ModelCode, "");

            return model;
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string clichePartNumber, int clicheDrawingNumber, string vin, string mvs)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, clichePartNumber, clicheDrawingNumber, vin, mvs);
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, string vin, string mvs)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, null, null, null, null, null, null, vin, mvs);
        }
        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, string vin, string mvs)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, null, null, null, null, null, vin, mvs);
        }
        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, string vin, string mvs)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, catalogueCode, null, null, null, null, null, null, null, vin, mvs);
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber, string scope, string vin, string mvs)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, catalogueCode, groupCode, subGroupCode, subSubGroupCode, drawingNumber, scope, null, null, vin, mvs);
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode, string modelCode)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, modelCode, null, null, null, null, null, null, null, null, null, null);
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language, string makeCode, string subMakeCode)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, makeCode, subMakeCode, null, null, null, null, null, null, null, null, null, null, null);
        }

        internal static NavigationViewModel PopulateNavigationModel(Controller controller, IMapper mapper, IRepository rep, string language)
        {
            return InternalPopulateNavigationModel(controller, mapper, rep, language, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }
        public static FilterModel PopulateFilterModel(IMapper mapper, IRepository rep, string language, string catalogueCode, string mvs, string vin)
        {
            var rc = new FilterModel();
            string vehiclePattern = "";
            rc.Vin = vin;
            rc.Mvs = mvs;
            //            var vehicleDetails = rep.FindMatchesForVin(language, vin);
            var vehicleDetails = rep.FindMatchesForMvsAndVin(language, mvs, vin);
            if (vehicleDetails is { Count: > 0 })
            {
                rc.BuildDate = vehicleDetails[0].BuildDate;
                rc.NumberForParts = vehicleDetails[0].Organization;
                rc.Engine = vehicleDetails[0].Motor;
                vehiclePattern = vehicleDetails[0].Caratt;
            }
            // sincom data is just for this specific type in the catalogue
            string sinComPattern = rep.GetSincomPattern(mvs);
            var pattern = sinComPattern;

            rc.DataSource = FilterDataSource.Sincom;
            if (!string.IsNullOrEmpty(vehiclePattern))
            {
                pattern = PatternMatchHelper.ConsolidatePatterns(sinComPattern, vehiclePattern);
                rc.DataSource = FilterDataSource.Vin;
            }
            var potentialOptions = rep.GetMvsDetailsForCatalogue(catalogueCode, language);


            var ourOptions = pattern.Split(new[] { '+' });
            foreach (var ourOption in ourOptions)
            {
                var o = new FilterOptions();
                var key = ourOption;
                o.Present = true;
                if (key.StartsWith("!"))
                {
                    key = key[1..];
                    o.Present = false;
                }
                var opt = potentialOptions.FirstOrDefault(x => x.TypeCodePair.EndsWith(key));
                if (opt != null)
                {
                    o.MultiValue = !string.IsNullOrEmpty(opt.TypeDescription);
                    o.TypeDescription = opt.TypeDescription;
                    o.TypeCode = opt.TypeCode;
                    o.ValueCode = opt.ValueCode;
                    o.ValueDescription = opt.CodeDescription;
                    rc.Options.Add(o);
                }
            }
            return rc;
        }

        internal static UserDataViewModel GetUserDataFromCookie(Controller controller)
        {
            var rc = new UserDataViewModel();
            var ourCookie = controller.Request.Cookies["xf_eper"];
            rc.Vehicles = new List<UserVehicleDataViewModel>();
            controller.ViewData["AdFree"] = false;
            controller.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(7)
            };

            if (ourCookie == null)
            {
                rc.AdvertFree = false;
                rc.UserId = 0;
                return rc;
            }
            try
            {
                var pwd = ConfigurationHelper.config["CookiePassword"];
                if (string.IsNullOrEmpty(pwd)) { throw new Exception("No cookie password found"); }
                var jsonCookie = CookieHelper.DecryptString(ourCookie, pwd);
                dynamic o = JsonConvert.DeserializeObject(jsonCookie);
                rc.AdvertFree = o.a;
                controller.ViewData["AdFree"] = rc.AdvertFree;
                rc.UserId = o.u;
                dynamic v = o["v"];
                foreach (var item in v)
                {
                    var car = new UserVehicleDataViewModel(item.Name, (string)item.Value);
                    rc.Vehicles.Add(car);
                }
                if (rc.UserId != 0)
                {
                    controller.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Private = true,
                        Public = false,
                        MaxAge = TimeSpan.FromSeconds(60)
                    };

                }
            }
            catch (Exception ex)
            {

                rc.CookieError = ex.Message;
            }

            return rc;
        }
        internal static List<SearchEngineViewModel> GetSearchEnginesFromConfig(IConfiguration config)
        {
            var rc = config.GetSection("SearchUrls").Get<List<SearchEngineViewModel>>();
            return rc;
        }
        
    }
}
