using Microsoft.IdentityModel.Tokens;
using simple_note_app_api.Models;
using simple_note_app_api.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace simple_note_app_api.Services
{
    public interface IJWTService
    {
        string GenerateToken(User user, TokenType type);
        Token ValidateToken(string token, TokenType type);
    }
    public class JWTService: IJWTService
    {
        private readonly JwtSettings _jwtSettings;

        public JWTService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(User user, TokenType type)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSecretKey();

            var claims = new List<Claim>
            {
            new("id", user.Id.ToString()),
            new("username", user.Username),
            new("token_type", type.ToString())
            };

            var expirationInMinutes = type == TokenType.Access ? _jwtSettings.ExpirationMinutes : _jwtSettings.RefreshTokenExpirationInMinutes;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Token ValidateToken(string token, TokenType type)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSecretKey();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,                          ValidateAudience = false,                        ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Validate the JWT token against security rules (signature, expiration, issuer, etc.)
                // - Returns ClaimsPrincipal with user identity
                // - Also returns the parsed SecurityToken via 'out' parameter
                // - Throws SecurityTokenException if token is invalid/expired
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var t = new Token(principal);

                if (t.Type.Value != type.Value)
                {
                    throw new Exception("Token type mismatch");
                }

                return t;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }

        public byte[] GetSecretKey()
        {
            return Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        }
    }
    public class TokenType
    {
        private TokenType(string type) { Value = type; }

        public string Value { get; private set; }

        public static TokenType Access { get { return new TokenType("access"); } }
        public static TokenType Refresh { get { return new TokenType("refresh"); } }

        public static TokenType GetObject(string? type)
        {
            var lowerType = type?.ToLower();

            switch(lowerType)
            {
                case "access":
                    return Access;
                case "refresh":
                    return Refresh;
                default:
                    throw new Exception("Invalid token type");
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }

    [JsonConverter(typeof(TokenJsonConverter))]
    public class Token
    {
        private string _id;
        private string _username;
        private TokenType _type;
        private DateTime _issuedAt;
        private DateTime _expiresAt;

        public string Id { get { return _id; } }
        public string Username { get { return _username; } }
        public TokenType Type { get { return _type; } }
        public DateTime IssuedAt { get { return _issuedAt; } }
        public DateTime? ExpirationAt { get { return _expiresAt; } }

        public Token(ClaimsPrincipal principal)
        {
            var id = principal.FindFirst("id")?.Value;
            var username = principal.FindFirst("username")?.Value;
            var type = principal.FindFirst("token_type")?.Value;
            var issuedAt = principal.FindFirst("iat")?.Value;
            var expiresAt = principal.FindFirst("exp")?.Value;

            // if any field is null, throw exception
            if (id == null || username == null || type == null || issuedAt == null || expiresAt == null)
            {
                Console.WriteLine($"Token claims - id: {id}, username: {username}, type: {type}, issuedAt: {issuedAt}, expiresAt: {expiresAt}");
                throw new Exception("Invalid token claims");
            }

            _id = id;
            _username = username;
            _type = TokenType.GetObject(type);
            _issuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(issuedAt)).UtcDateTime;
            _expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt)).UtcDateTime;
        }
    }
    public class TokenJsonConverter : JsonConverter<Token>
    {
        public override void Write(Utf8JsonWriter writer, Token value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("id", int.TryParse(value.Id, out var _id) ? _id : 0);
            writer.WriteString("username", value.Username);
            writer.WriteString("type", value.Type.ToString());
            writer.WriteString("issuedAt", value.IssuedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (value.ExpirationAt.HasValue)
            {
                writer.WriteString("expiresAt", value.ExpirationAt.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }
            else
            {
                writer.WriteNull("expiresAt");
            }

            writer.WriteEndObject();
        }

        public override Token Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // We only need json serialization
            throw new NotImplementedException("Deserialization not implemented");
        }
    }
}
