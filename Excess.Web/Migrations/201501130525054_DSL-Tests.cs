using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class DSLTests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DSLTests",
                c => new
                {
                    ID = c.Guid(false),
                    ProjectID = c.Int(false),
                    Caption = c.String(),
                    Contents = c.String()
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.DSLTests");
        }
    }
}