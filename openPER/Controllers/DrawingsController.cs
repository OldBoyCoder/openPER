﻿using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using openPER.Interfaces;
using openPER.Models;
using openPER.ViewModels;

namespace openPER.Controllers
{
    public class DrawingsController : Controller
    {
        readonly IVersionedRepository _rep;
        readonly IMapper _mapper;
        public DrawingsController(IVersionedRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }
        // The most specific route, only the drawings for the lowest level are returned
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{SubSubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int subSubGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel();
            model.ReleaseCode = releaseCode;
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubSubGroup(releaseCode, makeCode, modelCode,
                catalogueCode, groupCode, subGroupCode, subSubGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x=>x.ReleaseCode = releaseCode);
            return View(model);
        }
        // A very vague route to a large set of drawings!
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel();
            model.ReleaseCode = releaseCode;
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForCatalogue(releaseCode, makeCode, modelCode,
                catalogueCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            return View(model);
        }
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel();
            model.ReleaseCode = releaseCode;
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForGroup(releaseCode, makeCode, modelCode, catalogueCode, groupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            return View(model);
        }
        [Route("Drawings/{ReleaseCode}/{MakeCode}/{ModelCode}/{CatalogueCode}/{GroupCode}/{SubGroupCode}/{DrawingNumber}")]
        public IActionResult Index(int releaseCode, string makeCode, string modelCode, string catalogueCode, int groupCode, int subGroupCode, int drawingNumber)
        {
            // Standard prologue
            var language = Helpers.LanguageSupport.SetCultureBasedOnCookie(HttpContext);
            ControllerHelpers.ResetReleaseCookie(HttpContext, releaseCode);

            var model = new DrawingsViewModel();
            model.ReleaseCode = releaseCode;
            // We need to get all of the drawing keys for this sub sub group
            List<DrawingKeyModel> drawings = _rep.GetDrawingKeysForSubGroup(releaseCode, makeCode, modelCode, catalogueCode, groupCode, subGroupCode);
            model.Drawings = _mapper.Map<List<DrawingKeyModel>, List<DrawingKeyViewModel>>(drawings);
            model.Drawings.ForEach(x => x.ReleaseCode = releaseCode);
            return View(model);
        }
    }
}
