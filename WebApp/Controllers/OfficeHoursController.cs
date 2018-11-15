using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class OfficeHoursController : Controller
    {
        // GET: OfficeHours
        public ActionResult Index()
        {
            OfficeHoursContext officeHoursContext = new OfficeHoursContext();
            List<Faculty> faculties = officeHoursContext.faculties.ToList();
            List<OfficeSchedule> officeHours = faculties.First().Office_Hours.ToList();
            return View(officeHours);
        }
    }
}