﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Helpers;
using openPER.ViewModels;
using openPERHelpers;
using openPERModels;
using openPERRepositories.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace openPER.Controllers
{
    public class PartController : Controller
    {
        readonly IRepository _rep;
        private readonly IMapper _mapper;
        public PartController(IRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var p = new PartSearchViewModel();
            return View(p);
        }

        [HttpGet]

        public ActionResult SearchPartByPartNumber(string language, string partNumber)
        {
            var p = new PartSearchViewModel();
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            p.Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language);

            p.Language = language;
            if (string.IsNullOrEmpty(partNumber)) return View("Index", p);
            p.PartNumberSearch = partNumber;

            p.Result = _rep.GetPartDetails(p.PartNumberSearch, language);
            return View("Index", p);
        }

        public IActionResult SearchPartByModelAndName(string language, string partModelName, string partName)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new PartSearchResultsViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language),
                Language = language
            };
            if (string.IsNullOrEmpty(partModelName) || string.IsNullOrEmpty(partName)) return View("SearchResults", model);

            var parts = _rep.GetBasicPartSearch(partModelName, partName, language);

            foreach (var p in parts)
            {
                var v = new PartSearchResultViewModel
                {
                    Description = p.Description,
                    FamilyCode = p.FamilyCode,
                    FamilyDescription = p.FamilyDescription,
                    PartNumber = p.PartNumber,
                    UnitOfSale = p.UnitOfSale,
                    Weight = p.Weight,
                    CatalogueCode = p.CatalogueCode,
                    CatalogueDescription = p.CatalogueDescription,
                    Drawings = p.Drawings
                };
                model.Results.Add(v);

            }
            return View("SearchResults", model);
        }
        public IActionResult SearchPartByCatalogueAndCode(string language, string catalogueCode,string mvs, string vin, string searchText)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var model = new PartSearchResultsViewModel
            {
                Navigation = NavigationHelper.PopulateNavigationModel(this, _mapper, _rep, language),
                Language = language
            };
            if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(catalogueCode)) return View("SearchResults", model);

            var parts = _rep.GetPartSearchForCatalogue(catalogueCode, searchText, language);

            foreach (var p in parts)
            {
                var v = new PartSearchResultViewModel
                {
                    Description = p.Description,
                    FamilyCode = p.FamilyCode,
                    FamilyDescription = p.FamilyDescription,
                    PartNumber = p.PartNumber,
                    UnitOfSale = p.UnitOfSale,
                    Weight = p.Weight,
                    CatalogueCode = p.CatalogueCode,
                    CatalogueDescription = p.CatalogueDescription,
                    Drawings = p.Drawings
                };
                model.Results.Add(v);

            }
            return View("SearchResults", model);
        }

        public FileResult AllParts(string language, string catalogueCode)
        {
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var parts = _rep.GetAllPartsForCatalogue(language, catalogueCode);
            var contents = "Table code\tGroup desc\tSubgroup desc\tPart code\tPart desc\tPart desc extra\tNotes\tDrawing number\tIndex\tPart Patten\tPart mods\tTable pattern\tTable mods\tQuantity\r\n";
            foreach (var p in parts)
            {
                var line = p.TableCode + "\t"
                    + p.GroupDescription + "\t"
                    + p.SubGroupDescription + "\t"
                    + p.PartCode + "\t"
                    + p.CodeDescription + "\t"
                    + p.ExtraDescription + "\t"
                    + p.Notes + "\t"
                    + p.DrawingNumber + "\t"
                    + p.Rif + "\t"
                    + p.PartPattern + "\t"
                    + p.PartModification + "\t"
                    + p.TablePattern + "\t"
                    + p.TableModification + "\t"
                    + p.Quantity + "\r\n";
                contents += line.Replace("\r", "").Replace("\n", "") + "\r\n";

            }
            var b = Encoding.UTF8.GetBytes(contents);
            return File(b, "text/tab-separated-values", $"AllParts_{catalogueCode}_{language}.txt");
        }

        public FileResult AllPartsFiltered(string language, string catalogueCode, string vin, string mvs)
        {
            // Must have VIN and MVS to get a filtered list otherwise performance is dire.
            if (string.IsNullOrEmpty(vin) || string.IsNullOrEmpty(mvs))
                return AllParts(language, catalogueCode);
            //https://localhost:44329/Part/AllPartsFiltered?language=en&catalogueCode=PK&MVS=183.829.0.0&VIN=ZFA18300000040598
            language = LanguageSupport.GetIso639CodeFromString(language);
            ViewData["Language"] = language;
            LanguageSupport.SetCultureBasedOnRoute(language);
            var sinComPattern = _rep.GetSincomPattern(mvs);
            var vmkCodes = _rep.GetVmkDataForCatalogue(catalogueCode, language);
            var vehiclePattern = _rep.GetVehiclePattern(language, vin);
            var vehicleModificationFilters = _rep.GetFiltersForVehicle(language, vin, mvs);

            var parts = _rep.GetAllPartsForCatalogue(language, catalogueCode);
            var contents = "Table code\tGroup desc\tSubgroup desc\tPart code\tPart desc\tPart desc extra\tNotes\tDrawing number\tIndex\tPart Patten\tPart mods\tTable pattern\tTable mods\tQuantity\r\n";
            var RuleCache = new Dictionary<string, bool>();
            foreach (var p in parts)
            {
                var omitPart = false;
                var pattern = p.TablePattern;
                var cacheKey = pattern + "|" + p.TableModification + "|" + p.PartModification;
                if (!string.IsNullOrEmpty(pattern) || !string.IsNullOrEmpty(p.TableModification) || !string.IsNullOrEmpty(p.PartModification))
                {
                    var b = false;
                    if (RuleCache.ContainsKey(cacheKey))
                        b = RuleCache[cacheKey];
                    else
                    {
                        b = PatternMatchHelper.ApplyPatternAndModificationRules(pattern, sinComPattern, vmkCodes, vehiclePattern,_mapper.Map<List<ModificationModel>, List<ModificationViewModel>>(p.Modifications), vehicleModificationFilters);
                        RuleCache[cacheKey] = b;
                    }
                    if (!b) omitPart = true;
                }
                pattern = p.PartPattern;
                cacheKey = pattern + "|" + p.TableModification + "|" + p.PartModification;
                if (!string.IsNullOrEmpty(pattern) || !string.IsNullOrEmpty(p.TableModification) || !string.IsNullOrEmpty(p.PartModification))
                {
                    var b = false;
                    if (RuleCache.ContainsKey(cacheKey))
                        b = RuleCache[cacheKey];
                    else
                    {
                        b = PatternMatchHelper.ApplyPatternAndModificationRules(pattern, sinComPattern, vmkCodes, vehiclePattern, _mapper.Map<List<ModificationModel>, List<ModificationViewModel>>(p.Modifications), vehicleModificationFilters);
                        RuleCache[cacheKey] = b;
                    }
                    if (!b) omitPart = true;
                }

                if (!omitPart)
                {
                    var line = p.TableCode + "\t"
                        + p.GroupDescription + "\t"
                        + p.SubGroupDescription + "\t"
                        + p.PartCode + "\t"
                        + p.CodeDescription + "\t"
                        + p.ExtraDescription + "\t"
                        + p.Notes + "\t"
                        + p.DrawingNumber + "\t"
                        + p.Rif + "\t"
                        + p.PartPattern + "\t"
                        + p.PartModification + "\t"
                        + p.TablePattern + "\t"
                        + p.TableModification + "\t"
                        + p.Quantity;
                    contents += line.Replace("\r", "").Replace("\n", "") + "\r\n";
                }

            }
            var bytes = Encoding.UTF8.GetBytes(contents);
            return File(bytes, "text/tab-separated-values", $"AllParts_{catalogueCode}_{language}_{vin}.txt");

        }

    }
}
