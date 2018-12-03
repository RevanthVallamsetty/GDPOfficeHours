using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class FacultyMessagesController : Controller
    {
        private OfficeHoursContext db = new OfficeHoursContext();

        // GET: FacultyMessages
        public ActionResult Index()
        {
            List<StudentMessage> studentMessages = new List<StudentMessage>();
            var messages = db.messages.ToList();
            foreach (var msg in messages)
            {
                if (msg.Email.Equals(Session["facultyMail"].ToString()))
                {
                    studentMessages.Add(msg);
                }
            }
            return View(studentMessages);
        }

        // GET: FacultyMessages/Details/5
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
            studentMessage.is_archived = true;

            db.Entry(studentMessage).State = EntityState.Modified;
            db.SaveChanges();
            return View(studentMessage);
        }

        // GET: FacultyMessages/Create
        public ActionResult Create()
        {
            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name");
            return View();
        }

        // POST: FacultyMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,student_id,student_Name,message,Date_Created,is_archived,Email")] StudentMessage studentMessage)
        {
            if (ModelState.IsValid)
            {
                db.messages.Add(studentMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name", studentMessage.Email);
            return View(studentMessage);
        }

        // GET: FacultyMessages/Edit/5
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

        // POST: FacultyMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,student_id,student_Name,message,Date_Created,is_archived,Email")] StudentMessage studentMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.faculties, "Email", "first_Name", studentMessage.Email);
            return View(studentMessage);
        }

        // GET: FacultyMessages/Delete/5
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

        // POST: FacultyMessages/Delete/5
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
