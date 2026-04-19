using System.Security.Cryptography;

using Corathing.Organizer.Database.Data;
using Corathing.Organizer.Database.Model.Identity;
using Corathing.Organizer.Server.API.Hubs;
using Corathing.Organizer.Server.Application.Identity.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using Prometheus;

using Serilog;
using Serilog.Events;

namespace Corathing.Organizer.Server.Application;

public class ServerHost
{
    private static Lazy<ServerHost>? _instance;
    public static ServerHost Instance => (_instance ??= new Lazy<ServerHost>(() => new ServerHost())).Value;

    public WebApplication? App { get; set; }
    public string AppName { get; set; } = "CorathingOrganizerServer";

    public void OnStartup(string[] args)
    {
        StartAsync(args).Wait();
    }

    public async Task StartAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // Add Serilog to the logging pipeline
        builder.Host.UseSerilog();
        builder.Services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);

        // CORS 설정을 appsettings.json에서 가져오기
        var corsConfig = builder.Configuration.GetSection("Cors");
        var allowedOrigins = corsConfig.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        var allowCredentials = corsConfig.GetValue<bool>("AllowCredentials");
        var httpPort = builder.Configuration.GetValue<int>("HttpPort", 9931);
        var httpsPort = builder.Configuration.GetValue<int>("HttpsPort", 9932);

        builder.WebHost.UseUrls($"http://localhost:{httpPort}", $"https://localhost:{httpsPort}");

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(ServerHost).Assembly);

        builder.Services.AddSignalR()
            .AddMessagePackProtocol(options => {
                options.SerializerOptions = MessagePack.MessagePackSerializerOptions.Standard
                    .WithSecurity(MessagePack.MessagePackSecurity.UntrustedData)
                    .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Corathing Organizer API",
                Version = "v1",
                Description = "Corathing Organizer Server API Documentation"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            };

            // JWT Bearer 인증 설정
            c.AddSecurityDefinition("Bearer", securityScheme);
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
            });
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=Database/CorathingData.db"; // SQLite 기본값

        var connectionStringBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
        var dbPath = connectionStringBuilder.DataSource;

        // 데이터베이스 파일의 디렉토리가 없으면 생성
        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
            Log.Information("Created database directory: {Directory}", dbDirectory);
        }

        // 데이터베이스 파일이 없으면 생성됨 (EnsureCreatedAsync가 자동으로 처리)
        if (!File.Exists(dbPath))
        {
            Log.Information("Database file does not exist. Creating new database at: {Path}", dbPath);
        }

        builder.Services.AddDbContext<CorathingOrganizerDatabaseContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        builder.Services.AddIdentity<IdentityUserEntity, IdentityRole<string>>(options =>
            {
                // NOTE:
                // 필요시, Password 정책 강화
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;

                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<CorathingOrganizerDatabaseContext>()
            .AddDefaultTokenProviders();

        var jwtSettings = new JwtSettings();
        builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer((Action<JwtBearerOptions>)(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new RsaSecurityKey(JwtTokenService.CreateRsaKeyFromString(jwtSettings.PublicKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning<string>("OnAuthenticationFailed: {0}", context.Exception.Message);
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log.Warning<string>("OnChallenge: {0}", context.Error);
                        // 401 응답이 발생했을 때 실행되는 부분
                        context.HandleResponse(); // 기본 응답 방지
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            status = 401,
                            message = "You are not authorized"
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            }));

        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IAuthService, AuthService>(); // AuthService 구현체 등록
        builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>(); // JwtTokenService 구현체 등록

        App = builder.Build();

        App.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var scheme = httpReq.Scheme;
                var host = httpReq.Host.Value;

                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = $"{scheme}://{host}", Description = "Current Server" },
                    new OpenApiServer { Url = $"http://localhost:{httpPort}", Description = "HTTP Development" },
                    new OpenApiServer { Url = $"https://localhost:{httpsPort}", Description = "HTTPS Development" },
                };
            });
        });
        App.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Corathing Organizer API V1");
        });

        App.UseCors();
        App.UseHttpsRedirection();
        App.UseRouting();
        App.UseAuthentication();
        App.UseAuthorization();

        App.MapControllers();
        App.MapHub<ServerHub>("/hub");

        App.UseHttpMetrics();
        App.MapMetrics("/metrics");

        //App.Services.GetRequiredService<ServerStateService>().InitializeMetrics();

        using (var scope = App.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<CorathingOrganizerDatabaseContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserEntity>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();


                // 데이터베이스가 존재하지 않으면 삭제 후 재생성
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                // 보류 중인 모든 마이그레이션을 적용하여 데이터베이스 생성
                await context.Database.MigrateAsync();

                await CorathingOrganizerDatabaseContext.SeedDataAsync(context, userManager, roleManager);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while seeding the database.");
            }
        }


        await App.RunAsync();
    }

}
