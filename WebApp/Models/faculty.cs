using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    [Table("Faculty")]
    public class Faculty
    {
        [Key]
        public string faculty_email { get; set; }
        public string faculty_name { get; set; }
        public List<OfficeHours> officeHours { get; set; }
    }
}