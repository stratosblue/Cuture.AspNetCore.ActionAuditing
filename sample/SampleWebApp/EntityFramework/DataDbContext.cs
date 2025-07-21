using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SampleWebApp.EntityFramework;

public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    public virtual DbSet<UserPermission> UserPermissions { get; set; }
}

[Index(nameof(Uid))]
public record class UserPermission([property: Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] long Id, long Uid, string Permission);
