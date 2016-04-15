using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class ProjectConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DSLProjects",
                c => new
                {
                    ID = c.Int(false, true),
                    ProjectID = c.Int(false),
                    Name = c.String(),
                    ParserKind = c.String(),
                    LinkerKind = c.String(),
                    ExtendsNamespaces = c.Boolean(false),
                    ExtendsTypes = c.Boolean(false),
                    ExtendsMembers = c.Boolean(false),
                    ExtendsCode = c.Boolean(false)
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.DSLProjects");
        }
    }
}