using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Organizer.Database.Model.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Corathing.Organizer.Database.Data;

public partial class CorathingOrganizerDatabaseContext : IdentityDbContext<
    IdentityUserEntity,             // DbSet<IdentityUserEntity>        Users
    IdentityRole<string>,           // DbSet<IdentityRole<string>>      Roles
    string,                         // 테이블들의 PK 타입
    IdentityUserClaim<string>,      // DbSet<IdentityUserClaim<string>> UserClaims
    IdentityUserRole<string>,       // DbSet<IdentityUserRole<string>>  UserRoles
    IdentityUserLogin<string>,      // DbSet<IdentityUserLogin<string>> UserLogins
    IdentityRoleClaim<string>,      // DbSet<IdentityRoleClaim<string>> RoleClaims
    IdentityUserTokenEntity         // DbSet<IdentityUserTokenEntity>   UserTokens
    >
{
    public CorathingOrganizerDatabaseContext(DbContextOptions<CorathingOrganizerDatabaseContext> options)
        : base(options)
    {
    }

    // --------------------------------------------------------------------
    // 테이블 목록
    // --------------------------------------------------------------------

    // DbSets for Identity
    // 01. User Table (parent)
    // 02. Role(User) Table (parent)
    // 03. User Claim Table (parent)
    // 04. User and Role Relation Table (parent)
    // 05. User Login Table (parent)
    // 06. Role Claim Table (parent)
    // 07. User Token Table (parent)

    // --------------------------------------------------------------------
    // SQLite 마이그레이션
    // --------------------------------------------------------------------
    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (builder == null)
            return;

        base.OnModelCreating(builder);

        // Identity 관련 테이블 빌드
        builder.Entity<IdentityUserEntity>(builder =>
        {
            builder.ToTable("IDENTITY_USER");
            IdentityUserEntity.BuildSqlLite(builder);
        });
        builder.Entity<IdentityRole<string>>(builder => builder.ToTable("IDENTITY_ROLE"));
        builder.Entity<IdentityUserClaim<string>>(builder => builder.ToTable("IDENTITY_USER_CLAIM"));
        builder.Entity<IdentityUserRole<string>>(builder =>
        {
            builder.ToTable("IDENTITY_USER_ROLE");
        });
        builder.Entity<IdentityUserLogin<string>>(builder =>
        {
            builder.ToTable("IDENTITY_USER_LOGIN");
            builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });
        builder.Entity<IdentityRoleClaim<string>>(builder => builder.ToTable("IDENTITY_ROLE_CLAIM"));
        builder.Entity<IdentityUserTokenEntity>(builder =>
        {
            builder.ToTable("IDENTITY_USER_TOKEN");
            IdentityUserTokenEntity.BuildSqlLite(builder);
        });
    }
}
