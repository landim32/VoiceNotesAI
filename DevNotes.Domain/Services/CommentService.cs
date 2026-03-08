using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;
using DevNotes.Repository;
using DevNotes.Services.Interfaces;

namespace DevNotes.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<List<CommentInfo>> GetByNoteIdAsync(int noteId)
    {
        return await _commentRepository.GetByNoteIdAsync(noteId);
    }

    public async Task SaveAsync(CommentInfo commentInfo)
    {
        Comment comment;

        if (commentInfo.Id != 0)
        {
            comment = _mapper.Map<Comment>(commentInfo);
        }
        else
        {
            comment = Comment.Create(commentInfo.NoteId, commentInfo.Content);
        }

        await _commentRepository.SaveAsync(_mapper.Map<CommentInfo>(comment));
    }

    public async Task DeleteAsync(int id)
    {
        await _commentRepository.DeleteAsync(id);
    }

    public async Task DeleteByNoteIdAsync(int noteId)
    {
        await _commentRepository.DeleteByNoteIdAsync(noteId);
    }
}
