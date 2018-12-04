﻿using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
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
                calendar = findFolderResults.Where(f => f.DisplayName == name).Cast<CalendarFolder>().FirstOrDefault();
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
    }
}