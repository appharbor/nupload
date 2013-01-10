using System.Data.Entity;

namespace Nupload.Sample.Models
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Painting> Paintings { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, DatabaseMigrationConfiguration>());
			base.OnModelCreating(modelBuilder);
		}
	}
}
