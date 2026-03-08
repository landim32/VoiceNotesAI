using AutoMapper;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentInfo>();
        CreateMap<CommentInfo, Comment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
