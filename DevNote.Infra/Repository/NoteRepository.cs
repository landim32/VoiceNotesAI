using AutoMapper;
using DevNote.Context;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Repository;

public class NoteRepository : INoteRepository
{
    private readonly AppDatabase _database;
    private readonly IMapper _mapper;

    public NoteRepository(AppDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<List<NoteInfo>> GetAllAsync()
    {
        var notes = await _database.Connection
            .Table<Note>()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<NoteInfo>>(notes);
    }

    public async Task<NoteInfo?> GetByIdAsync(int id)
    {
        var note = await _database.Connection
            .Table<Note>()
            .Where(n => n.Id == id)
            .FirstOrDefaultAsync();
        return note is null ? null : _mapper.Map<NoteInfo>(note);
    }

    public async Task<int> SaveAsync(NoteInfo noteInfo)
    {
        var note = _mapper.Map<Note>(noteInfo);

        if (note.Id != 0)
        {
            return await _database.Connection.UpdateAsync(note);
        }

        return await _database.Connection.InsertAsync(note);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Note>(id);
    }

    public async Task<List<NoteInfo>> GetByCategoryAsync(string category)
    {
        var notes = await _database.Connection
            .Table<Note>()
            .Where(n => n.Category == category)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<NoteInfo>>(notes);
    }
}
