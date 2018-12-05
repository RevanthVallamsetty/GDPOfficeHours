using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class HomeService
    {

        public ExchangeService Service(string email,string password)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
            service.Credentials = new WebCredentials(email, password);
            service.Url = new Uri("https://outlook.office365.com/ews/exchange.asmx");
            return service;
        }

        private CalendarFolder FindDefaultCalendarFolder(string email, string password)
        {
            try
            {
                return CalendarFolder.Bind(Service(email, password), WellKnownFolderName.Calendar, new PropertySet());
            }
            catch
            {
                throw;
            }
        }


        private CalendarFolder FindNamedCalendarFolder(string name, string email, string password)
        {
            CalendarFolder calendar;
            try
            {
                FolderView view = new FolderView(100);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                view.PropertySet.Add(FolderSchema.DisplayName);
                view.Traversal = FolderTraversal.Deep;

                SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(FolderSchema.FolderClass, "IPF.Appointment");

                FindFoldersResults findFolderResults = Service(email, password).FindFolders(WellKnownFolderName.Root, sfSearchFilter, view);
                calendar = findFolderResults.Where(f => f.DisplayName.ToUpper().Equals(name.ToUpper())).Cast<CalendarFolder>().FirstOrDefault();
                if (calendar != null)
                {
                    return calendar;
                }
                else
                {
                    return CalendarFolder.Bind(Service(email, password), WellKnownFolderName.Calendar, new PropertySet());
                }               
            }
            catch
            {
                throw;
            }
        }

        public List<EventsItem> LoadAppointments(string email, string password)
        {
            List<EventsItem> eventsItems = new List<EventsItem>();

            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);

            try
            {
                CalendarFolder calendar = FindNamedCalendarFolder("officehours", email, password);  // or 
                //CalendarFolder calendar = FindDefaultCalendarFolder(email, password);

                CalendarView cView = new CalendarView(startDate, endDate, 50);
                cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.When, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Id);
                FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);

                if (appointments != null && appointments.Any())
                {
                    foreach (var appt in appointments)
                    {
                        eventsItems.Add(
                            new EventsItem
                            {
                                Id = appt.Id.ToString(),
                                Subject = appt.Subject,
                                EventDate = DateTime.Parse(appt.Start.ToString()).Date,
                                EventDay = DateTime.Parse(appt.Start.ToString()).DayOfWeek,
                                EventStart = DateTime.Parse(appt.Start.ToString()).ToString("t"),
                                EventEnd = DateTime.Parse(appt.End.ToString()).ToString("t"),
                            });
                    }
                }

                return eventsItems;
            }
            catch
            {
                throw;
            }

        }

        public List<JsonEventsSchedule> GetAppointments(string email, string password)
        {
            List<JsonEventsSchedule> eventsItems = new List<JsonEventsSchedule>();

            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);

            try
            {
                CalendarFolder calendar = FindNamedCalendarFolder("officehours", email, password);  // or 
                //CalendarFolder calendar = FindDefaultCalendarFolder(email, password);

                CalendarView cView = new CalendarView(startDate, endDate, 50);
                cView.PropertySet = new PropertySet(AppointmentSchema.Subject, AppointmentSchema.When, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Id);
                FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);

                if (appointments != null && appointments.Any())
                {
                    foreach (var appt in appointments)
                    {
                        eventsItems.Add(
                            new JsonEventsSchedule
                            {
                                id = appt.Id.ToString(),
                                title = appt.Subject,
                                start = ConvertToiso8601(appt.Start),
                                end = ConvertToiso8601(appt.End),
                            });
                    }
                }

                return eventsItems;
            }
            catch
            {
                throw;
            }

        }

        public string ConvertToiso8601(DateTime date)
        {
            // Your input
            DateTime dt = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                date.Millisecond, DateTimeKind.Utc);

            // ISO8601 with 7 decimal places
            string s1 = dt.ToString("o", CultureInfo.InvariantCulture);
            //=> "2017-06-26T20:45:00.0700000Z"

            // ISO8601 with 3 decimal places
            string s2 = dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture);
            //=> "2017-06-26T20:45:00.070Z"

            return s2;
        }

    }
}