using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class OfficeHoursContext: DbContext
    {
        public OfficeHoursContext()
            : base("name=OfficeHoursContext")
        {
        }

        public DbSet<OfficeSchedule> officeSchedule { get; set; }
        public DbSet<Faculty> faculties { get; set; }
        public DbSet<Message> messages { get; set; }
    }
}