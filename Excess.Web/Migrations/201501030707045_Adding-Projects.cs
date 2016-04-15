using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class AddingProjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectFiles",
                c => new
                {
                    ID = c.Int(false, true),
                    Name = c.String(),
                    Contents = c.String(),
                    OwnerProject = c.Int(false)
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Projects",
                c => new
                {
                    ID = c.Int(false, true),
                    Name = c.String(),
                    ProjectType = c.String(),
                    IsSample = c.Boolean(false),
                    UserID = c.String()
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectFiles");
        }
    }
}