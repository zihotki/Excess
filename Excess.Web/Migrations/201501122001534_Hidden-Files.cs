using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class HiddenFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectFiles", "isHidden", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.ProjectFiles", "isHidden");
        }
    }
}