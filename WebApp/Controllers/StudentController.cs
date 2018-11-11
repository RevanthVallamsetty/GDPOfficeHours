using Microsoft.Graph;
using WebApp_OpenIDConnect_DotNet.Helpers;
using WebApp_OpenIDConnect_DotNet.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.Globalization;

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
                        Address = "s530458@nwmissouri.edu"
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
                results = await eventsService.CreateEvent(graphClient, results);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();

                // Personal accounts that aren't enabled for the Outlook REST API get a "MailboxNotEnabledForRESTAPI" or "MailboxNotSupportedForRESTAPI" error.
                return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            }
            return RedirectToAction("Index", "Events");
        }
    }
}