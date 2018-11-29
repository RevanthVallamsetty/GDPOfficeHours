using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class MessageController : Controller
    {
        EventsService eventsService = new EventsService();
        // GET: Message
        public ActionResult getMyMessages()
        {            
            string fac = Session["facultyMail"].ToString();
            OfficeHoursContext officeHoursContext = new OfficeHoursContext();
            List<Faculty> faculties = officeHoursContext.faculties.ToList();
            List<WebApp.Models.StudentMessage> messages = faculties.Find(x => x.Email.Equals(fac)).messages.ToList();
            return View(messages);
        }
    }
}