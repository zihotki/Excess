using System.Linq;
using System.Text;
using System.Web.Mvc;
using Excess.Web.Entities;

namespace Excess.Web.Controllers
{
    public class XSController : ExcessControllerBase
    {
        private readonly ExcessDbContext _db = new ExcessDbContext();
        private readonly ITranslationService _translator;

        public XSController(ITranslationService translator)
        {
            _translator = translator;
        }

        public ActionResult GetSamples()
        {
            var samples = from sample in _db.Samples
                select new
                {
                    id = sample.ID,
                    desc = sample.Name
                };

            return Json(samples, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSample(int id)
        {
            var content = from sample in _db.Samples
                where sample.ID == id
                select sample.Contents;

            return Content(content.First());
        }

        [ValidateInput(false)]
        public ActionResult Translate(string text)
        {
            var result = _translator.Translate(text);
            return Content(result);
        }

        public ActionResult GetKeywords()
        {
            var result = new StringBuilder();

            //td: !!!
            //foreach (string kw in factory.supported())
            //{
            //    result.Append(" ");
            //    result.Append(kw);
            //}

            return Content(result.ToString());
        }

        public ActionResult GetSampleProjects()
        {
            var repo = new ProjectRepository();
            return Json(repo.GetSampleProjects(), JsonRequestBehavior.AllowGet);
        }
    }
}