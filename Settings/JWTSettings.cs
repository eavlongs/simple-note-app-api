namespace simple_note_app_api.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int ExpirationMinutes { get; set; } = 120; // 2 hour
        public int RefreshTokenExpirationInMinutes { get; set; } = 20160; // 14 days
    }
}
