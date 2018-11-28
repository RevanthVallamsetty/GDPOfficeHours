/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApp.Models
{

    // An entity, such as a user, group, or message.
    public class ResultsItem
    {

        // The ID and display name for the entity's radio button.
        public string Id { get; set; }
        public string Display { get; set; }

        // The properties of an entity that display in the UI.
        public Dictionary<string, object> Properties;

        public ResultsItem()
        {
            Properties = new Dictionary<string, object>();
        }
    }

    public class EventsItem
    {
        public string Id { get; set; }
        [Required]
        public string Subject { get; set; }
        public Location Location { get; set; }
        public List<Attendee> Attendees { get; set; }
        public ItemBody Body { get; set; }
        public DateTimeTimeZone StartTime { get; set; }
        public DateTimeTimeZone EndTime { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { set; get; }
        public System.DayOfWeek EventDay { get; set; }
        [Required]
        public string EventStart { get; set; }
        [Required]
        public string EventEnd { get; set; }
    }

    public class CaptureNoteModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string NoteLink { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Captured Date")]
        public DateTime CapturedDate { get; set; }
        public string Email { get; set; }
    }

    // View model to display a collection of one or more entities returned from the Microsoft Graph. 
    public class ResultsViewModel
    {

        // Set to false if you don't want to display radio buttons with the results.
        public bool Selectable { get; set; }

        // The list of entities to display.
        public IEnumerable<ResultsItem> Items { get; set; }
        public ResultsViewModel(bool selectable = true)
        {

            // Indicates whether the results should display radio buttons.
            // This is how an entity ID is passed to methods that require it.
            Selectable = selectable;

            Items = Enumerable.Empty<ResultsItem>();
        }
    }
}