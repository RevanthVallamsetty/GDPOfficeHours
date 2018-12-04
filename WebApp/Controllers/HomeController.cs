using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Graph;
using WebApp.Helpers;
using System.Net;
using Resources;
using System;
using WebApp.DAL;
using System.Collections.Generic;

namespace WebApp.Controllers
{

    public class HomeController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();
        EventsService eventsService = new EventsService();
        HomeService homeService = new HomeService();
        UsersService usersService = new UsersService();

        public async Task<ActionResult> Index()
        {
            if (Request.IsAuthenticated)
            {
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
                var userDetails = await eventsService.GetMyDetails(graphClient);

                var hasFaculty = db.faculties.Select(q => q.Email).ToList().Contains(userDetails.Mail ?? userDetails.UserPrincipalName);

                if (hasFaculty)
                {
                    Session["Role"] = "Faculty";
                    return View();
                }
                else
                {
                    Session["Role"] = "Student";
                    return RedirectToAction("Home");
                }
            }
            return RedirectToAction("Home");
        }

        public ActionResult SetRole(string role)
        {
            if(role != null && role.Equals("Faculty"))
            {
                Session["Role"] = role;
            }
            else if(role != null && role.Equals("Faculty"))
            {
                Session["Role"] = role;
            }

            return RedirectToAction("Home");
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Name = ClaimsPrincipal.Current.FindFirst("name").Value;
            ViewBag.AuthorizationRequest = string.Empty;
            // The object ID claim will only be emitted for work or school accounts at this time.
            Claim oid = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            ViewBag.ObjectId = oid == null ? string.Empty : oid.Value;

            // The 'preferred_username' claim can be used for showing the user's primary way of identifying themselves
            ViewBag.Username = ClaimsPrincipal.Current.FindFirst("preferred_username").Value;

            // The subject or nameidentifier claim can be used to uniquely identify the user
            ViewBag.Subject = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return View();
        }

        public ActionResult Home()
        {
            if(Session["facultyMail"] != null && Session["facultyPassword"] != null)
            {
                var mail = Session["facultyMail"].ToString();
                var password = Session["facultyPassword"].ToString();
                return RedirectToAction("Schedule", "Home");
            }
            else
            {
                var faculty = db.faculties.OrderBy(q => q.first_Name).ToList();
                return View(faculty.ToList());
            }
        }

        public ActionResult Select(int? id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var facultyMail = (from f in db.faculties where f.Id == id select f.Email).FirstOrDefault().ToString();
            if (facultyMail == null)
            {
                return HttpNotFound();
            }
            Session["facultyMail"] = facultyMail;
            IQueryable<OfficeSchedule> officeSchedules = db.officeSchedule
                .Where(o => o.faculty.Email.Equals(facultyMail.ToString()))
                .OrderBy(d => d.start_time);
            var sql = officeSchedules.ToString();
            return View(officeSchedules.ToList());
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var faculty = (from f in db.faculties where f.Id == id select f).FirstOrDefault();
            if (faculty == null)
            {
                return HttpNotFound();
            }
            Session["facultyMail"] = faculty.Email;
            IQueryable<OfficeSchedule> officeSchedules = db.officeSchedule
                .Where(o => o.faculty.Email.Equals(faculty.ToString()))
                .OrderBy(d => d.start_time);

            return View(faculty);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit()
        {
            
            var mail = Request.Form["Email"];
            var password = Request.Form["Password"];
            Session["facultyPassword"] = password;
            return RedirectToAction("Schedule","Home");
        }
        
        public ActionResult Schedule()
        {
            var mail = Session["facultyMail"].ToString();
            var password = Session["facultyPassword"].ToString();
            List<scheduleModel> schdMdlList = new List<scheduleModel>();
            scheduleModel scheduleModel;

            try
            {
                var results = homeService.LoadAppointments(mail, password);

                var groupResults = results.GroupBy(s => s.EventDate).ToList();


                if (groupResults != null && groupResults.Any())
                {
                    foreach(var eveitm in groupResults)
                    {
                        scheduleModel = new scheduleModel();
                        scheduleModel.eventTimeList = new List<EventTimes>();
                        foreach (var item in eveitm)
                        {
                            scheduleModel.EventDate = item.EventDate.ToString("d");
                            scheduleModel.EventDay = item.EventDay;
                            scheduleModel.eventTimeList.Add(new EventTimes() {
                                Subject = item.Subject,
                                EventEnd = item.EventEnd,
                                EventStart = item.EventStart
                            });
                        }
                        schdMdlList.Add(scheduleModel);
                    }
                }

                return View(schdMdlList);
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Error", new
                {
                    message = string.Format(Resource.Error_Message, Request.RawUrl, e.Message,
                   "User not present or invalid password")
                });
            }
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/Home/ReadMail" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
        public PartialViewResult status()
        {
            Faculty fac = db.faculties.Find(Session["facultymail"]);
            return PartialView("FacultyStatus",fac);
        }
       

    }
}