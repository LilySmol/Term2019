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
using System.Web.UI;
using System.Resources;
using System.Reflection;
using Term3.Models;

namespace TErm.Helpers.Integration
{
    public class GitLabParser: Requests, IParsing
    {
        static ResourceManager resource = new ResourceManager("TErm3.Resource", Assembly.GetExecutingAssembly());
        public string baseUrl = resource.GetString("baseUrl");
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Возвращает список проектов по privateToken и имени пользователя.
        /// </summary>
        public List<ProjectModel> getProjectsListByPrivateToken(string privateToken, string userName)
        {
            //try
            //{
                string response = get(privateToken, baseUrl + "/api/v4/users/" + userName + "/projects");
                List<ProjectModel> projectList = JsonConvert.DeserializeObject<List<ProjectModel>>(response);
                foreach (ProjectModel project in projectList)
                {
                    project.issuesList = getIssuesListByPrivateToken(privateToken, project._links.issues + "?per_page=100");
                }
                return projectList;
            //}
            //catch (NullReferenceException e) 
            //{               
            //    logger.Error(e.ToString());
            //    return null;
            //}
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

        /// <summary>
        /// Возвращает список задач по privateToken и ссылке на задачи проекта.
        /// </summary>
        public List<IssuesModel> getAllIssues(string privateToken, int userId)
        {
            string response = get(privateToken, baseUrl + "/api/v4/issues?assignee_id=" + userId);
            List<IssuesModel> issuesList = JsonConvert.DeserializeObject<List<IssuesModel>>(response);
            return issuesList;
        }

        /// <summary>
        /// Возвращает список участников проекта.
        /// </summary>
        public List<AssigneesModel> getProjectMembers(string privateToken, int projectId)
        {
            string response = get(privateToken, baseUrl + "/api/v4/projects/" + projectId + "/members");
            List<AssigneesModel> projectList = JsonConvert.DeserializeObject<List<AssigneesModel>>(response);
            return projectList;
        }
    }
}