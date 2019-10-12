using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TErm.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TErm.Helpers;
using NLog;

namespace TErm.Helpers.Integration
{
    public class GitLabParser: Requests, IParsing
    {
        public string baseUrl { get; set; }
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Возвращает список проектов по privateToken и имени пользователя.
        /// </summary>
        public List<ProjectModel> getProjectsListByPrivateToken(string privateToken, string userName)
        {
            try
            {
                string response = get(privateToken, baseUrl + "/api/v4/users/" + userName + "/projects");
                List<ProjectModel> projectList = JsonConvert.DeserializeObject<List<ProjectModel>>(response);
                foreach (ProjectModel project in projectList)
                {
                    project.issuesList = getIssuesListByPrivateToken(privateToken, project._links.issues + "?per_page=100");
                }
                return projectList;
            }
            catch (NullReferenceException e) 
            {
                logger.Error(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Возвращает список задач по privateToken и ссылке на задачи проекта.
        /// </summary>
        public List<IssuesModel> getIssuesListByPrivateToken(string privateToken, string linkIssues)
        {
            string response = get(privateToken, linkIssues);
            List<IssuesModel> issuesList = JsonConvert.DeserializeObject<List<IssuesModel>>(response);
            return issuesList;
        }
    }
}