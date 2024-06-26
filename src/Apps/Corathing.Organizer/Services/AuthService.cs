using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Helpers;
using Corathing.Organizer.Configurations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Corathing.Organizer.Services;

public class AuthContext(EntityConfigurationOption? options = null) : DbContext
{
    public DbSet<EntitySettings> Settings => Set<EntitySettings>();
    public static readonly new string RNG_AUTH = GenerateRandomString();

    public static string GenerateRandomString()
    {
        byte[] nonceBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(nonceBytes);
        }
        return Convert.ToBase64String(nonceBytes);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var a = options.SqliteConnectionString;
        var conn = new SqliteConnection(@"Data Source=yourSQLite.db;");
        conn.Open();

        var command = conn.CreateCommand();
        command.CommandText = "PRAGMA key = password;";
        command.ExecuteNonQuery();

        optionsBuilder.UseSqlite(conn);

        options.ConfigureOptionsBuilder(optionsBuilder);
    }
}

public class AuthService : IAuthService
{
    private readonly ISecretService _secretService;

    public string SecretKey { get; set; }
    public bool UseAuthService { get; set; } = true;
    public bool IsAuthorized { get; set; } = false;

    public AuthService(ISecretService secretService)
    {
    }

    public async Task<bool> RequestAuthentication(Task<AuthenticationInfo?> request)
    {
        var authenticationInfo = await request;


        return true;
    }

    private AuthenticationResponse Authentication(AuthenticationInfo? authenticationInfo)
    {
        var response = new AuthenticationResponse();
        if (authenticationInfo == null)
        {
            response.Code = AuthenticationCode.Error;
            return response;
        }
        string encrytedPassword = CryptoHelper.SHA256Hash(password: authenticationInfo.password);

        var builder = new ConfigurationBuilder()
            .AddJsonFile("data")
            .AddEntityConfiguration();
        var tempConfig = builder.Build();
        var entityConfiguration = new EntityConfigurationOption();
        tempConfig.GetSection(EntityConfigurationOption.Section).Bind(entityConfiguration);
        var dbContext = new EntityConfigurationContext(entityConfiguration);
        dbContext.Database.EnsureCreated();

        if (File.Exists(entityConfiguration.SqliteDatabaseFile))
        {
            //var authenticationInfo = await requestAuthentication;
        }
        return response;
    }
}
