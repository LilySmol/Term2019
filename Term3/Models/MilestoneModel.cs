using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TErm.Models
{
    public class MilestoneModel
    {
        public int id { get; set; }
        public int iid { get; set; }
        public int project_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string state { get; set; }
        public DateTime due_date { get; set; }
        public DateTime start_date { get; set; } 
      
    }
}