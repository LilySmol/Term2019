using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TErm.Models;

namespace Term3.Models
{
    public class UserNounsModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public List<IssuesModel> similarIssuesList { get; set; } 
        public int estimateIssueTime { get; set; }
        
        public UserNounsModel(int userId, string userName, List<IssuesModel> similarIssuesList)
        {
            this.userId = userId;
            this.userName = userName;
            this.similarIssuesList = similarIssuesList;
        }   
    }
}