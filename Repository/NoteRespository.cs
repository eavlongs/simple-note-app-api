using simple_note_app_api.Models;
using simple_note_app_api.Services;

namespace simple_note_app_api.Repository
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllNotesByUserId(int userId);
        Task<Note> CreateNote(int userId, string title, string content);
        Task<Note> UpdateNote(int noteId, string title, string content);
        Task<bool> DeleteNote(int noteId);
        Task<Note> GetNoteById(int noteId);
    }

    public class NoteRespository: INoteRepository
    {
        private readonly IDBService _dbService;

        public NoteRespository(IDBService dbService)
        {
            _dbService = dbService;
        }

        public async Task<List<Note>> GetAllNotesByUserId(int userId)
        {
            var query = @"
                SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
                FROM Notes
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC
            ";
            var p = new { UserId = userId };
            var notes = await _dbService.GetAll<Note>(query, p);
            return notes;
        }

        public async Task<Note> CreateNote(int userId, string title, string? content)
        {
            var query = @"
                INSERT INTO Notes (UserId, Title, Content) 
                OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.Title, INSERTED.Content, INSERTED.CreatedAt, INSERTED.UpdatedAt
                VALUES (@UserId, @Title, @Content)
            ";
            var p = new { UserId = userId, Title = title, Content = content };
            var note = await _dbService.CreateOrSave<Note>(query, p);
            if (note == null)
            {
                throw new Exception("Failed to create note");
            }
            return note;
        }

        public async Task<Note> UpdateNote(int noteId, string title, string content)
        {
            var query = @"
                UPDATE Notes
                SET Title = @Title, Content = @Content, UpdatedAt = GETUTCDATE()
                OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.Title, INSERTED.Content, INSERTED.CreatedAt, INSERTED.UpdatedAt
                WHERE Id = @Id
            ";
            var p = new { Id = noteId, Title = title, Content = content };
            var note = await _dbService.CreateOrSave<Note>(query, p);
            if (note == null)
            {
                throw new Exception("Failed to update note");
            }
            return note;
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            var query = @"
                DELETE FROM Notes
                WHERE Id = @Id
            ";
            var p = new { Id = noteId };
            var rowsAffected = await _dbService.Execute(query, p);
            return rowsAffected > 0;
        }

        public async Task<Note> GetNoteById(int noteId)
        {
            var query = @"
                SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
                FROM Notes
                WHERE Id = @Id
            ";
            var p = new { Id = noteId };
            var note = await _dbService.GetOne<Note>(query, p);
            return note;
        }
    }
}
