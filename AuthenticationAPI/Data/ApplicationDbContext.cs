using AuthenticationAPI.Data.TableModels;
using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Replace("AspNet", string.Empty)); // Removes "AspNet" prefix from all Identity table names.
                }
            }

            builder.Entity<UserTokenTableRow>(build => 
            {
                build.ToTable("UserTokens"); // Maps UserTokenTableRow to the "UserTokens" table.
                build.Property(t => t.LifetimeExtended).HasDefaultValue(false).IsRequired(); // Sets default value for LifetimeExtended to false.
            });
        }
    }
}
