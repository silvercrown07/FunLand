using System.Collections.Generic;
using AutoMapper;
using FunLand.Data.Models;
using FunLand.Data.ViewModels;

namespace FunLand.Data
{
    public class FunLandMappingProfile : Profile
    {
        public FunLandMappingProfile()
        {
            CreateMap<BlogAttachment, BlogAttachmentView>().ReverseMap();
            CreateMap<Blog, BlogView>()
                .ForMember(view => view.BlogAttachments, option => option.MapFrom(model => Mapper.Map<List<BlogAttachmentView>>(model.BlogAttachments)))
                .ReverseMap();
        }
    }
}