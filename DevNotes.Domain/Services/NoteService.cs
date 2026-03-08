using AutoMapper;
using DevNotes.DTOs;
using DevNotes.Models;
using DevNotes.Repository;
using DevNotes.Services.Interfaces;

namespace DevNotes.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public NoteService(INoteRepository noteRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<List<NoteInfo>> GetAllAsync()
    {
        return await _noteRepository.GetAllAsync();
    }

    public async Task<List<NoteInfo>> GetByCategoryAsync(string category)
    {
        return await _noteRepository.GetByCategoryAsync(category);
    }

    public async Task<NoteInfo?> GetByIdAsync(int id)
    {
        return await _noteRepository.GetByIdAsync(id);
    }

    public async Task<NoteInfo> SaveAsync(NoteInfo noteInfo)
    {
        Note note;

        if (noteInfo.Id != 0)
        {
            var existing = await _noteRepository.GetByIdAsync(noteInfo.Id)
                ?? throw new InvalidOperationException("Nota não encontrada.");
            note = _mapper.Map<Note>(existing);
            note.Update(noteInfo.Title, noteInfo.Description, noteInfo.Category, noteInfo.AudioFilePath);
        }
        else
        {
            note = Note.Create(noteInfo.Title, noteInfo.Description, noteInfo.Category, noteInfo.AudioFilePath);
        }

        note.Validate();
        var dto = _mapper.Map<NoteInfo>(note);
        await _noteRepository.SaveAsync(dto);

        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        await _noteRepository.DeleteAsync(id);
    }

    public async Task<List<string>> GetAllCategoryNamesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => c.Name).ToList();
    }
}
