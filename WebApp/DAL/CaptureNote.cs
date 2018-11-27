using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class CaptureNote
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string NoteLink { get; set; }
        public DateTime CapturedDate { get; set; }
        public string Email { get; set; }

        public virtual Faculty faculty { get; set; }
    }
}