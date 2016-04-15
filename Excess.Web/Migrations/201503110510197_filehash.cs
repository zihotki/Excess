using System.Data.Entity.Migrations;

namespace Excess.Web.Migrations
{
    public partial class filehash : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileHashes",
                c => new
                {
                    ID = c.Int(false, true),
                    FileID = c.Int(false),
                    Hash = c.Int(false)
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            DropTable("dbo.FileHashes");
        }
    }
}