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
using System.Linq;

namespace WebApp.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        EventsService eventsService = new EventsService();

        // GET: StudentAppointment/Create
        public ActionResult Create()
        {
            return View();
        }


        // Create an event.
        // This snippet creates an hour-long event three days from now. 
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Subject,EventDate,EventStart,EventEnd,Location")] EventsItem eventsItem)
        {
            EventsItem results = new EventsItem();
            try
            {
                var subject = Request.Form["Subject"];
                var eventDate = Request.Form["EventDate"];
                var eventStartTime = Request.Form["EventStart"];
                var eventEndTime = Request.Form["EventEnd"];
                var location = Request.Form["Location"];

                var eventdatetime = DateTime.Parse(eventDate);
                var startTimespan = DateTime.ParseExact(eventStartTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                var endTimespan = DateTime.ParseExact(eventEndTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                // List of attendees
                List<Attendee> attendees = new List<Attendee>();
                attendees.Add(new Attendee
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = Session["facultyMail"].ToString()
                    },
                    Type = AttendeeType.Required
                });

                var s = eventsItem;

                results = new EventsItem()
                {
                    Subject = subject,
                    StartTime = new DateTimeTimeZone
                    {
                        DateTime = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, startTimespan.Hour,
                                                startTimespan.Minute, startTimespan.Second).ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    },
                    EndTime = new DateTimeTimeZone
                    {
                        DateTime = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, endTimespan.Hour,
                                                endTimespan.Minute, endTimespan.Second).ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    },
                    Attendees = attendees,
                    Location = new Location
                    {
                        DisplayName = location,
                    }
                };

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Create the event.
                results = await eventsService.CreateStudentEvent(graphClient, results);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return RedirectToAction("Index", "Student");
        }

        // Get a specified event.
        public async Task<ActionResult> Details(string id)
        {
            EventsItem results = new EventsItem();
            try
            {

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Get the event.
                results = await eventsService.GetEvent(graphClient, id);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return View(results);
        }


        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            EventsItem results = new EventsItem();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            // Get the event.
            results = await eventsService.GetEvent(graphClient, id);
            if (results == null)
            {
                return HttpNotFound();
            }

            return View(results);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit()
        {
            EventsItem results = new EventsItem();
            try
            {
                var id = Request.Form["Id"];
                var subject = Request.Form["Subject"];
                var eventDate = Request.Form["EventDate"];
                var eventStartTime = Request.Form["EventStart"];
                var eventEndTime = Request.Form["EventEnd"];

                var eventdatetime = DateTime.Parse(eventDate);
                var startTimespan = DateTime.ParseExact(eventStartTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                var endTimespan = DateTime.ParseExact(eventEndTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                results = new EventsItem()
                {
                    Id = id,
                    Subject = subject,
                    StartTime = new DateTimeTimeZone
                    {
                        DateTime = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, startTimespan.Hour,
                                                startTimespan.Minute, startTimespan.Second).ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    },
                    EndTime = new DateTimeTimeZone
                    {
                        DateTime = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, endTimespan.Hour,
                                                endTimespan.Minute, endTimespan.Second).ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    },
                };

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Update the event.
                results = await eventsService.UpdateEvent(graphClient, id, results);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return RedirectToAction("Index", "Student");
        }

        // Delete an event.
        public async Task<ActionResult> DeleteEvent(string id)
        {
            EventsItem results = new EventsItem();
            try
            {

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Delete the event.
                results = await eventsService.DeleteEvent(graphClient, id);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return View(results);
        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            EventsItem results = new EventsItem();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            // Get the event.
            results = await eventsService.GetEvent(graphClient, id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return View(results);
        }

        // POST: Department/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete()
        {
            EventsItem results = new EventsItem();
            try
            {
                var id = Request.Form["Id"];
                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Delete the event.
                results = await eventsService.DeleteEvent(graphClient, id);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return RedirectToAction("Index", "Student");
        }

        // Get events.
        public async Task<ActionResult> Index()
        {
            List<EventsItem> results = new List<EventsItem>();
            List<EventsItem> filteredResults = new List<EventsItem>();
            try
            {

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Get events.
                results = await eventsService.GetMyAppointments(graphClient);

                if(results != null && results.Any())
                {
                    foreach(EventsItem item in results)
                    {
                        foreach(Attendee attendee in item.Attendees)
                        {
                            if (attendee.EmailAddress.Address.Equals(Session["facultyMail"].ToString()))
                                filteredResults.Add(item);
                        }
                    }
                }
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });

            }
            return View("Index",filteredResults);
        }

        // Get user's calendar view.
        public async Task<ActionResult> GetMyCalendarView()
        {
            ResultsViewModel results = new ResultsViewModel();
            try
            {
                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Get a calendar view.
                results.Items = await eventsService.GetMyCalendarView(graphClient);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return View("Events", results);
        }
    }
}