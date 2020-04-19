using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Term3.Models
{
    public class AssigneesModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string state { get; set; }
        public string avatar_url { get; set; }
        public string web_url { get; set; }

        public double estimateTime { get; set; }

        public AssigneesModel() { }

        public AssigneesModel(int id, string name, string username, string state)
        {
            this.id = id;
            this.name = name;        
            this.username = username;
            this.state = state;
        }
    }
}