using FluentMigrator;


namespace CMS.Migration.Migrations
{
    [Migration(20250716161707)]
    public class CMS20_06_InitialDbCreation : FluentMigrator.Migration // Changed from IMigration to Migration
    {
        public override void Up()
        {
            this.CreateTableIfNotExists("User",
                c =>
                {
                    return c
                    .WithColumn("UserId").AsInt32().NotNullable().PrimaryKey().Identity();
                    


                });

            this.CreateTableIfNotExists("CMS_Users", table => table
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Username").AsString(100).NotNullable()
                .WithColumn("PasswordHash").AsString(255));

        }

        public override void Down()
        {
            Delete.Table("CMS_Users");
        }
    }
}
