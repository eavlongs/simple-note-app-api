namespace simple_note_app_api
{
    public class Constants
    {
        public const int MAX_TITLE_LENGTH = 255;
        public const int MAX_CONTENT_LENGTH = 5000;

        public const int MAX_USERNAME_LENGTH = 50;
        public const int MAX_PLAIN_PASSWORD_LENGTH = 50;
        public const int MAX_PASSWORD_HASH_LENGTH = 512;

        public const string DB_CONNECTION = "DefaultConnection";
        public const string JWT_SETTINGS = "JwtSettings";

        public const int DUPLICATE_ENTRY_CODE = 2601;
    }
}
