using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class OfficeHoursContext: DbContext
    {
        public DbSet<OfficeHours> officeHours { get; set; }
        public DbSet<Faculty> faculties { get; set; }
    }
}