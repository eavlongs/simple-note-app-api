using Microsoft.AspNetCore.Mvc;
using simple_note_app_api.Models;
using simple_note_app_api.Services;
using simple_note_app_api.Dto;
using Microsoft.Data.SqlClient;
using Azure.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace simple_note_app_api.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJWTService _jwtService;

        public AuthController(IAuthService authService, IJWTService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var tokens = await _authService.RegisterUser(request.Username, request.Password);
                return ResponseBuilder.Success(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(SqlException))
                {
                    var sqlEx = (SqlException)ex;
                    if (sqlEx.Number == Constants.DUPLICATE_ENTRY_CODE)
                    {
                        // only username is unique in this case
                        return ResponseBuilder.BadRequest(new { }, "Username is already taken");
                    }
                }
                return ResponseBuilder.BadRequest(new { }, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var tokens = await _authService.AuthenticateUser(request.Username, request.Password);
                return ResponseBuilder.Success(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BadRequest(new { }, ex.Message);
            }
        }

        [HttpGet("verify-access-token")]
        public IActionResult VerifyToken()
        {
            try
            {
                var token = _authService.GetTokenFromAuthorizationHeader(Request);
                var t = _jwtService.ValidateToken(token, TokenType.Access);

                if (t == null)
                {
                    throw new Exception("Invalid token");
                }
                return ResponseBuilder.Success(new {user = t});
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BadRequest(new { }, ex.Message);
            }
        }

        [HttpPost("refresh-session")]
        public async Task<IActionResult> RefreshSession([FromBody] RefreshSessionRequest request)
        {
            try
            {
                var tokens = await _authService.RefreshSession(request.RefreshToken);
                return ResponseBuilder.Success(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BadRequest(new { }, ex.Message);
            }
        }
    }
}
