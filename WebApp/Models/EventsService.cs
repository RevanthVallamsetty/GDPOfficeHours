/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Graph;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp_OpenIDConnect_DotNet.Models
{
    public class EventsService
    {
        // Get events in all the current user's mail folders.
        public async Task<List<EventsItem>> GetMyEvents(GraphServiceClient graphClient)
        {
            List<EventsItem> items = new List<EventsItem>();

            // Get events.
            IUserEventsCollectionPage events = await graphClient.Me.Events.Request().GetAsync();

            if (events?.Count > 0)
            {
                foreach (Event current in events)
                {
                    items.Add(new EventsItem
                    {                        
                        Id = current.Id,
                        Body = current.Body,
                        Attendees = current.Attendees.ToList(),
                        Location = current.Location,
                        EndTime = current.End,
                        StartTime = current.Start,
                        Subject = current.Subject,
                        EventDate = DateTime.Parse(current.Start.DateTime.ToString()).Date,
                        EventDay = DateTime.Parse(current.Start.DateTime.ToString()).DayOfWeek,
                        EventStart = DateTime.Parse(current.Start.DateTime.ToString()).ToString("t"),
                        EventEnd = DateTime.Parse(current.End.DateTime.ToString()).ToString("t"),
                    });
                }
            }
            return items;
        }

        // Get user's calendar view.
        // This snippets gets events for the next seven days.
        public async Task<List<ResultsItem>> GetMyCalendarView(GraphServiceClient graphClient)
        {
            List<ResultsItem> items = new List<ResultsItem>();

            // Define the time span for the calendar view.
            List<QueryOption> options = new List<QueryOption>();
            options.Add(new QueryOption("startDateTime", DateTime.Now.ToString("o")));
            options.Add(new QueryOption("endDateTime", DateTime.Now.AddDays(7).ToString("o")));

            ICalendarCalendarViewCollectionPage calendar = await graphClient.Me.Calendar.CalendarView.Request(options).GetAsync();

            if (calendar?.Count > 0)
            {
                foreach (Event current in calendar)
                {
                    items.Add(new ResultsItem
                    {
                        Display = current.Subject,
                        Id = current.Id
                    });
                }
            }
            return items;
        }

        // Create an event.
        // This snippet creates an hour-long event three days from now. 
        public async Task<EventsItem> CreateEvent(GraphServiceClient graphClient, EventsItem eventsItem)
        {
            EventsItem items = new EventsItem();

            // Add the event.
            Event createdEvent = await graphClient.Me.Events.Request().AddAsync(new Event
            {
                Subject = eventsItem.Subject,
                Start = eventsItem.StartTime,
                End = eventsItem.EndTime,
                Attendees = eventsItem.Attendees,
                Location = eventsItem.Location,
                IsAllDay = false
            });

            if (createdEvent != null)
            {
                // Get updated event properties.
                items = new EventsItem()
                {
                    Id = createdEvent.Id,
                    Body = createdEvent.Body,
                    Attendees = createdEvent.Attendees.ToList(),
                    Location = createdEvent.Location,
                    EndTime = createdEvent.End,
                    StartTime = createdEvent.Start,
                    Subject = createdEvent.Subject,
                    EventDate = DateTime.Parse(createdEvent.Start.DateTime.ToString()).Date,
                    EventDay = DateTime.Parse(createdEvent.Start.DateTime.ToString()).DayOfWeek,
                    EventStart = DateTime.Parse(createdEvent.Start.DateTime.ToString()).ToString("t"),
                    EventEnd = DateTime.Parse(createdEvent.End.DateTime.ToString()).ToString("t"),
                };
            }
            return items;
        }

        // Get a specified event.
        public async Task<EventsItem> GetEvent(GraphServiceClient graphClient, string id)
        {
            EventsItem items = new EventsItem();

            // Get the event.
            Event retrievedEvent = await graphClient.Me.Events [id].Request().GetAsync();
                
            if (retrievedEvent != null)
            {
                items = new EventsItem()
                {
                    Id = retrievedEvent.Id,
                    Body = retrievedEvent.Body,
                    Attendees = retrievedEvent.Attendees.ToList(),
                    Location = retrievedEvent.Location,
                    EndTime = retrievedEvent.End,
                    StartTime = retrievedEvent.Start,
                    Subject = retrievedEvent.Subject,
                    EventDate = DateTime.Parse(retrievedEvent.Start.DateTime.ToString()).Date,
                    EventDay = DateTime.Parse(retrievedEvent.Start.DateTime.ToString()).DayOfWeek,
                    EventStart = DateTime.Parse(retrievedEvent.Start.DateTime.ToString()).ToString("t"),
                    EventEnd = DateTime.Parse(retrievedEvent.End.DateTime.ToString()).ToString("t"),
                };

            }
            return items;
        }


        // Update an event. 
        // This snippets updates the event subject, time, and attendees.
        public async Task<EventsItem> UpdateEvent(GraphServiceClient graphClient, string id, EventsItem eventsItem)
        {
            EventsItem items = new EventsItem();

            // Get the current list of attendees, and then add an attendee.
            Event originalEvent = await graphClient.Me.Events[id].Request().Select("attendees").GetAsync();
            List<Attendee> attendees = originalEvent.Attendees as List<Attendee>;
           
            // Update the event.
            Event updatedEvent = await graphClient.Me.Events[id].Request().UpdateAsync(new Event
            {
                Subject = eventsItem.Subject,
                Attendees = attendees,
                Start = eventsItem.StartTime,
                End = eventsItem.EndTime,
                IsAllDay = false
            });

            if (updatedEvent != null)
            {
                // Get updated event properties.
                items = new EventsItem()
                {
                    Id = updatedEvent.Id,
                    Body = updatedEvent.Body,
                    Attendees = updatedEvent.Attendees.ToList(),
                    Location = updatedEvent.Location,
                    EndTime = updatedEvent.End,
                    StartTime = updatedEvent.Start,
                    Subject = updatedEvent.Subject,
                    EventDate = DateTime.Parse(updatedEvent.Start.DateTime.ToString()).Date,
                    EventDay = DateTime.Parse(updatedEvent.Start.DateTime.ToString()).DayOfWeek,
                    EventStart = DateTime.Parse(updatedEvent.Start.DateTime.ToString()).ToString("t"),
                    EventEnd = DateTime.Parse(updatedEvent.End.DateTime.ToString()).ToString("t"),
                };
            }
            return items;
        }

        // Delete a specified event.
        public async Task<EventsItem> DeleteEvent(GraphServiceClient graphClient, string id)
        {
            EventsItem items = new EventsItem();

            // Delete the event.
            await graphClient.Me.Events[id].Request().DeleteAsync();
          
            return items;
        }

        // Accept a meeting request.
        public async Task<List<ResultsItem>> AcceptMeetingRequest(GraphServiceClient graphClient, string id)
        {
            List<ResultsItem> items = new List<ResultsItem>();

            // This snippet first checks whether the selected event originates with an invitation from the current user. If it did, 
            // the SDK would throw an ErrorInvalidRequest exception because organizers can't accept their own invitations.
            Event myEvent = await graphClient.Me.Events[id].Request().Select("ResponseStatus").GetAsync();
            if (myEvent.ResponseStatus.Response != ResponseType.Organizer)
            {

                // Accept the meeting.
                await graphClient.Me.Events[id].Accept(Resource.GenericText).Request().PostAsync();

                items.Add(new ResultsItem
                {

                    // This operation doesn't return anything.
                    Properties = new Dictionary<string, object>
                    {
                        { Resource.No_Return_Data, "" }
                    }
                });
            }
            else
            {
                items.Add(new ResultsItem
                {

                    // Let the user know the operation isn't supported for this event.
                    Properties = new Dictionary<string, object>
                    {
                        { Resource.Event_CannotAcceptOwnMeeting, "" }
                    }
                });
            }
            return items;
        }
    }
}