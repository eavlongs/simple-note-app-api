using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using simple_note_app_api.Dto;
using simple_note_app_api.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace simple_note_app_api.Controller
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var _user = HttpContext.User;
            var user = new Token(_user);
            var userId = int.TryParse(user.Id, out var id) ? id : 0;

            var notes = await _noteService.GetAllNotesByUserId(userId);

            return ResponseBuilder.Success(new { notes });
        }

        [Authorize]
        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            var _user = HttpContext.User;
            var user = new Token(_user);
            var userId = int.TryParse(user.Id, out var id) ? id : 0;

            var note = await _noteService.GetNoteById(noteId, userId);

            if (note == null)
            {
                return ResponseBuilder.NotFound(new { });
            }

            return ResponseBuilder.Success(new { note });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
        {
            var _user = HttpContext.User;
            var user = new Token(_user);
            var userId = int.TryParse(user.Id, out var id) ? id : 0;

            try
            {
                var note = await _noteService.CreateNote(userId, request.Title, request.Content);
                return ResponseBuilder.Success(new { note });
            } catch (Exception ex)
            {
                return ResponseBuilder.InternalServerError(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] UpdateNoteRequest request)
        {
            var _user = HttpContext.User;
            var user = new Token(_user);
            var userId = int.TryParse(user.Id, out var id) ? id : 0;

            try
            {
                var note = await _noteService.UpdateNote(noteId, userId, request.Title, request.Content);
                return ResponseBuilder.Success(new { note });
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var _user = HttpContext.User;
            var user = new Token(_user);
            var userId = int.TryParse(user.Id, out var id) ? id : 0;

            try
            {
                await _noteService.DeleteNote(noteId, userId);
                return ResponseBuilder.Success(new { });
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BadRequest(new { message = ex.Message });
            }
        }
    }
}
