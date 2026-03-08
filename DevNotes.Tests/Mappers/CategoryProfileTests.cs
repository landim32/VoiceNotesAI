using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Mapping;
using DevNotes.Models;

namespace DevNotes.Tests.Mappers;

public class CategoryProfileTests
{
    private readonly IMapper _mapper;

    public CategoryProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_Category_To_CategoryInfo()
    {
        var category = new Category { Id = 1, Name = "Trabalho" };

        var info = _mapper.Map<CategoryInfo>(category);

        Assert.Equal(category.Id, info.Id);
        Assert.Equal(category.Name, info.Name);
    }

    [Fact]
    public void Should_Map_CategoryInfo_To_Category()
    {
        var info = new CategoryInfo { Id = 1, Name = "Trabalho" };

        var category = _mapper.Map<Category>(info);

        Assert.Equal(info.Id, category.Id);
        Assert.Equal(info.Name, category.Name);
    }
}
