using Azure.Core;
using simple_note_app_api.Repository;

namespace simple_note_app_api.Services
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> RegisterUser(string username, string plainPassword);
        Task<(string AccessToken, string RefreshToken)> AuthenticateUser(string username, string plainPassword);
        Task<(string AccessToken, string RefreshToken)> RefreshSession(string token);
        string HashPassword(string plainPassword);
        bool VerifyPassword(string plainPassword, string hashedPassword);
        string GetTokenFromAuthorizationHeader(HttpRequest request);
    }

    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private const int BCRYPT_ROUNDS = 12;

        public AuthService(IUserRepository userRepository, IJWTService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<(string AccessToken, string RefreshToken)> RegisterUser(string username, string plainPassword)
        {
            var hashedPassword = HashPassword(plainPassword);
            var user = await _userRepository.CreateUser(username, hashedPassword);
            
            var accessToken = _jwtService.GenerateToken(user, TokenType.Access);
            var refreshToken = _jwtService.GenerateToken(user, TokenType.Refresh);

            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> AuthenticateUser(string username, string plainPassword)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            VerifyPassword(plainPassword, user.Password);
            
            var accessToken = _jwtService.GenerateToken(user, TokenType.Access);
            var refreshToken = _jwtService.GenerateToken(user, TokenType.Refresh);
            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshSession(string token)
        {
            var t = _jwtService.ValidateToken(token, TokenType.Refresh);
            if (t == null)
            {
                throw new Exception("Invalid token");
            }

            var userId = int.TryParse(t.Id, out var id) ? id : 0;
            if (userId == 0)
            {
                throw new Exception("Invalid user ID");
            }

            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var newAccessToken = _jwtService.GenerateToken(user, TokenType.Access);
            var newRefreshToken = _jwtService.GenerateToken(user, TokenType.Refresh);

            return (newAccessToken, newRefreshToken);
        }
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, BCRYPT_ROUNDS);
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
           bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
           return isValid;
        }

        public string GetTokenFromAuthorizationHeader(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                throw new Exception("Authorization header is required");
            }

            var token = authHeader.ToString();

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7);
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token is required");
            }

            return token;
        }
    }
}
