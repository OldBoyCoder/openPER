using AutoMapper;
using openPER.Models;
using openPER.ViewModels;

namespace openPER.AutoMapper
{
    public class openPerMapperProfile:Profile
    {
        public openPerMapperProfile()
        {
            CreateMap<MakeModel, MakeViewModel>();
            CreateMap<CatalogueModel, CatalogueViewModel>();
        }
    }
}
