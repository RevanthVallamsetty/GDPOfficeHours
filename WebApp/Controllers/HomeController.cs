using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebApp_OpenIDConnect_DotNet.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Graph;
using WebApp_OpenIDConnect_DotNet.Helpers;
using Resources;
using WebApp.Models;
using System.Net;

namespace WebApp.Controllers
{
    
    public class HomeController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();
        EventsService eventsService = new EventsService();

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
            if(Session["facultyMail"] != null)
            {
                var id = db.faculties.Find(Session["facultyMail"].ToString()).Id;
                return RedirectToAction("Select", new
                {
                    id,
                });
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

        
        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/Home/ReadMail" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}