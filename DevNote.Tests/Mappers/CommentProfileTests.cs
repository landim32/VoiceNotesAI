using AutoMapper;
using DevNote.DTOs;
using DevNote.Mapping;
using DevNote.Models;

namespace DevNote.Tests.Mappers;

public class CommentProfileTests
{
    private readonly IMapper _mapper;

    public CommentProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CommentProfile>());
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_Comment_To_CommentInfo()
    {
        var comment = new Comment
        {
            Id = 1,
            NoteId = 5,
            Content = "Test comment",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var info = _mapper.Map<CommentInfo>(comment);

        Assert.Equal(comment.Id, info.Id);
        Assert.Equal(comment.NoteId, info.NoteId);
        Assert.Equal(comment.Content, info.Content);
        Assert.Equal(comment.CreatedAt, info.CreatedAt);
    }

    [Fact]
    public void Should_Map_CommentInfo_To_Comment_Ignoring_CreatedAt()
    {
        var info = new CommentInfo
        {
            Id = 1,
            NoteId = 5,
            Content = "Test comment",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var comment = _mapper.Map<Comment>(info);

        Assert.Equal(info.Id, comment.Id);
        Assert.Equal(info.NoteId, comment.NoteId);
        Assert.Equal(info.Content, comment.Content);
        Assert.NotEqual(info.CreatedAt, comment.CreatedAt); // Ignored, uses default
    }
}
