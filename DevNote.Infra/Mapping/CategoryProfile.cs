using AutoMapper;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryInfo>();
        CreateMap<CategoryInfo, Category>();
    }
}
