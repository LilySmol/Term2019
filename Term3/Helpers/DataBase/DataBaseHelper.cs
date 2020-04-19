using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using TErm.Helpers.DataBase;
using TErm.Helpers.Integration;
using TErm.Models;
using Term3.Models;

namespace Term3.Helpers.DataBase
{
    public class DataBaseHelper
    {       
        /// <summary>
        /// Обновляет проекты и задачи пользователя
        /// </summary>
        public static void update(int userId)
        {
            DataTable projects = DataBaseRequest.getProjects(userId);
            DataTable issues = new DataTable();
            foreach (DataRow project in projects.Rows)
            {
                issues = DataBaseRequest.getIssues(userId, Convert.ToInt32(project["projectId"]));
                foreach (DataRow issue in issues.Rows)
                {
                    DataBaseRequest.deleteIssue(Convert.ToInt32(issue["issueID"]));
                }
                DataBaseRequest.deleteProject(Convert.ToInt32(project["projectID"]));
            }

            DataTable user = DataBaseRequest.getUser(userId);
            List<ProjectModel> projectList = DataBaseHelper.getProjectsList(user.Rows[0]["token"].ToString(), user.Rows[0]["name"].ToString());
            addProjectsAndIssues(userId);
        }

        public static List<ProjectModel> getProjectsList(string token, string user)
        {
            GitLabParser gitLabParser = new GitLabParser();
            return gitLabParser.getProjectsListByPrivateToken(token, user);
        }

        public static int addUserData(UserModel user)
        {
            DataBaseRequest.insertUser(user.Name, user.Token);
            int userId = DataBaseRequest.getUserId(user.Name, user.Token);
            addProjectsAndIssues(userId);
            return userId;
        }

        public static void addProjectsAndIssues(int userId)
        {
            foreach (ProjectModel project in UserModel.Projects)
            {
                DataBaseRequest.insertProject(project.id, project.description, project.name, userId);
                foreach (IssuesModel issue in project.issuesList)
                {
                    DataBaseRequest.insertIssue(issue.id, issue.iid, issue.title, issue.description, project.id, issue.time_stats.total_time_spent, issue.time_stats.time_estimate);
                    if (issue.assignees != null)
                    {
                        foreach (AssigneesModel assignee in issue.assignees)
                        {
                            try
                            {
                                DataBaseRequest.insertAssigne(assignee.id, assignee.name, assignee.username, assignee.state, assignee.avatar_url, assignee.web_url);
                            }
                            catch (SQLiteException e)
                            {
                                e.ToString();
                                //если уже есть исполнитель в бд
                            }
                            DataBaseRequest.insertAssigneIssue(issue.id, assignee.id);
                        }
                    }
                }
            }            
        }
    }
}