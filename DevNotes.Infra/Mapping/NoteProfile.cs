using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;

namespace DevNotes.Mapping;

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
