using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corathing.Organizer.Database.Model.Identity;

public class IdentityUserEntity : IdentityUser
{
    // --------------------------------------------------------------------
    // 테이블에 정의되는 속성들
    // --------------------------------------------------------------------
    public string? Name { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // nullable로 변경된 보안 관련 속성들
    public DateTime? PasswordUpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }  // nullable로 변경
    public int FailedLoginAttempts { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }

    // --------------------------------------------------------------------
    // 런타임에 참조되거나 매핑되는 속성들
    // --------------------------------------------------------------------
    public virtual ICollection<IdentityUserTokenEntity> UserTokens { get; set; } = new List<IdentityUserTokenEntity>();

    public static void BuildSqlLite(EntityTypeBuilder<IdentityUserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
               .HasMaxLength(100)
               .IsRequired();
        builder.Property(u => u.LastLoginIp)
               .HasMaxLength(50)
               .IsRequired(false);
        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("DATETIME('now')")
               .ValueGeneratedOnAdd();
        builder.Property(u => u.UpdatedAt)
               .HasDefaultValueSql("DATETIME('now')")
               .ValueGeneratedOnAddOrUpdate();

        builder.HasMany(u => u.UserTokens)
               .WithOne(ut => ut.User)
               .HasForeignKey(ut => ut.UserId)
               .IsRequired();
    }
}
