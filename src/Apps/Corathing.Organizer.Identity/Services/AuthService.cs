using Corathing.Organizer.Database.Model.Identity;
using Corathing.Organizer.Identity.Dto;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using ILogger = Serilog.ILogger;

namespace Corathing.Organizer.Server.Application.Identity.Services;

// Interface
public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync();
    Task<bool> ValidateTokenAsync(string token);
}

// Implementation
public class AuthService : IAuthService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;

    private const string ACCESS_TOKEN_CACHE_KEY = "current_access_token";
    private const string REFRESH_TOKEN_CACHE_KEY = "current_refresh_token";
    private const string TOKEN_EXPIRES_CACHE_KEY = "token_expires_at";

    public AuthService(IServiceProvider serviceProvider,
        IJwtTokenService jwtTokenService,
        ILogger logger,
        IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserEntity>>();
            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<IdentityUserEntity>>();

            _logger.Information("로그인 시도: {Username}", request.Username);

            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                _logger.Warning("존재하지 않는 사용자: {Username}", request.Username);
                return null;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                _logger.Warning("비밀번호 불일치: {Username}", request.Username);
                return null;
            }

            // JWT 토큰 생성
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtTokenService.GenerateRefreshTokenAsync(user);
            var expiresAt = DateTime.UtcNow.AddHours(1); // 액세스 토큰 만료 시간

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.UserName ?? string.Empty
            };

            // 캐시에 토큰 저장
            _cache.Set(ACCESS_TOKEN_CACHE_KEY, accessToken, expiresAt);
            _cache.Set(REFRESH_TOKEN_CACHE_KEY, refreshToken, TimeSpan.FromDays(7));
            _cache.Set(TOKEN_EXPIRES_CACHE_KEY, expiresAt);

            _logger.Information("로그인 성공: {Username}", request.Username);
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "로그인 처리 중 오류 발생: {Username}", request.Username);
            return null;
        }
    }

    public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserEntity>>();
            var validationResult = await _jwtTokenService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (!validationResult.IsValid || validationResult.UserId == null)
            {
                _logger.Warning("유효하지 않은 리프레시 토큰");
                return null;
            }

            var user = await userManager.FindByIdAsync(validationResult.UserId);
            if (user == null)
            {
                _logger.Warning("리프레시 토큰의 사용자를 찾을 수 없음: {UserId}", validationResult.UserId);
                return null;
            }

            // 새로운 토큰 생성
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtTokenService.GenerateRefreshTokenAsync(user);
            var expiresAt = DateTime.UtcNow.AddHours(1);

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.UserName ?? string.Empty
            };

            // 캐시 업데이트
            _cache.Set(ACCESS_TOKEN_CACHE_KEY, accessToken, expiresAt);
            _cache.Set(REFRESH_TOKEN_CACHE_KEY, refreshToken, TimeSpan.FromDays(7));
            _cache.Set(TOKEN_EXPIRES_CACHE_KEY, expiresAt);

            _logger.Information("토큰 갱신 성공: {UserId}", user.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 갱신 중 오류 발생");
            return null;
        }
    }

    //public async Task<string?> GetAccessTokenAsync()
    //{
    //    try
    //    {
    //        // 캐시에서 토큰 확인
    //        if (_cache.TryGetValue(ACCESS_TOKEN_CACHE_KEY, out string? cachedToken) &&
    //            _cache.TryGetValue(TOKEN_EXPIRES_CACHE_KEY, out DateTime expiresAt))
    //        {
    //            // 토큰이 만료되지 않았으면 반환
    //            if (DateTime.UtcNow < expiresAt.AddMinutes(-5)) // 5분 여유를 둠
    //            {
    //                return cachedToken;
    //            }

    //            // 토큰이 만료되었으면 리프레시 시도
    //            if (_cache.TryGetValue(REFRESH_TOKEN_CACHE_KEY, out string? refreshToken))
    //            {
    //                var refreshResult = await RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = refreshToken });
    //                return refreshResult?.AccessToken;
    //            }
    //        }

    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.Error(ex, "액세스 토큰 조회 중 오류 발생");
    //        return null;
    //    }
    //}

    //public async Task<bool> IsAuthenticatedAsync()
    //{
    //    var token = await GetAccessTokenAsync();
    //    if (string.IsNullOrEmpty(token))
    //        return false;

    //    return await ValidateTokenAsync(token);
    //}

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var validationResult = await _jwtTokenService.ValidateAccessTokenAsync(token);
            return validationResult.IsValid;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 검증 중 오류 발생");
            return false;
        }
    }

    public Task LogoutAsync()
    {
        try
        {
            // 캐시에서 토큰 제거
            _cache.Remove(ACCESS_TOKEN_CACHE_KEY);
            _cache.Remove(REFRESH_TOKEN_CACHE_KEY);
            _cache.Remove(TOKEN_EXPIRES_CACHE_KEY);

            _logger.Information("로그아웃 완료");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "로그아웃 처리 중 오류 발생");
        }

        return Task.CompletedTask;
    }
}
