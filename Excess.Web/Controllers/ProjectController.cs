using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Excess.Compiler;
using Excess.RuntimeProject;
using Excess.Web.Entities;
using Excess.Web.Services;
using Microsoft.AspNet.Identity;

namespace Excess.Web.Controllers
{
    public class ProjectController : ExcessControllerBase
    {
        private readonly IProjectManager _manager;

        public ProjectController(IProjectManager manager)
        {
            _manager = manager;
        }

        public ActionResult LoadProject(int projectId)
        {
            var repo = new ProjectRepository();

            dynamic config;
            var project = repo.LoadProject(projectId, out config);
            if (project == null)
                return HttpNotFound();

            if (!project.IsSample)
            {
                if (!User.Identity.IsAuthenticated)
                    return HttpNotFound(); //td: right error

                if (User.Identity.GetUserId() != project.UserID)
                    return HttpNotFound(); //td: right error
            }

            var path = new Scope(null) as dynamic;
            path.ToolPath = Path.Combine(Server.MapPath("~/App_Data"), "Tools");

            IRuntimeProject runtime = _manager.CreateRuntime(project.ProjectType, project.Name, config, path, new ProjectStorage(project));
            foreach (var file in project.ProjectFiles)
                runtime.Add(file.Name, file.ID, file.Contents);

            Session["project"] = runtime;

            if (!project.IsSample)
                Session["projectId"] = project.ID;
            else
                Session["SampleProjectId"] = project.ID;

            return Json(new
            {
                defaultFile = runtime.DefaultFile(),
                tree = new[] {projectTree(project, runtime)}
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadFile(string file)
        {
            var project = Session["project"] as IRuntimeProject;
            if (project == null || file == null)
                return HttpNotFound(); //td: right error

            var contents = project.FileContents(file);
            if (contents == null)
                return HttpNotFound(); //td: right error

            return Content(contents);
        }

        [ValidateInput(false)]
        public ActionResult SaveFile(string file, string contents)
        {
            var project = Session["project"] as IRuntimeProject;
            if (project == null)
                return HttpNotFound(); //td: right error

            project.Modify(file, contents);

            if (Session["projectId"] != null)
            {
                var fileIdx = project.FileId(file);
                if (fileIdx < 0)
                    return HttpNotFound(); //td: right error

                var repo = new ProjectRepository();
                repo.SaveFile(fileIdx, contents);
            }

            return Content("ok");
        }

        public ActionResult CreateClass(string className)
        {
            var project = Session["project"] as IRuntimeProject;
            if (project == null)
                return HttpNotFound(); //td: right error

            var contents = " ";
            if (string.IsNullOrEmpty(Path.GetExtension(className)))
                contents = string.Format("class {0} \n{{\n}}", className);

            var fileId = -1;
            if (Session["projectId"] != null)
            {
                var repo = new ProjectRepository();
                fileId = repo.CreateFile((int) Session["projectId"], className, contents);
            }

            project.Add(className, fileId, contents);

            var node = new TreeNode
            {
                label = className,
                icon = "fa-code",
                action = "select-file",
                data = className,
                actions = new[]
                {
                    new TreeNodeAction {id = "remove-file", icon = "fa-times-circle-o"},
                    new TreeNodeAction {id = "open-tab", icon = "fa-arrow-circle-o-right"}
                }.Union(project.FileActions(className))
            };

            return Json(new
            {
                node
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Compile()
        {
            var project = Session["project"] as IRuntimeProject;
            if (project == null)
                return HttpNotFound(); //td: right error

            var result = new CompilationResult(project.Compile());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Execute(string notification)
        {
            var project = Session["project"] as IRuntimeProject;
            if (project == null)
                return HttpNotFound(); //td: right error

            dynamic clientData;
            var errors = project.Run(new HubNotifier(notification), out clientData);
            var result = new CompilationResult(errors, clientData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserProjects()
        {
            if (!User.Identity.IsAuthenticated)
                return HttpNotFound(); //td: right error

            var repo = new ProjectRepository();
            var projects = repo.GetUserProjects(User.Identity.GetUserId());
            return Json(projects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateProject(string projectType, string projectName, string projectData)
        {
            if (!User.Identity.IsAuthenticated)
                return HttpNotFound(); //td: right error

            var repo = new ProjectRepository();
            var result = repo.CreateProject(projectType, projectName, projectData, User.Identity.GetUserId());

            return Json(new {projectId = result.ID}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult debugDSL(string text)
        {
            var runtime = Session["project"] as IExtensionRuntime;
            if (runtime == null)
                return HttpNotFound(); //td: right error

            string result;
            try
            {
                result = runtime.DebugExtension(text);

                //td: !!!
                //var notProvider = runtime as IRuntimeProject;
                //var nots = notProvider.notifications();

                //if (nots.Any())
                //{
                //    StringBuilder notBuilder = new StringBuilder();
                //    foreach (var not in nots)
                //        notBuilder.AppendLine(not.Message);

                //    result = string.Format("{0} \n ================= Notifications ================= \n {1}", 
                //        result, notBuilder.ToString());
                //}
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Content(result);
        }

        public ActionResult GetDSLTests()
        {
            var project = Session["projectId"] as int?;
            if (project == null)
                project = Session["SampleProjectId"] as int?;

            if (project == null)
                return HttpNotFound(); //td: right error

            //td: !!! entity project
            var _db = new ExcessDbContext();
            var result = _db.DSLTests.Where(test => test.ProjectID == project);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [ValidateInput(false)]
        public ActionResult UpdateDSLTest(string id, string contents)
        {
            var project = Session["projectId"] as int?;
            if (project == null)
                return HttpNotFound(); //td: right error

            Guid testId;
            if (!Guid.TryParse(id, out testId))
                return HttpNotFound(); //td: right error

            var _db = new ExcessDbContext();
            var result = _db.DSLTests
                .Where(test => test.ID == testId)
                .FirstOrDefault();

            if (result == null)
                return HttpNotFound(); //td: right error

            result.Contents = contents;
            _db.SaveChanges();

            return Content("ok");
        }

        [ValidateInput(false)]
        public ActionResult AddDSLTest(string name, string contents)
        {
            var project = Session["projectId"] as int?;
            if (project == null)
                return HttpNotFound(); //td: right error

            var _db = new ExcessDbContext();

            var result = new DSLTest
            {
                ID = Guid.NewGuid(),
                ProjectID = (int) project,
                Caption = name,
                Contents = contents
            };

            _db.DSLTests.Add(result);
            _db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateGrammar()
        {
            var runtime = Session["project"] as IExtensionRuntime;
            if (runtime == null)
                return HttpNotFound(); //td: right error

            string extension, transform;
            if (!runtime.GenerateGrammar(out extension, out transform))
                return HttpNotFound(); //td: right error

            return Json(new
            {
                extension,
                transform
            }, JsonRequestBehavior.AllowGet);
        }

        private TreeNode projectTree(Project project, IRuntimeProject runtime)
        {
            var result = new TreeNode
            {
                label = project.Name,
                icon = "fa-sitemap",
                actions = new[]
                {
                    new TreeNodeAction {id = "add-class", icon = "fa-plus-circle"}
                },
                children = project.ProjectFiles
                    .Where(projectFile => !projectFile.isHidden)
                    .Select(projectFile =>
                    {
                        return new TreeNode
                        {
                            label = projectFile.Name,
                            icon = "fa-code",
                            action = "select-file",
                            data = projectFile.Name,
                            actions = new[]
                            {
                                new TreeNodeAction {id = "remove-file", icon = "fa-times-circle-o"},
                                new TreeNodeAction {id = "open-tab", icon = "fa-arrow-circle-o-right"}
                            }.Union(runtime.FileActions(projectFile.Name))
                        };
                    })
            };

            return result;
        }

        private class ProjectStorage : IPersistentStorage
        {
            private readonly Project _project;

            public ProjectStorage(Project project)
            {
                _project = project;
            }

            public int AddFile(string name, string contents, bool hidden)
            {
                var repo = new ProjectRepository();
                return repo.AddFile(_project, name, contents, hidden);
            }

            public int CachedId(string name)
            {
                var file = _project.Find(name);
                if (file == null)
                    return 0;

                var repo = new ProjectRepository();
                return repo.fileCache(file.ID);
            }

            public void CachedId(string name, int hash)
            {
                var repo = new ProjectRepository();

                var file = _project.Find(name);
                var fileID = -1;
                if (file == null)
                    fileID = repo.GetFileId(name, _project.ID);
                else
                    fileID = file.ID;

                Debug.Assert(fileID >= 0);
                repo.fileCache(fileID, hash);
            }
        }

        private class CompilationResult
        {
            public bool Succeded { get; set; }
            public IEnumerable<Error> Errors { get; set; }
            public dynamic ClientData { get; set; }

            public CompilationResult(IEnumerable<Error> errors, dynamic clientData = null)
            {
                Succeded = errors == null || !errors.Any();
                Errors = errors;
                ClientData = clientData;
            }
        }
    }
}