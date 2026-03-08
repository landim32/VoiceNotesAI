using AutoMapper;
using DevNote.DTOs;
using DevNote.Mapping;
using DevNote.Models;

namespace DevNote.Tests.Mappers;

public class NoteProfileTests
{
    private readonly IMapper _mapper;

    public NoteProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<NoteProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_Note_To_NoteInfo()
    {
        var note = new Note
        {
            Id = 1,
            Title = "Test",
            Description = "Desc",
            Category = "Work",
            AudioFilePath = "/path/audio.mp3",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2)
        };

        var info = _mapper.Map<NoteInfo>(note);

        Assert.Equal(note.Id, info.Id);
        Assert.Equal(note.Title, info.Title);
        Assert.Equal(note.Description, info.Description);
        Assert.Equal(note.Category, info.Category);
        Assert.Equal(note.AudioFilePath, info.AudioFilePath);
        Assert.Equal(note.CreatedAt, info.CreatedAt);
        Assert.Equal(note.UpdatedAt, info.UpdatedAt);
    }

    [Fact]
    public void Should_Map_NoteInfo_To_Note_Ignoring_Timestamps()
    {
        var info = new NoteInfo
        {
            Id = 1,
            Title = "Test",
            Description = "Desc",
            Category = "Work",
            AudioFilePath = "/path/audio.mp3",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 1, 2)
        };

        var note = _mapper.Map<Note>(info);

        Assert.Equal(info.Id, note.Id);
        Assert.Equal(info.Title, note.Title);
        Assert.Equal(info.Description, note.Description);
        Assert.Equal(info.Category, note.Category);
        Assert.Equal(info.AudioFilePath, note.AudioFilePath);
        Assert.NotEqual(info.CreatedAt, note.CreatedAt); // Ignored, uses default
        Assert.NotEqual(info.UpdatedAt, note.UpdatedAt); // Ignored, uses default
    }
}
