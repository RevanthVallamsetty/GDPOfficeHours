using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class StudentMessagesController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();
        EventsService eventsService = new EventsService();

        // GET: StudentMessages
        public ActionResult Index()
        {
            var messages = db.messages.Include(s => s.faculty);
            return View(messages.ToList());
        }

        // GET: StudentMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentMessage studentMessage = db.messages.Find(id);
            if (studentMessage == null)
            {
                return HttpNotFound();
            }
            return View(studentMessage);
        }

        // GET: StudentMessages/Create
        public ActionResult Create()
        {
            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name");
            return View();
        }

        // POST: StudentMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "message")] StudentMessage studentMessage)
        {
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            var userDetails = await eventsService.GetMyDetails(graphClient);

            if (ModelState.IsValid)
            {
                studentMessage.Date_Created = DateTime.Now;
                studentMessage.Email = Session["facultyMail"].ToString();
                studentMessage.is_archived = false;
                studentMessage.student_id = userDetails.Mail;
                studentMessage.student_Name = userDetails.DisplayName;
                
                db.messages.Add(studentMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name", studentMessage.Email);
            return View(studentMessage);
        }

        // GET: StudentMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentMessage studentMessage = db.messages.Find(id);
            if (studentMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name", studentMessage.Email);
            return View(studentMessage);
        }

        // POST: StudentMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "Id,student_id,student_Name,message,Date_Created,is_archived,Email")] StudentMessage studentMessage)
        {
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            var userDetails = await eventsService.GetMyDetails(graphClient);

            if (ModelState.IsValid)
            {
                studentMessage.Date_Created = DateTime.Now;
                studentMessage.Email = Session["facultyMail"].ToString();
                studentMessage.is_archived = false;
                studentMessage.student_id = userDetails.Mail;
                studentMessage.student_Name = userDetails.DisplayName;

                db.Entry(studentMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name", studentMessage.Email);
            return View(studentMessage);
        }

        // GET: StudentMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentMessage studentMessage = db.messages.Find(id);
            if (studentMessage == null)
            {
                return HttpNotFound();
            }
            return View(studentMessage);
        }

        // POST: StudentMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentMessage studentMessage = db.messages.Find(id);
            db.messages.Remove(studentMessage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
