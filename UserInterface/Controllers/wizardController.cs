using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.ListHub;

namespace UserInterface.Controllers
{
    public class wizardController : Controller
    {
        readonly IAgent _agents;
        readonly IListHub _listhub;
        public wizardController(IAgent agents,IListHub listhub)
        {
            _agents = agents;
            _listhub = listhub;
        }
        //
        // GET: /wizard/
        public ActionResult search()
        {
            return View();
        }

        public ViewResult CretifiedAgents(int count = 5)
        {
            var agents = _agents.GetCertifiedAgents(count);
            return View(agents);
        }

        public ViewResult FeaturedAgents(int count = 5)
        {
            var agents = _agents.GetFeaturedAgents(count);
            return View(agents);
        }

        public ViewResult ClassifiedListing(string type,int count = 5)
        {
            var listhub = _listhub.GetClassified(type, count);
            return View(listhub);
        }

        public ViewResult CommunityListing(string type, string communityName)
        {
            var _community = _listhub.GetPropertyBasedOnCommunity(type,communityName);
            return View(_community);
        }


	}
}