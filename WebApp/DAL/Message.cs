using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class StudentMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string student_id { get; set; }
        public string student_Name { get; set; }
        public string message { get; set; }
        public DateTime? Date_Created { get; set; }
        public bool is_archived { get; set; }
        public string Email { get; set; }

        public virtual Faculty faculty { get; set; }
    }
}