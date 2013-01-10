using System.Data.Entity.Migrations;

namespace Nupload.Sample.Models
{
	public class DatabaseMigrationConfiguration : DbMigrationsConfiguration<DatabaseContext>
	{
		public DatabaseMigrationConfiguration()
		{
			AutomaticMigrationsEnabled = true;
		}
	}
}
