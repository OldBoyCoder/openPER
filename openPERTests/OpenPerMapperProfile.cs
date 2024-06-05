using AutoMapper;
using openPER.ViewModels;
using openPERModels;

namespace openPERTests
{
    public class OpenPerMapperProfile : Profile
    {
        public OpenPerMapperProfile()
        {
            CreateMap<MakeModel, MakeViewModel>();
            CreateMap<ModelModel, ModelViewModel>();
            CreateMap<CatalogueModel, CatalogueViewModel>();
            CreateMap<GroupModel, GroupViewModel>();
            CreateMap<SubGroupModel, SubGroupViewModel>();
            CreateMap<SubSubGroupModel, SubSubGroupViewModel>();
            CreateMap<DrawingKeyModel, DrawingKeyViewModel>();
            CreateMap<TablePartModel, PartViewModel>();
            CreateMap<TableModel, TableViewModel>();
            CreateMap<BreadcrumbModel, BreadcrumbViewModel>();
            CreateMap<GroupImageMapEntryModel, GroupImageMapEntryViewModel>();
            CreateMap<SubGroupImageMapEntryModel, SubGroupImageMapEntryViewModel>();
            CreateMap<ActivationModel, ActivationViewModel>();
            CreateMap<VariationModel, VariationViewModel>();
            CreateMap<ModificationModel, ModificationViewModel>();
            CreateMap<VinSearchResultModel, VinSearchResultViewModel>();
            CreateMap<MvsDataModel, MvsDataViewModel>();
            CreateMap<PatternModel, PatternViewModel>();
            CreateMap<CatalogueVariantsModel, CatalogueVariantsViewModel>();
            CreateMap<ColourModel, ColourViewModel>();
            CreateMap<PartHotspotModel, PartHotspotViewModel>();
            CreateMap<PartPriceModel, PartPriceViewModel>();
            CreateMap<ModifiedDrawingModel, ModifiedDrawingViewModel>();

        }
    }
}