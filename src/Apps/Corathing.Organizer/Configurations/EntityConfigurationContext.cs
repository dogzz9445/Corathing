using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Corathing.Organizer.Configurations;

public enum DatabaseConnectionType
{
    Default,
    Sqlite,
    MySql
}

public class EntityConfigurationOption
{
    public const string Section = "WidgetSystem:Database";

    /// <summary>
    /// 선택된 DB 연결 타입
    /// </summary>
    public DatabaseConnectionType? DatabaseType { get; set; }

    /// <summary>
    /// Sqlite 연결 정보를 담은 문자열
    /// </summary>
    public string? SqliteConnectionString { get; set; }

    public string? SqliteDatabaseFile { get; set; }

    /// <summary>
    /// Mysql 연결 정보를 담은 문자열
    /// </summary>
    public string? MySqlConnectionString { get; set; }

    /// <summary>
    /// Mysql 문자 인코딩 설정
    /// </summary>
    public string? MySqlCharSet { get; set; }

    /// <summary>
    /// DatabaseOptions 생성자
    /// 기본 값들을 할당
    /// </summary>
    public EntityConfigurationOption()
    {
        SqliteDatabaseFile = "database.db";
        DatabaseType = DatabaseConnectionType.Sqlite;
        SqliteConnectionString = $"Data Source={SqliteDatabaseFile};Version=3;";
        MySqlConnectionString = $"server=<server>;port=<port>;database=<database>;uid=<user>;password=<password>";
        MySqlCharSet = "utf8mb4";
    }

    /// <summary>
    /// OptionsBuilder를 옵션에 따라 설정합니다.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <returns></returns>
    public DbContextOptionsBuilder ConfigureOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
    {
        return DatabaseType switch
        {
            //DatabaseConnectionType.MySql
            //    => optionsBuilder.UseMySql(MySqlConnectionString, ServerVersion.AutoDetect(MySqlConnectionString),
            //                       builder => builder.EnableRetryOnFailure(10)),
            DatabaseConnectionType.Sqlite
                => optionsBuilder.UseSqlite(SqliteConnectionString),
            _ or DatabaseConnectionType.Default
                => optionsBuilder.UseSqlite(SqliteConnectionString),
        };
    }
}

public record EntitySettings(string Id, string? Value);

public class EntityConfigurationSource(
        EntityConfigurationOption? options) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new EntityConfigurationProvider(options);
}
public class EntityConfigurationProvider(EntityConfigurationOption? options)
    : ConfigurationProvider
{
    public override void Load()
    {
        using var dbContext = new EntityConfigurationContext(options);

        dbContext.Database.EnsureCreated();

        Data = dbContext.Settings.Any()
            ? dbContext.Settings.ToDictionary(c => c.Id, c => c.Value, StringComparer.OrdinalIgnoreCase)
            : CreateAndSaveDefaultValues(dbContext);
    }

    static IDictionary<string, string?> CreateAndSaveDefaultValues(
        EntityConfigurationContext context)
    {
        var settings = new Dictionary<string, string?>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["WidgetOptions:IsInitialized"] = "false",
        };

        context.Settings.AddRange(
            settings.Select(kvp => new EntitySettings(kvp.Key, kvp.Value))
                    .ToArray());

        context.SaveChanges();

        return settings;
    }
}

public class EntityConfigurationContext(EntityConfigurationOption? options = null) : DbContext
{
    public DbSet<EntitySettings> Settings => Set<EntitySettings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        options.ConfigureOptionsBuilder(optionsBuilder);
    }
}

public static class EntityConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEntityConfiguration(
        this IConfigurationBuilder builder)
    {
        var tempConfig = builder.Build();
        var options = new EntityConfigurationOption();
        tempConfig.GetSection(EntityConfigurationOption.Section).Bind(options);

        if (options == null)
            return builder;

        return builder.Add(new EntityConfigurationSource(options));
    }
}
