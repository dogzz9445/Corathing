using Corathing.Organizer.Database.Model.Identity;
using Corathing.Organizer.Identity.Dto;
using Corathing.Organizer.Server.Application.Identity.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Corathing.Organizer.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<IdentityUserEntity> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger _logger;

    public AuthController(
        IAuthService authService,
        IJwtTokenService jwtTokenService,
        UserManager<IdentityUserEntity> userManager,
        ILogger logger)
    {
        _authService = authService;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// 사용자 로그인
    /// </summary>
    /// <param name="request">로그인 요청 (Username 또는 Email, Password)</param>
    /// <returns>로그인 응답 (토큰 정보)</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status423Locked)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Email 또는 Username으로 사용자 찾기
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                _logger.Warning("존재하지 않는 사용자: {Username}", request.Username);
                return BadRequest(new { message = "Invalid credentials" });
            }

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            _logger.Information("로그인 성공: {Username}", request.Username);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "로그인 처리 중 오류 발생: {Username}", request.Username);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// 토큰 갱신
    /// </summary>
    /// <param name="request">리프레시 토큰 요청</param>
    /// <returns>새로운 토큰 정보</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var response = await _authService.RefreshTokenAsync(request);
            if (response == null)
            {
                _logger.Warning("토큰 갱신 실패: 유효하지 않은 리프레시 토큰");
                return BadRequest(new { message = "Invalid refresh token" });
            }

            _logger.Information("토큰 갱신 성공: {UserId}", response.UserId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 갱신 중 오류 발생");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// 토큰 유효성 검증
    /// </summary>
    /// <returns>토큰 유효성 결과</returns>
    [HttpGet("validate")]
    [Authorize]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Authorization header is required" });
            }

            var token = authHeader.Substring(7);
            var isValid = await _authService.ValidateTokenAsync(token);

            if (isValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                return Ok(new
                {
                    isValid = true,
                    userId = userId,
                    username = username,
                    message = "Token is valid"
                });
            }

            return BadRequest(new { isValid = false, message = "Token is invalid" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 검증 중 오류 발생");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// 로그아웃
    /// </summary>
    /// <returns>로그아웃 결과</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _authService.LogoutAsync();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.Information("로그아웃 완료: {UserId}", userId);

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "로그아웃 처리 중 오류 발생");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// 현재 사용자 정보 조회
    /// </summary>
    /// <returns>사용자 정보</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User not found in token" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new
            {
                userId = user.Id,
                username = user.UserName,
                email = user.Email,
                roles = roles
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "사용자 정보 조회 중 오류 발생");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// 토큰 만료 시간 확인
    /// </summary>
    /// <returns>토큰 만료 정보</returns>
    [HttpGet("token-info")]
    [Authorize]
    public IActionResult GetTokenInfo()
    {
        try
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Authorization header is required" });
            }

            var token = authHeader.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return BadRequest(new { message = "Invalid token format" });
            }

            var jwtToken = handler.ReadJwtToken(token);
            var expiresAt = jwtToken.ValidTo;
            var issuedAt = jwtToken.ValidFrom;
            var timeRemaining = expiresAt - DateTime.UtcNow;

            return Ok(new
            {
                issuedAt = issuedAt,
                expiresAt = expiresAt,
                timeRemaining = timeRemaining.TotalMinutes,
                isExpired = expiresAt <= DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 정보 조회 중 오류 발생");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

}
