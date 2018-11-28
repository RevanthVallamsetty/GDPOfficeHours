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
            var facultyMail = Session["facultyMail"].ToString();
            List<CaptureNoteModel> captureNoteModels = new List<CaptureNoteModel>();

            var captureNotes = db.captureNotes
                .Where(o => o.Email.Equals(facultyMail))
                .OrderBy(d => d.StudentName);


            foreach (var cn in captureNotes)
            {
                captureNoteModels.Add(new CaptureNoteModel()
                {
                    CapturedDate = cn.CapturedDate,
                    Email = cn.Email,
                    Id = cn.Id,
                    NoteLink = cn.NoteLink,
                    StudentName = cn.StudentName
                });
            }

            return View(captureNoteModels);
        }

        // Get a specified event.
        public ActionResult Details(int? id)
        {
            var facultyMail = Session["facultyMail"].ToString();
            CaptureNoteModel captureNoteModels = new CaptureNoteModel();

            var cn = db.captureNotes
                .Where(o => o.Id == id)
                .OrderBy(d => d.StudentName).FirstOrDefault();

            captureNoteModels = new CaptureNoteModel()
            {
                CapturedDate = cn.CapturedDate,
                Email = cn.Email,
                Id = cn.Id,
                NoteLink = cn.NoteLink,
                StudentName = cn.StudentName
            };


            return View(captureNoteModels);
        }
    }
}