using Microsoft.Graph;
using WebApp.Helpers;
using WebApp.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.Globalization;
using WebApp.Models;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace WebApp.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        FilesService filesService = new FilesService();
        EventsService eventsService = new EventsService();
        private OfficeHoursContext db = new OfficeHoursContext();

        [HttpGet]
        public ActionResult Index()
        {
            Session["val"] = "";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string imageName)
        {
            byte[] byteArray;
            if (Session["dump"] == null)
            {
                return RedirectToAction("Index", "Error", new { message = string.Format("Try capturing the image again") });
            }
            byteArray = String_To_Bytes2(Session["dump"].ToString());
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            var userDetails = await eventsService.GetMyDetails(graphClient);
            var result = await filesService.CreateFile(graphClient, byteArray);
            try
            {
                CaptureNote captureNote = new CaptureNote()
                {
                    CapturedDate = DateTime.Now,
                    Email = Session["facultyMail"] != null ? Session["facultyMail"].ToString() : "",
                    NoteLink = result.Display,
                    StudentName = userDetails.DisplayName,                    
                };

                try
                {
                    db.captureNotes.Add(captureNote);
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }

            return RedirectToAction("Index", "Student");
        }

        [HttpGet]
        public ActionResult Changephoto()
        {
            if (Convert.ToString(Session["val"]) != string.Empty)
            {
                ViewBag.pic = "http://localhost:4279/WebImages/" + Session["val"].ToString();
            }
            else
            {
                ViewBag.pic = "../../WebImages/person.jpg";
            }
            return View();
        }
               
        public JsonResult Rebind()
        {
            string path = "http://localhost:4279/WebImages/" + Session["val"].ToString();

            return Json(path, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Capture()
        {
            var stream = Request.InputStream;
            string dump;

            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();

                DateTime nm = DateTime.Now;

                string date = nm.ToString("yyyymmddMMss");

                var path = Server.MapPath("~/WebImages/" + date + "test.jpg");

                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));

                Session["dump"] = dump;

                ViewData["path"] = date + "test.jpg";

                Session["val"] = date + "test.jpg";
            }

            return View("Index");
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }        
    }
}