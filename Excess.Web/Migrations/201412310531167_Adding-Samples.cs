using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class AddingSamples : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TranslationSamples",
                c => new
                {
                    ID = c.Int(false, true),
                    Name = c.String(),
                    Contents = c.String()
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.TranslationSamples");
        }
    }
}