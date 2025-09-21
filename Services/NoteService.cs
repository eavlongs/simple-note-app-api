using simple_note_app_api.Models;
using simple_note_app_api.Repository;

namespace simple_note_app_api.Services
{
    public interface INoteService
    {
        Task<List<Note>> GetAllNotesByUserId(int userId);
        Task<Note> CreateNote(int userId, string title, string content);
        Task<Note> UpdateNote(int noteId, int userId, string title, string content);
        Task<bool> DeleteNote(int noteId, int userId);
        Task<Note> GetNoteById(int noteId, int userId);
    }

    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<List<Note>> GetAllNotesByUserId(int userId)
        {
            return await _noteRepository.GetAllNotesByUserId(userId);
        }

        public async Task<Note> CreateNote(int userId, string title, string content)
        {
            return await _noteRepository.CreateNote(userId, title, content);
        }

        public async Task<Note> UpdateNote(int noteId, int userId, string title, string content)
        {
            var note = await _noteRepository.GetNoteById(noteId);

            if (note == null || note.UserId != userId)
            {
                throw new Exception("Note not found or access denied");
            }

            return await _noteRepository.UpdateNote(noteId, title, content);
        }

        public async Task<bool> DeleteNote(int noteId, int userId)
        {
            var note = await _noteRepository.GetNoteById(noteId);
            if (note == null || note.UserId != userId)
            {
                throw new Exception("Note not found or access denied");
            }

            return await _noteRepository.DeleteNote(noteId);
        }

        public async Task<Note> GetNoteById(int noteId, int userId)
        {
            var note = await _noteRepository.GetNoteById(noteId);

            if (note == null || note.UserId != userId)
            {
                return null;
            }

            return note;
        }
    }
}
