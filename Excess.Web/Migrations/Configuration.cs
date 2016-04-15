namespace Excess.Web.Migrations
{
    using Excess.Web.Entities;
    using Excess.Web.Resources;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Excess.Web.Entities.ExcessDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Excess.Web.Entities.ExcessDbContext context)
        {
            foreach (var sample in TranslationSamples())
                context.Samples.AddOrUpdate(sample);

            int projectSamples = 1;
            foreach (var sampleProject in SampleProjects())
            {
                projectSamples++;
                context.Projects.AddOrUpdate(sampleProject);
            }

            reserveProjects(context, projectSamples, 299);

            int projectFileSamples = 1;
            foreach (var file in SampleProjectFiles())
            {
                projectFileSamples++;
                context.ProjectFiles.AddOrUpdate(file);
            }

            reserveProjectFiles(context, projectFileSamples, 999);

            foreach (var dslTest in DSLTests())
            {
                context.DSLTests.AddOrUpdate(dslTest);
            }
        }

        private void reserveProjectFiles(ExcessDbContext context, int projectFileSamples, int reserveCount)
        {
            for (int i = projectFileSamples; i < reserveCount; i++)
            {
                var reserved = new ProjectFile { ID = i };
                context.ProjectFiles.AddOrUpdate(reserved);
            }
        }

        private void reserveProjects(ExcessDbContext context, int projectSamples, int reserveCount)
        {
            for (int i = projectSamples; i < reserveCount; i++)
            {
                var reserved = new Project { ID = i, IsSample = true };
                context.Projects.AddOrUpdate(reserved);
            }
        }

        private TranslationSample[] TranslationSamples()
        {
            return new TranslationSample[]
            {
                new TranslationSample
                {
                    ID = 1,
                    Name = "Functions",
                    Contents = Samples.Functions,
                },

                new TranslationSample
                {
                    ID = 2,
                    Name = "Events",
                    Contents = Samples.Events,
                },

                new TranslationSample
                {
                    ID = 3,
                    Name = "Arrays",
                    Contents = Samples.Arrays,
                },

                new TranslationSample
                {
                    ID = 4,
                    Name = "Misc",
                    Contents = Samples.Misc,
                },

                new TranslationSample
                {
                    ID = 5,
                    Name = "Extension (asynch/synch)",
                    Contents = Samples.Extensions,
                },
            };
        }
        private ProjectFile[] SampleProjectFiles()
        {
            return new ProjectFile[]
            {
                //Hello world
                new ProjectFile
                {
                    ID = 1,
                    OwnerProject = 1,
                    Name = "application",
                    Contents = Samples.HelloWorld,
                },

                //Lolcats
                new ProjectFile
                {
                    ID = 2,
                    OwnerProject = 2,
                    Name = "application",
                    Contents = Samples.LolCatsApplication,
                },

                new ProjectFile
                {
                    ID = 3,
                    OwnerProject = 2,
                    Name = "lolcat",
                    Contents = Samples.LolCats,
                },

                new ProjectFile
                {
                    ID = 4,
                    OwnerProject = 2,
                    Name = "speek",
                    Contents = Samples.LolCatsSpeek,
                },

                new ProjectFile
                {
                    ID = 5,
                    OwnerProject = 2,
                    Name = "trollcat",
                    Contents = Samples.LolCatsTrollcat,
                },

                //contract
                new ProjectFile
                {
                    ID           = 6,
                    OwnerProject = 3,
                    Name         = "plugin",
                    isHidden     = true,
                    Contents     = ProjectTemplates.ExtensionPlugin
                },

                new ProjectFile
                {
                    ID           = 7,
                    OwnerProject = 3,
                    Name         = "extension",
                    Contents     = Samples.ContractExtension,
                },

                new ProjectFile
                {
                    ID           = 8,
                    OwnerProject = 3,
                    Name         = "Transform",
                    Contents     = Samples.ContractTransform,
                },

                //match
                new ProjectFile
                {
                    ID           = 9,
                    OwnerProject = 4,
                    Name         = "plugin",
                    isHidden     = true,
                    Contents     = ProjectTemplates.ExtensionPlugin
                },

                new ProjectFile
                {
                    ID           = 10,
                    OwnerProject = 4,
                    Name         = "extension",
                    Contents     = Samples.MatchExtension,
                },

                new ProjectFile
                {
                    ID           = 11,
                    OwnerProject = 4,
                    Name         = "Transform",
                    Contents     = Samples.MatchTransform,
                },

                //asynch
                new ProjectFile
                {
                    ID           = 12,
                    OwnerProject = 5,
                    Name         = "plugin",
                    isHidden     = true,
                    Contents     = ProjectTemplates.ExtensionPlugin
                },

                new ProjectFile
                {
                    ID           = 13,
                    OwnerProject = 5,
                    Name         = "extension",
                    Contents     = Samples.AsynchExtension,
                },

                new ProjectFile
                {
                    ID           = 14,
                    OwnerProject = 5,
                    Name         = "Transform",
                    Contents     = Samples.AsynchTransform,
                },

                //philosophers
                new ProjectFile
                {
                    ID           = 15,
                    OwnerProject = 6,
                    Name         = "application",
                    Contents     = Samples.PhilosophersApp
                },

                new ProjectFile
                {
                    ID           = 16,
                    OwnerProject = 6,
                    Name         = "philosopher",
                    Contents     = Samples.Philosophers,
                },

                new ProjectFile
                {
                    ID           = 17,
                    OwnerProject = 6,
                    Name         = "chopstick",
                    Contents     = Samples.Chopsticks,
                },

                //starving philosophers
                new ProjectFile
                {
                    ID           = 18,
                    OwnerProject = 7,
                    Name         = "application",
                    Contents     = Samples.PhilosophersApp
                },

                new ProjectFile
                {
                    ID           = 19,
                    OwnerProject = 7,
                    Name         = "philosopher",
                    Contents     = Samples.StarvingPhilosophers,
                },

                new ProjectFile
                {
                    ID           = 20,
                    OwnerProject = 7,
                    Name         = "chopstick",
                    Contents     = Samples.Chopsticks,
                },

                //santa's shop
                new ProjectFile
                {
                    ID           = 21,
                    OwnerProject = 8,
                    Name         = "application",
                    Contents     = Samples.SantaShopApp
                },

                new ProjectFile
                {
                    ID           = 22,
                    OwnerProject = 8,
                    Name         = "santa",
                    Contents     = Samples.SantaShopSanta,
                },

                new ProjectFile
                {
                    ID           = 23,
                    OwnerProject = 8,
                    Name         = "shop",
                    Contents     = Samples.SantaShopShop,
                },

                //santa's creatures
                new ProjectFile
                {
                    ID           = 24,
                    OwnerProject = 9,
                    Name         = "application",
                    Contents     = Samples.SantaCreaturesApp
                },

                new ProjectFile
                {
                    ID           = 25,
                    OwnerProject = 9,
                    Name         = "santa",
                    Contents     = Samples.SantaCreaturesSanta,
                },

                new ProjectFile
                {
                    ID           = 26,
                    OwnerProject = 9,
                    Name         = "creatures",
                    Contents     = Samples.SantaCreaturesCreatures,
                },

                //barbershop
                new ProjectFile
                {
                    ID           = 27,
                    OwnerProject = 10,
                    Name         = "application",
                    Contents     = Samples.BarbersApp
                },

                new ProjectFile
                {
                    ID           = 28,
                    OwnerProject = 10,
                    Name         = "barbershop",
                    Contents     = Samples.BarbersShop,
                },

                new ProjectFile
                {
                    ID           = 29,
                    OwnerProject = 10,
                    Name         = "barber",
                    Contents     = Samples.BarbersBarber,
                },

                //barbershop
                new ProjectFile
                {
                    ID           = 30,
                    OwnerProject = 11,
                    Name         = "application",
                    Contents     = Samples.ReadersWriterApp
                },

                new ProjectFile
                {
                    ID           = 31,
                    OwnerProject = 11,
                    Name         = "resource",
                    Contents     = Samples.ReadersWriterResource,
                },

                new ProjectFile
                {
                    ID           = 32,
                    OwnerProject = 11,
                    Name         = "client",
                    Contents     = Samples.ReadersWriterClient,
                },

                //factorial
                new ProjectFile
                {
                    ID           = 33,
                    OwnerProject = 12,
                    Name         = "application",
                    Contents     = Samples.FactorialApp
                },

                new ProjectFile
                {
                    ID           = 34,
                    OwnerProject = 12,
                    Name         = "factorial",
                    Contents     = Samples.Factorial,
                },
            };
        }

        private Project[] SampleProjects()
        {
            return new Project[]
            {
                new Project
                {
                    ID = 1,
                    ProjectType = "console",
                    Name = "Hello World",
                    IsSample = true,
                },

                new Project
                {
                    ID = 2,
                    ProjectType = "console",
                    Name = "LOL Cats",
                    IsSample = true,
                },

                new Project
                {
                    ID = 3,
                    ProjectType = "extension",
                    Name = "Contract Extension",
                    IsSample = true,
                },

                new Project
                {
                    ID = 4,
                    ProjectType = "extension",
                    Name = "Match Extension",
                    IsSample = true,
                },

                new Project
                {
                    ID = 5,
                    ProjectType = "extension",
                    Name = "Asynch Extension",
                    IsSample = true,
                },

                new Project
                {
                    ID = 6,
                    ProjectType = "concurrent",
                    Name = "Dining Philosophers",
                    IsSample = true,
                },

                new Project
                {
                    ID = 7,
                    ProjectType = "concurrent",
                    Name = "Starving Philosophers",
                    IsSample = true,
                },

                new Project
                {
                    ID = 8,
                    ProjectType = "concurrent",
                    Name = "Santa's Shop",
                    IsSample = true,
                },

                new Project
                {
                    ID = 9,
                    ProjectType = "concurrent",
                    Name = "Santa's Creatures",
                    IsSample = true,
                },

                new Project
                {
                    ID = 10,
                    ProjectType = "concurrent",
                    Name = "Barbershop",
                    IsSample = true,
                },

                new Project
                {
                    ID = 11,
                    ProjectType = "concurrent",
                    Name = "Readers Writer",
                    IsSample = true,
                },

                new Project
                {
                    ID = 12,
                    ProjectType = "concurrent",
                    Name = "Factorial",
                    IsSample = true,
                },
            };
        }

        static Guid ContractUsage  = new Guid("E8FB63DB-D135-4FE9-893A-24A4162A1D0B");
        static Guid MatchUsage  = new Guid("6C3371F4-59AD-4D74-91BA-50C5A9424632");
        static Guid AsynchUsage = new Guid("834A3BDB-40A4-4025-8588-FB341C22A2E6");

        private DSLTest[] DSLTests()
        {
            return new DSLTest[]
            {
                new DSLTest
                {
                    ID          = ContractUsage,
                    ProjectID   = 3,
                    Caption     = "Usage",
                    Contents    = Samples.ContractUsage,
                },

                new DSLTest
                {
                    ID          = MatchUsage,
                    ProjectID   = 4,
                    Caption     = "Usage",
                    Contents    = Samples.MatchUsage,
                },
                new DSLTest
                {
                    ID          = AsynchUsage,
                    ProjectID   = 5,
                    Caption     = "Usage",
                    Contents    = Samples.AsynchUsage,
                },
            };
        }
    }
}
