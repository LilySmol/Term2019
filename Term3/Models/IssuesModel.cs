using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TErm.Models
{
    public class IssuesModel
    {
        public int id { get; set; }          //общее id задачи
        public int iid { get; set; }         //id задачи
        public string title { get; set; }
        public string description { get; set; }
        public TimeStatsModel time_stats { get; set; }

        public IssuesModel(int id, string title, string description, double spendTime, double estimateTime)
        {
            this.id = id;
            time_stats = new TimeStatsModel();
            this.title = title;
            this.description = description;
            time_stats.total_time_spent = spendTime;
            time_stats.time_estimate = estimateTime;
        }
    }
}