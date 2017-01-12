using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/Index
        public ActionResult Index(string message)
        {
            return View((object)message);
        }
    }
}