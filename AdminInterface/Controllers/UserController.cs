using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AdminInterface.Models;
using AdminInterface.Models.Mail;
using AutoMapper;
using Newtonsoft.Json;
using RazorEngine.Templating;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Mail;
using Repositories.Models.Admin.User;
using Repositories.Models.DataTable;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using Security.Models;
using Utility;
using System.Collections.Generic;
using Repositories.Interfaces.Admin.Office;
using Repositories.Interfaces.ListHub;

namespace AdminInterface.Controllers
{
    [RoutePrefix("agent")]

    public class UserController : BaseController
    {
        private readonly IAgent _agent;
        private readonly IMailBase _mailBase;

        private readonly IListHub _listHub;
        private readonly IOffice _office;
        public UserController(IAgent agent, IMailBase mailBase, IListHub listHub, IOffice office)
        {
            _agent = agent;
            _mailBase = mailBase;
            _listHub = listHub;
            _office = office;
        }

        #region only Admin Can Access
        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-listing")]
        public ActionResult UserListing()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-listing-ajax-handler")]
        public ActionResult UserAjaxHandler(JQueryDataTableParamModel param)
        {
            long filterCount = 0;
            var listHubPropertysearchCriteria = new UserDataTable()
            {
                isParticipantIdSearchable = Convert.ToBoolean(Request["bSearchable_0"]),
                isFirstNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]),
                isLastNameSearchable = Convert.ToBoolean(Request["bSearchable_2"]),
                isEmailSearchable = Convert.ToBoolean(Request["bSearchable_3"]),
                isPrimaryContactPhoneSearchable = Convert.ToBoolean(Request["bSearchable_4"]),
                isOfficePhoneSearchable = Convert.ToBoolean(Request["bSearchable_5"]),

                isParticipantIdSortable = Convert.ToBoolean(Request["bSortable_0"]),
                isFirstNameSortable = Convert.ToBoolean(Request["bSortable_1"]),
                isLastNameSortable = Convert.ToBoolean(Request["bSortable_2"]),
                isEmailSortable = Convert.ToBoolean(Request["bSortable_3"]),
                isPrimaryContactPhoneSortable = Convert.ToBoolean(Request["bSortable_4"]),
                isOfficePhoneSortable = Convert.ToBoolean(Request["bSortable_5"]),


                sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]),
                sortDirection = Request["sSortDir_0"]
            };

            var filteredData = _agent.GetDataSet("", param, listHubPropertysearchCriteria, out filterCount, "purchase");

            var totalCount = _agent.GetTotalCount("");

            var result = from c in filteredData
                         select
                             new[]
                             {
                         c.ParticipantId,
                         c.FirstName,
                         c.LastName,
                         c.Email,
                         c.PrimaryContactPhone,
                         c.OfficePhone,
                         string.Join(",",c.Roles),
                         Convert.ToString(c.IsActive),
                         Convert.ToString(c.IsEmailSend),
                             };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filterCount,
                aaData = result
            },
                JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("manage/{uniquid}/{Office}")]
        public ActionResult ManageUser(string uniquid, string Office)
        {
            var manageUser = _agent.GetAgentDetails(uniquid);
            ViewBag.UpdateType = Office;
            return View(manageUser);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-activation/{uniquid}")]
        public ActionResult SendActivationMail(string uniquid)
        {
            string message;
            var agentDetails = _agent.GetAgentDetails(uniquid);

            if (agentDetails != null)
            {
                var actuallMail = agentDetails.Email;
#if DEBUG
                agentDetails.Email = "ankit.sarkar@indusnet.co.in,edwindantas@gmail.com,vikas@ntooitive.com";
#endif

                var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/EmailTemplates");
                var razorTemplateFileLocation = Path.Combine(templateFolderPath, "send_invitation.cshtml");
                var razorText = System.IO.File.ReadAllText(razorTemplateFileLocation);
                var model = new SendInvitation { FuzzyLink = LinkCreator.CreateAgentInvitationLink(agentDetails.ParticipantId) };
                var templateService = new TemplateService();
                var emailHtmlBody = templateService.Parse(razorText, model, null, null);


                var result = _mailBase.SendMail(new[] { agentDetails.Email }, "Next Step: Activate Your Account", emailHtmlBody);
                if (result)
                {
                    var dbResult = _agent.InitiateRegistration(agentDetails.ParticipantId, actuallMail);
                    message = dbResult
                        ? "Activation mail send successfully. Please ask your agent to check inbox for more detail."
                        : "Unable to send the mail. Please try again later.";
                }
                else
                {
                    message = "Unable to send the mail. Please try again later.";
                }

            }
            else
            {
                message = "Unable to send the mail. Please try again later.";

            }


            return Json(new
            {
                Message = message
            },
                JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-deactivation/{uniquid}")]
        public ActionResult DeactivateAccount(string uniquid, bool isActivated)
        {
            var deactivateUser = _agent.DeactiveAgent(uniquid, isActivated);

            var message = deactivateUser
                ? "Activation mail send successfully. Please ask your agent to check inbox for more detail."
                : "Unable to send the mail. Please try again later.";


            return Json(new
            {
                Message = message
            },
                JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("new-agent")]
        public ActionResult Add(string agentId)
        {
            Participant p = new Participant();
            p.GetOfficeList = _office.GetOffice();
            if (!String.IsNullOrEmpty(agentId))
            {
                var editUser = _agent.GetAgentDetails(agentId);
                p.ParticipantId = editUser.ParticipantId;
                p.ParticipantKey = editUser.ParticipantKey;
                p.FirstName = editUser.FirstName;
                p.LastName = editUser.LastName;
                p.PrimaryContactPhone = editUser.PrimaryContactPhone;
                p.OfficePhone = editUser.OfficePhone;
                p.Email = editUser.Email;
                p.WebsiteURL = editUser.WebsiteURL;
                p.AgentDescription = editUser.AgentDescription;
                p.Role = editUser.Roles[0];
                p.OfficeName = editUser.OfficeName;
                p.OfficeId = editUser.OfficeId;
                return View(p);
            }
            else
            {
                return View(p);
            }

        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("delete-agent")]
        public JsonResult DeleteUser(string agentId)
        {
            bool IsDeleted = _agent.DeleteUser(agentId, true);

            return Json(true);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("add-agent")]
        public JsonResult AddAgent(User user)
        {
            if (_agent.Validation(user))
            {
                if (String.IsNullOrEmpty(user.ParticipantId))
                {
                    //add agent.
                    user.ParticipantId = Utility.UtilityClass.GetUniqueKey();
                    user.ParticipantKey = "3yd-SDRE-" + user.ParticipantId;
                    user.InitiateDate = DateTime.Now;
                    user.IsUpdateByPortal = true;
                    //user.Role = "Listing";
                    user.Roles = new string[] { user.Role };
                    List<User> userList = new List<Repositories.Models.Admin.User.User>();
                    userList.Add(user);
                    bool IsInserted = _agent.InsertBulkUsers(userList);

#if DEBUG
                    user.Email = "amitabha.saha@indusnet.co.in";//ankit.sarkar@indusnet.co.in,edwindantas@gmail.com,vikas@ntooitive.com";
#endif

                    var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/EmailTemplates");
                    var razorTemplateFileLocation = Path.Combine(templateFolderPath, "send_invitation.cshtml");
                    var razorText = System.IO.File.ReadAllText(razorTemplateFileLocation);
                    var model = new SendInvitation { FuzzyLink = LinkCreator.CreateAgentInvitationLink(user.ParticipantId) };
                    var templateService = new TemplateService();
                    var emailHtmlBody = templateService.Parse(razorText, model, null, null);


                    var result = _mailBase.SendMail(new[] { user.Email }, "Next Step: Activate Your Account", emailHtmlBody);
                    return Json(new { success = true, ParticipantId = user.ParticipantId, update = false });
                }
                else
                {
                    user.Roles = new string[] { user.Role };
                    bool IsInserted = _agent.UpdateUser(user);
                    return Json(new { success = true, ParticipantId = user.ParticipantId, update = true });
                }
            }
            else
            {
                return Json(new { success = false, ParticipantId = user.ParticipantId, update = true });
            }

        }


        [Route("activate/{hashCode}")]
        public ActionResult ActivateAgent(string hashCode)
        {
            var uniquid = UtilityClass.Base64Decode(hashCode);
            var agentDetails = _agent.GetAgentDetails(uniquid);
            var registartionViewModel = Mapper.Map<RegistartionViewModel>(agentDetails);
            return View(registartionViewModel);
        }

        [Route("registration")]
        public ActionResult Registration(RegistartionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activateUser = _agent.SetPassword(model.ParticipantId, model.Email, HashPassword.Encrypt(model.Password));
                if (activateUser)
                {
#if DEBUG
                    model.Email = "ankit.sarkar@indusnet.co.in,edwindantas@gmail.com,vikas@ntooitive.com";
#endif

                    var templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/EmailTemplates");
                    var razorTemplateFileLocation = Path.Combine(templateFolderPath, "agent_activation.cshtml");
                    var razorText = System.IO.File.ReadAllText(razorTemplateFileLocation);
                    var SendInvitation = new SendInvitation { FuzzyLink = ConfigurationManager.AppSettings["URL"] + "agent/agent-login" };
                    var templateService = new TemplateService();
                    var emailHtmlBody = templateService.Parse(razorText, SendInvitation, null, null);
                    _mailBase.SendMail(new[] { model.Email }, "Next Step: Login Your Account", emailHtmlBody);
                    return Redirect("/property/purchase-property-listing");

                }
                else
                {
                    return View("ActivateAgent", model);
                }
            }
            else
            {
                return View("ActivateAgent", model);
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-featured/{uniquid}")]
        public ActionResult FeaturedAgent(string uniquid, bool isActivated)
        {
            var deactivateUser = _agent.FeaturedAgent(uniquid, isActivated);

            var message = deactivateUser
                ? "Activation mail send successfully. Please ask your agent to check inbox for more detail."
                : "Unable to send the mail. Please try again later.";


            return Json(new
            {
                Message = message
            },
                JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Route("agent-certified/{uniquid}")]
        public ActionResult CertifiedAgent(string uniquid, bool isActivated)
        {
            var deactivateUser = _agent.CertifiedAgent(uniquid, isActivated);

            var message = deactivateUser
                ? "Activation mail send successfully. Please ask your agent to check inbox for more detail."
                : "Unable to send the mail. Please try again later.";


            return Json(new
            {
                Message = message
            },
                JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region only Agent & Admin Can Access
        [Route("agent-profile")]
        [CustomAuthorize(Roles = "Agent,Admin")]
        public ActionResult AgentProfile()
        {
            return View(new Repositories.Models.ViewModel.ResetPassword());
        }

        [Route("agent-profile")]
        [HttpPost]
        [CustomAuthorize(Roles = "Agent,Admin")]
        public ActionResult AgentProfile(Repositories.Models.ViewModel.ResetPassword Model)
        {
            if (ModelState.IsValid)
            {
                if (_agent.ResetPassword(User.UserId, Model.OldPassword, Model.NewPassword))
                    ViewData["success"] = true;
                else
                    ViewData["success"] = false;
            }
            else
            {
                ViewData["success"] = false;
            }

            return View();
        }
        #endregion

        #region Anonymous Access
        [Route("agent-login")]
        public ActionResult Login()
        {
            if (User != null)
            {
                if (User.Roles.Contains("Admin"))
                {
                    return Redirect("/agent/agent-listing");
                }
                else if (User.Roles.Contains("Agent"))
                {
                    return Redirect("/property/purchase-property-listing");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View("Index");
        }

        [HttpPost]
        [Route("agent-login")]
        public ActionResult Login(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _agent.Login(viewModel.UserEmail, viewModel.Password);

                if (user != null)
                {
                    var roles = user.Roles.Select(m => m).ToArray();

                    var serializeModel = new CustomPrincipalSerializeModel
                    {
                        UserId = user.Id.ToString(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Roles = roles,
                        ProfileImage = user.ProfileImage,
                        ParticipantId = user.ParticipantId
                    };

                    string userData = JsonConvert.SerializeObject(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                             1,
                            user.Email,
                             DateTime.Now,
                             DateTime.Now.AddMinutes(30),
                             false,
                             userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (roles.Contains("Admin"))
                    {
                        return Redirect("/agent/agent-listing");
                    }
                    else if (roles.Contains("Agent"))
                    {
                        return Redirect("/property/purchase-property-listing");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Incorrect username and/or password");
            }

            return View("Index", viewModel);
        }


        [Route("agent-signout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/agent/agent-login");
        }
        #endregion

        #region Make All Property As Featured
        [Route("make-featured")]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult MakeFeatured(string fieldbyUpdate, string uniquid, bool value, string type)
        {
            fieldbyUpdate = fieldbyUpdate.Replace('_', '.');
            var result = _listHub.MakeBulkFeatured(fieldbyUpdate, value, uniquid, type);
            if (type == "rent")
            {
                var updateAgent = _agent.UpdateUser(uniquid, "IsAllRentPropertyFeatured", value);

            }
            else
            {
                var updateAgent = _agent.UpdateUser(uniquid, "IsAllPurchasePropertyFeatured", value);

            }
            return Json(result);
        }
        #endregion

    }


}