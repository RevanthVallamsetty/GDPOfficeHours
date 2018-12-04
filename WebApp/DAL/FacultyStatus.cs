using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.DAL
{
    public class FacultyStatus
    {
        [Key, Column(TypeName = "varchar")]
        public String Status { get; set; }
    }
}