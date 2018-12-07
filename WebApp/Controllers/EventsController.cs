using Microsoft.Graph;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        EventsService eventsService = new EventsService();
        private OfficeHoursContext db = new OfficeHoursContext();
       
        // Get events.
        public async Task<ActionResult> Index()
        {
            List<EventsItem> results = new List<EventsItem>();
            try
            {

                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Get events.
                results = await eventsService.GetMyEvents(graphClient);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });

            }
            return View(results);
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

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }


        // Create an event.
        // This snippet creates an hour-long event three days from now. 
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Subject,EventDate,EventStart,EventEnd")] EventsItem eventsItem)
        {
            EventsItem results = new EventsItem();
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            var userDetails = await eventsService.GetMyDetails(graphClient);

            try
            {
                var subject = Request.Form["Subject"];
                var eventDate = Request.Form["EventDate"];
                var eventStartTime = Request.Form["EventStart"];
                var eventEndTime = Request.Form["EventEnd"];

                var eventdatetime = DateTime.Parse(eventDate);
                var startTimespan = DateTime.ParseExact(eventStartTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                var endTimespan = DateTime.ParseExact(eventEndTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                var s = eventsItem;

                OfficeSchedule officeSchedule = new OfficeSchedule
                {
                    end_time = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, endTimespan.Hour,
                                                endTimespan.Minute, endTimespan.Second),
                    start_time = new DateTime(eventdatetime.Year, eventdatetime.Month, eventdatetime.Day, startTimespan.Hour,
                                                startTimespan.Minute, startTimespan.Second),
                    Subject = subject,                    
                    Email = userDetails.Mail ?? userDetails.UserPrincipalName,
                };

                //var mail = await eventsService.GetMyEmailAddress(graphClient);

                try
                {
                    db.officeSchedule.Add(officeSchedule);
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

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
                };

                // Create the event.
                results = await eventsService.CreateEvent(graphClient, results);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return RedirectToAction("Index", "Events").Success("Event Created");
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
            return RedirectToAction("Index", "Events").Success("Event Edited");
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
            return RedirectToAction("Index", "Events").Success("Event Deleted");
        }

        // Accept a meeting request.
        // If the current user is the organizer of the meeting, the snippet will not work since organizers can't accept their
        // own invitations.
        public async Task<ActionResult> AcceptMeetingRequest(string id)
        {
            ResultsViewModel results = new ResultsViewModel(false);
            try
            {
                // Initialize the GraphServiceClient.
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                // Accept the meeting.
                results.Items = await eventsService.AcceptMeetingRequest(graphClient, id);
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