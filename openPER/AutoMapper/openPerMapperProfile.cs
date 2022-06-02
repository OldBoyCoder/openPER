﻿using AutoMapper;
using openPER.Models;
using openPER.ViewModels;

namespace openPER.AutoMapper
{
    public class OpenPerMapperProfile:Profile
    {
        public OpenPerMapperProfile()
        {
            CreateMap<MakeModel, MakeViewModel>();
            CreateMap<ModelModel, ModelViewModel>();
            CreateMap<CatalogueModel, CatalogueViewModel>();
            CreateMap<GroupModel, GroupViewModel>();
            CreateMap<SubGroupModel, SubGroupViewModel>();
            CreateMap<SubSubGroupModel, SubSubGroupViewModel>();
        }
    }
}
