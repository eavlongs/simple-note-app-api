using System.ComponentModel.DataAnnotations;

namespace simple_note_app_api.Dto
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(Constants.MAX_USERNAME_LENGTH, MinimumLength = 1)]
        public string Username { get; set; }

        [Required]
        [StringLength(Constants.MAX_PLAIN_PASSWORD_LENGTH, MinimumLength = 1)]
        public string Password { get; set; }
    }

    public class LoginRequest : RegisterRequest
    {
    }

    public class RefreshSessionRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class CreateNoteRequest
    {
        [Required]
        [StringLength(Constants.MAX_TITLE_LENGTH, MinimumLength = 1)]
        public string Title { get; set; }

        [StringLength(Constants.MAX_CONTENT_LENGTH)]
        public string? Content { get; set; }
    }

    public class  UpdateNoteRequest: CreateNoteRequest
    {
        
    }
}
