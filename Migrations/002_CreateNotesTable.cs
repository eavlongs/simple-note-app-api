using FluentMigrator;

namespace simple_note_app_api.Migrations
{
    [Migration(2)]
    public class CreateNotesTable: Migration
    {
        public static string TableName = "Notes";
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Title").AsString(Constants.MAX_TITLE_LENGTH).NotNullable()
                .WithColumn("Content").AsString(Constants.MAX_CONTENT_LENGTH).Nullable()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey(CreateUserTable.TableName, "Id")
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("UpdatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
