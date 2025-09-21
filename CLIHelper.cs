namespace simple_note_app_api
{
    public class CLIHelper
    {
        private readonly string[] _args;
        public CLIHelper(string[] args)
        {
            _args = args;
        }

        public bool HasArgument(string arg)
        {
            return _args.Contains(arg);
        }

        public MigrationArg GetMigrationArg()
        {
            if (this.HasArgument("--migrate-only"))
            {
                return MigrationArg.MigrateOnly;
            }
            else if (this.HasArgument("--migrate"))
            {
                return MigrationArg.ShouldMigrate;
            }
            else
            {
                return MigrationArg.NoMigration;
            }
        }
    }
    public enum MigrationArg
    {
        NoMigration,
        ShouldMigrate,
        MigrateOnly
    }
}
