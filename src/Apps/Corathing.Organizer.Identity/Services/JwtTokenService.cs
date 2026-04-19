using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ILogger = Serilog.ILogger;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using Corathing.Organizer.Database.Model.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.Server.Application.Identity.Services;

// JWT Token Service Interface
public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(IdentityUserEntity user);
    string GenerateRefreshTokenAsync(IdentityUserEntity user);
    Task<TokenValidationResult> ValidateAccessTokenAsync(string token);
    Task<TokenValidationResult> ValidateRefreshTokenAsync(string token);
}

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public string? ErrorMessage { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
}

// JWT Configuration Settings
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public int ClockSkewMinutes { get; set; } = 5;
    public string SecretKey { get; set; } = string.Empty; // Refresh Token용 대칭키
    public string PublicKey { get; set; } = string.Empty; // Access Token용 공개키
    public string PrivateKey { get; set; } = string.Empty; // Access Token용 개인키
}

// JWT Token Service Implementation
public class JwtTokenService : IJwtTokenService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger _logger;

    private readonly RsaSecurityKey _privateKey;

    public JwtTokenService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // JWT 설정을 바인딩
        _jwtSettings = new JwtSettings();
        configuration.GetSection("Jwt").Bind(_jwtSettings);

        // 필수 설정 검증
        ValidateJwtSettings();

        // RSA 키 생성
        _privateKey = new RsaSecurityKey(CreateRsaKeyFromString(_jwtSettings.PrivateKey));
    }

    private void ValidateJwtSettings()
    {
        if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is not configured");

        if (_jwtSettings.SecretKey.Length < 32)
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");

        if (string.IsNullOrEmpty(_jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured");

        if (string.IsNullOrEmpty(_jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience is not configured");
    }

    public async Task<string> GenerateAccessTokenAsync(IdentityUserEntity user)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserEntity>>();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("token_type", "access")
        };

        // 사용자 롤 추가
        try
        {
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _logger.Debug("Generated access token for user {UserId} with roles: {Roles}",
                user.Id, string.Join(", ", roles));
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to retrieve roles for user {UserId}", user.Id);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.Debug("Access token generated for user {UserId}, expires at {ExpiryTime}",
            user.Id, tokenDescriptor.Expires);

        return tokenString;
    }

    public string GenerateRefreshTokenAsync(IdentityUserEntity user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("token_type", "refresh")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.Debug("Refresh token generated for user {UserId}, expires at {ExpiryTime}",
            user.Id, tokenDescriptor.Expires);

        return tokenString;
    }

    public Task<TokenValidationResult> ValidateAccessTokenAsync(string token)
    {
        return ValidateTokenAsync(token, "access");
    }

    public Task<TokenValidationResult> ValidateRefreshTokenAsync(string token)
    {
        return ValidateTokenAsync(token, "refresh");
    }

    private Task<TokenValidationResult> ValidateTokenAsync(string token, string expectedType)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters;

            if (expectedType == "access")
            {
                // Access Token은 RSA 키로 검증 (공개키 사용)
                var publicKey = new RsaSecurityKey(_privateKey.Rsa.ExportParameters(false)); // 공개키만 추출

                validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = publicKey,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };
            }
            else
            {
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };
            }

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 토큰 타입 확인
            var tokenType = principal.FindFirst("token_type")?.Value;
            if (tokenType != expectedType)
            {
                _logger.Warning("Invalid token type. Expected: {Expected}, Actual: {Actual}",
                    expectedType, tokenType);

                return Task.FromResult(new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Invalid token type. Expected {expectedType} token."
                });
            }

            _logger.Debug("Token validation successful for user {UserId}, type: {TokenType}",
                userId, expectedType);

            return Task.FromResult(new TokenValidationResult
            {
                IsValid = true,
                UserId = userId,
                Principal = principal
            });
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.Warning("Token expired: {Message}", ex.Message);
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token has expired"
            });
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.Warning("Invalid token signature: {Message}", ex.Message);
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Invalid token signature"
            });
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.Warning("Token validation failed: {Message}", ex.Message);
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error during token validation");
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token validation failed"
            });
        }
    }

    public static RSA CreateRsaKeyFromString(string pemKey)
    {
        // Create new RSA instance
        var rsa = RSA.Create();

        // If private key import fails, try importing as public key
        try
        {
            rsa.ImportFromPem(pemKey);
            return rsa;
        }
        catch (CryptographicException)
        {
            throw new ArgumentException("Invalid RSA key format");
        }
    }
}
