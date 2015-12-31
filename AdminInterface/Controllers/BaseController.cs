using System.Web.Mvc;
using Security.Models;

namespace AdminInterface.Controllers
{
    public class BaseController : Controller
    {
        protected new virtual CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }

    }

}