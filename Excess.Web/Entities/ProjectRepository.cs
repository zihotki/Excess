﻿using System;
using System.Collections.Generic;
using System.Linq;
using Excess.Web.Resources;

namespace Excess.Web.Entities
{
    public class ProjectRepository
    {
        private readonly ExcessDbContext _db = new ExcessDbContext();

        public IEnumerable<Project> GetUserProjects(string userId)
        {
            return from project in _db.Projects
                where project.UserID == userId
                select project;
        }

        public IEnumerable<Project> GetSampleProjects()
        {
            return from project in _db.Projects
                where project.IsSample && project.Name != null
                select project;
        }

        public Project LoadProject(int projectId, out dynamic config)
        {
            config = null;
            var projects = from project in _db.Projects
                where project.ID == projectId
                select project;

            var result = projects.FirstOrDefault();
            if (result != null)
            {
                LoadProject(result);
                //config = _db.DSLProjects.SingleOrDefault(cfg => cfg.ProjectID == result.ID);
            }

            return result;
        }

        public int fileCache(int fileID)
        {
            var file = _db
                .FileCache
                .Where(f => f.FileID == fileID)
                .FirstOrDefault();

            if (file == null)
                return 0;

            return file.Hash;
        }

        public int GetFileId(string name, int projectID)
        {
            var file = _db
                .ProjectFiles
                .Where(f => f.OwnerProject == projectID && f.Name == name)
                .FirstOrDefault();

            if (file != null)
                return file.ID;

            return -1;
        }

        public void fileCache(int fileID, int hash)
        {
            var file = _db
                .FileCache
                .Where(f => f.FileID == fileID)
                .FirstOrDefault();

            if (file == null)
            {
                file = new FileHash {FileID = fileID, Hash = hash};
                _db.FileCache.Add(file);
            }
            else
                file.Hash = hash;

            _db.SaveChanges();
        }

        public void LoadProject(Project project)
        {
            var files = from projectFile in _db.ProjectFiles
                where projectFile.OwnerProject == project.ID
                select projectFile;

            project.ProjectFiles = new List<ProjectFile>(files);
        }

        public int AddFile(Project project, string name, string contents, bool hidden)
        {
            var file = new ProjectFile
            {
                Name = name,
                Contents = contents,
                OwnerProject = project.ID,
                isHidden = hidden
            };

            _db.ProjectFiles.Add(file);
            project.ProjectFiles.Add(file);

            _db.SaveChanges();
            return file.ID;
        }

        public void SaveProject(Project project)
        {
            _db.SaveChanges();
        }

        public void SaveFile(int fileId, string contents)
        {
            var files = from projectFile in _db.ProjectFiles
                where projectFile.ID == fileId
                select projectFile;
            var file = files.FirstOrDefault();
            if (file != null)
            {
                file.Contents = contents;
                _db.SaveChanges();
            }
        }

        public int CreateFile(int projectId, string fileName, string contents)
        {
            var newFile = new ProjectFile
            {
                Name = fileName,
                Contents = contents,
                OwnerProject = projectId
            };

            _db.ProjectFiles.Add(newFile);
            _db.SaveChanges();

            return newFile.ID;
        }

        public Project CreateProject(string projectType, string projectName, string projectData, string userId)
        {
            var project = new Project
            {
                IsSample = false,
                Name = projectName,
                ProjectType = projectType,
                UserID = userId
            };

            var files = new List<ProjectFile>();

            switch (projectType)
            {
                case "console":
                {
                    files.Add(new ProjectFile {Name = "application", Contents = ProjectTemplates.ConsoleApplication});
                    break;
                }

                case "extension":
                {
                    files.Add(new ProjectFile
                    {
                        Name = "plugin",
                        isHidden = true,
                        Contents = ProjectTemplates.ExtensionPlugin
                    });

                    files.Add(new ProjectFile
                    {
                        Name = "extension",
                        Contents = ProjectTemplates.Extension
                    });

                    files.Add(new ProjectFile
                    {
                        Name = "Transform",
                        Contents = ProjectTemplates.Transform
                    });
                    break;
                }
                //case "dsl":
                //{
                //    DSLConfiguration config = JsonConvert.DeserializeObject<DSLConfiguration>(projectData);

                //    //td: parser and linker types
                //    StringBuilder members = new StringBuilder();
                //    if (config.extendsNamespaces)
                //        members.AppendLine(ProjectTemplates.DSLParseNamespace);
                //    if (config.extendsTypes)
                //        members.AppendLine(ProjectTemplates.DSLParseType);
                //    if (config.extendsMembers)
                //        members.AppendLine(ProjectTemplates.DSLParseMember);
                //    if (config.extendsCode)
                //        members.AppendLine(ProjectTemplates.DSLParseCode);

                //    files.Add(new ProjectFile
                //    {
                //        Name     = "parser",
                //        Contents = string.Format(ProjectTemplates.DSLParser, members.ToString()) 
                //    });

                //    files.Add(new ProjectFile
                //    {
                //        Name     = "linker",
                //        Contents = ProjectTemplates.DSLLinker
                //    });

                //    files.Add(new ProjectFile
                //    {
                //        Name     = "plugin",
                //        isHidden = true,
                //        Contents = string.Format(ProjectTemplates.DSLPlugin, config.name)
                //    });

                //    dslConfig = new DSLProject
                //    {
                //        Name = config.name,
                //        ParserKind = config.parser,
                //        LinkerKind = config.linker,
                //        ExtendsNamespaces = config.extendsNamespaces,
                //        ExtendsTypes = config.extendsTypes,
                //        ExtendsMembers = config.extendsMembers,
                //        ExtendsCode = config.extendsCode,
                //    };
                //    break;
                //}

                default:
                    throw new InvalidOperationException("Invalid project type: " + projectType);
            }

            _db.Projects.Add(project);
            _db.SaveChanges();

            foreach (var file in files)
            {
                file.OwnerProject = project.ID;
                _db.ProjectFiles.Add(file);
            }

            //if (dslConfig != null)
            //{
            //    dslConfig.ProjectID = project.ID;
            //    _db.DSLProjects.Add(dslConfig);
            //}

            _db.SaveChanges();
            return project;
        }

        private class DSLConfiguration
        {
            public string name { get; set; }
            public string parser { get; set; }
            public string linker { get; set; }
            public bool extendsNamespaces { get; set; }
            public bool extendsTypes { get; set; }
            public bool extendsMembers { get; set; }
            public bool extendsCode { get; set; }
        }
    }
}