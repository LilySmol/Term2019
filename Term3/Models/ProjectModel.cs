using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TErm.Models
{
    public class ProjectModel
    {
        public int id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public LinksModel _links { get; set; }
        public List<IssuesModel> issuesList { get; set; }
        public double projectTime { get; set; } //в секундах

        public List<SelectListItem> projectsList { get; set; }
    }
}