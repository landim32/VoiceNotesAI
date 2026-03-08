using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;

namespace DevNotes.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryInfo>();
        CreateMap<CategoryInfo, Category>();
    }
}
