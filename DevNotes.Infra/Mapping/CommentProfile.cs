using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;

namespace DevNotes.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentInfo>();
        CreateMap<CommentInfo, Comment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
