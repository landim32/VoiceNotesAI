using AutoMapper;
using DevNote.Context;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly AppDatabase _database;
    private readonly IMapper _mapper;

    public CommentRepository(AppDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<List<CommentInfo>> GetByNoteIdAsync(int noteId)
    {
        var comments = await _database.Connection
            .Table<Comment>()
            .Where(c => c.NoteId == noteId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<CommentInfo>>(comments);
    }

    public async Task<int> SaveAsync(CommentInfo commentInfo)
    {
        var comment = _mapper.Map<Comment>(commentInfo);

        if (comment.Id != 0)
        {
            return await _database.Connection.UpdateAsync(comment);
        }

        return await _database.Connection.InsertAsync(comment);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Comment>(id);
    }

    public async Task<int> DeleteByNoteIdAsync(int noteId)
    {
        return await _database.Connection.ExecuteAsync(
            "DELETE FROM Comment WHERE NoteId = ?", noteId);
    }
}
