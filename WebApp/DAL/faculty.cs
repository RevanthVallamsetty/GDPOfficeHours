using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApp.DAL;

namespace WebApp.Models
{    
    public class Faculty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public string Password { get; set; }
        [Key, Column(TypeName = "varchar")]
        public string Email { get; set; }
        public string phone_number { get; set; }
        public string Status { get; set; }
        public virtual FacultyStatus facultyStatus { get; set; }
        public virtual ICollection<OfficeSchedule> Office_Hours { get; set; }
        public virtual ICollection<StudentMessage> messages { get; set; }
    }
}