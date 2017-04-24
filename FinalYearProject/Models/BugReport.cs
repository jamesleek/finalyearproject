using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalYearProject.Models
{
    public class BugReport
    {
        public int Id { get; set; }
        public string BugDescription { get; set; }
        public string Category { get; set; }
        public bool isResolved { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserName { get; set; }
    }
}