using AutoMapper;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Mapping;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteInfo>();
        CreateMap<NoteInfo, Note>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
