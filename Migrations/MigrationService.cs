using FluentMigrator.Runner;

namespace simple_note_app_api.Migrations
{
    public class MigrationService
    {
        private readonly IMigrationRunner _migrationRunner;
        public MigrationService(IMigrationRunner migrationRunner)
        {
            _migrationRunner = migrationRunner;
        }
        public void MigrateDatabase()
        {
            _migrationRunner.MigrateUp();
        }
    }
}
