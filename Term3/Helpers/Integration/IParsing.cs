using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TErm.Models;

namespace TErm.Helpers.Integration
{
    interface IParsing
    {
        List<ProjectModel> getProjectsListByPrivateToken(string privateToken, string userName);
        List<IssuesModel> getIssuesListByPrivateToken(string privateToken, string linkIssues);
    }
}