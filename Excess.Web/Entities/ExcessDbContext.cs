using System.Data.Entity;

namespace Excess.Web.Entities
{
    public class ExcessDbContext : DbContext
    {
        public DbSet<TranslationSample> Samples { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<DSLTest> DSLTests { get; set; }
        public DbSet<FileHash> FileCache { get; set; }

        public ExcessDbContext() :
            base("name=Default")
        {
        }
    }
}