using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CaptureController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();
        // GET: Capture
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "CaptureDate")] CaptureNote note)
        {
            var facultyMail = Session["facultyMail"].ToString();
            List<CaptureNoteModel> captureNoteModels = new List<CaptureNoteModel>();

            var captureNotes = db.captureNotes
                .Where(o => o.Email.Equals(facultyMail))
                .OrderBy(d => d.StudentName);


            foreach(var cn in captureNotes)
            {
                if (cn.CapturedDate.Equals(note.CapturedDate))
                {
                    captureNoteModels.Add(new CaptureNoteModel() {
                        CapturedDate = cn.CapturedDate,
                        Email = cn.Email,
                        Id = cn.Id,
                        NoteLink = cn.NoteLink,
                        StudentName = cn.StudentName
                    });
                }
            }

            return View(captureNoteModels);
        }
    }
}