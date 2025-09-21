using FluentMigrator;

namespace simple_note_app_api.Migrations
{
    [Migration(1)]
    public class CreateUserTable : Migration
    {
        public static string TableName = "Users";
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsString(Constants.MAX_USERNAME_LENGTH).NotNullable().Unique()
                .WithColumn("Password").AsString(Constants.MAX_PASSWORD_HASH_LENGTH).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
