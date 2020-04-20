using EP.Morph;
using EP.Ner;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TErm.Helpers.Clustering;
using TErm.Helpers.DataBase;
using TErm.Helpers.Integration;
using TErm.Models;
using Term3.Helpers.DataBase;
using Term3.Models;

namespace Term3.Controllers
{
    public class IssueController : Controller
    {
        static private ProjectModel project = new ProjectModel();
        static Clustering clustering = new Clustering();
        private Logger logger = LogManager.GetCurrentClassLogger();
        static ResourceManager resource = new ResourceManager("TErm3.Resource", Assembly.GetExecutingAssembly());
        static int userId = 0;

        // GET: Issue
        public ActionResult Issues(int userID, int projectId)
        {
            Sdk.Initialize(MorphLang.RU | MorphLang.EN);

            DataTable issuesTable = DataBaseRequest.getIssues(userID, projectId);
            project.issuesList = new List<IssuesModel>();
            project.id = projectId;
            userId = userID;

            DataTable assigneeTable = DataBaseRequest.getAssigneeIssue();

            double estimateTime = 0;
            double spentTime = 0;
            foreach (DataRow row in issuesTable.Rows)
            {
                spentTime = Convert.ToDouble(row["spentTime"]) / 3600;
                estimateTime = Convert.ToDouble(row["estimateTime"]) / 3600;

                List<AssigneesModel> assignees = new List<AssigneesModel>();
                foreach (DataRow assignee in assigneeTable.Rows)
                {
                    int issueId = Convert.ToInt32(assignee["issueID"]);
                    int issueId2 = Convert.ToInt32(row["issueID"]);
                    if (Convert.ToInt32(assignee["issueID"]) == Convert.ToInt32(row["issueID"])) {
                        assignees.Add(new AssigneesModel(Convert.ToInt32(assignee["assigneID"]), assignee["name"].ToString(), assignee["username"].ToString(), assignee["state"].ToString()));
                    }
                }

                project.issuesList.Add(new IssuesModel(Convert.ToInt32(row["issueID"]), row["title"].ToString(), row["description"].ToString(), spentTime, estimateTime, assignees));               
            }

            project.projectTime = DataBaseRequest.getProjectTime(project.name, userId);
            
            return View(project);
        }

        [HttpPost]
        public ActionResult Issues(string action)
        {
            if (action == "Получить рекомендацию")
            {            
                double projectEstimateTime = 0;
                List<string> assigneesList = new List<string>();
                string recomendation = "Наиболее подходящими разработчиками, способными выполнить все задачи проекта за минимальное время являются: ";
                foreach (IssuesModel issue in project.issuesList)
                {
                    List<AssigneesModel> assignees = getAssignee(issue.id);
                    issue.assignees = assignees;
                    if (assignees.Count > 0)
                    {
                        issue.time_stats.time_estimate = assignees[0].estimateTime;
                        projectEstimateTime += assignees[0].estimateTime;
                        assigneesList.Add(assignees[0].username);
                    }
                }
                project.projectTime = projectEstimateTime;

                foreach (string assignee in assigneesList.Distinct().ToList())
                {
                    recomendation += assignee + " ";
                }
                recomendation += "Оценочное время выполнения проекта в таком случае: " + projectEstimateTime;

                Response.Write("<script>alert('"+ recomendation +"')</script>");
            }
            else
            {            
                int issueId = Convert.ToInt32(action);
                List<AssigneesModel> assignees = getAssignee(issueId);
                project.issuesList.First(item => item.id == issueId).assignees = assignees;
                if (assignees.Count > 0)
                {
                    project.issuesList.First(item => item.id == issueId).time_stats.time_estimate = assignees[0].estimateTime;
                }
            }
            
            return View(project);
        }

        protected void prognosis()
        {
            if (userId != 0 && project.name != "")
            {
                InputDataConverter inputDataConverter = new InputDataConverter();
                foreach (IssuesModel issue in project.issuesList)
                {
                    Cluster clusterCenter = clustering.ClusterList[clustering.getNumberNearestCenter(inputDataConverter.convertToClusterObject(issue))];
                    issue.time_stats.time_estimate = clusterCenter.NearestObject.SpentTime / 3600;
                    project.projectTime += issue.time_stats.time_estimate;
                    logger.Info("Задача: " + issue.title + " Oтносится к кластеру: " + clusterCenter.NearestObject.Title);
                }                
            }
        }

        protected void createClusters()
        {
            int testProjectId = Convert.ToInt32(resource.GetString("testProjectId"));
            List<ProjectModel> projectList = DataBaseHelper.getProjectsList(resource.GetString("testProjectToken"), resource.GetString("testProjectUser"));
            InputDataConverter inputDataConverter = new InputDataConverter();
            var projectSelected = from project in projectList
                                  where project.id == testProjectId
                                  select project;
            ProjectModel projectWithTestData = projectSelected.ToList()[0];
            clustering = new Clustering(inputDataConverter.convertListToClusterObject(projectWithTestData.issuesList), 9);
            clustering.initializationClusterCenters();
            clustering.clustering();
        }

        protected List<AssigneesModel> getAssignee(int issueId)
        {
            List<UserNounsModel> users = new List<UserNounsModel>();
            GitLabParser gitLabParser = new GitLabParser();
            InputDataConverter inputDataConverter = new InputDataConverter();

            IssuesModel issue = project.issuesList.First(item => item.id == issueId);
            List<string> nounsList = inputDataConverter.getHandledTextList(issue.title + " " + issue.description).Distinct().ToList();

            DataTable user = DataBaseRequest.getUser(userId);
            List<AssigneesModel> projectMembers = gitLabParser.getProjectMembers(user.Rows[0]["token"].ToString(), project.id);                 

            foreach(AssigneesModel assignee in projectMembers)
            {
                List<IssuesModel> userIssues = gitLabParser.getAllIssues(user.Rows[0]["token"].ToString(), assignee.id);
                List<IssuesModel> similarIssuesList = inputDataConverter.getSimilarIssues(userIssues, nounsList);
                if (similarIssuesList.Count > 0)
                {
                    users.Add(new UserNounsModel(assignee.id, assignee.name, similarIssuesList));
                }
            }

            return getAssignee(users);
        }

        protected List<AssigneesModel> getAssignee(List<UserNounsModel> userNouns)
        {
            List<AssigneesModel> assigneeList = new List<AssigneesModel>();
            int maxIssuesCount = getMaxIssuesCount(userNouns);
            foreach(UserNounsModel user in userNouns)
            {
                if (user.similarIssuesList.Count == maxIssuesCount)
                {
                    AssigneesModel assignee = new AssigneesModel(user.userId, "", user.userName, "");
                    assignee.estimateTime = getEstimateIssueTime(user.similarIssuesList) / 3600;
                    assigneeList.Add(assignee);
                }
            }
            if (assigneeList.Count > 1)
            {
                return getAssigneesWithMinEstimateTime(assigneeList);
            }
            return assigneeList;
        }   

        protected List<AssigneesModel> getAssigneesWithMinEstimateTime(List<AssigneesModel> assigneeList)
        {
            double min = assigneeList[0].estimateTime;
            AssigneesModel assigneeWithMinEstimateTime = assigneeList[0];
            foreach (AssigneesModel assignee in assigneeList)
            {
                if (assignee.estimateTime < min)
                {
                    min = assignee.estimateTime;
                    assigneeWithMinEstimateTime = assignee;
                }
            }
            return new List<AssigneesModel>() { assigneeWithMinEstimateTime };
        }

        protected int getMaxIssuesCount(List<UserNounsModel> userNouns)
        {
            int maxCount = 0;
            foreach(UserNounsModel user in userNouns)
            {
                if (user.similarIssuesList.Count > maxCount)
                {
                    maxCount = user.similarIssuesList.Count;
                }
            }
            return maxCount;
        }

        protected double getEstimateIssueTime(List<IssuesModel> similarIssuesList)
        {
            double sum = 0;
            int count = 0;
            foreach (IssuesModel issue in similarIssuesList)
            {
                sum += issue.time_stats.total_time_spent;
                count++;
            }
            return (double)sum / count;
        }

        // оценить проект целиком (кластеризация)
        //    createClusters();
        //    prognosis();
        //    foreach (IssuesModel issue in project.issuesList)
        //    {
        //        double estimateTime = issue.time_stats.time_estimate * 3600;
        //        DataBaseRequest.updateEstimateTime(issue.id, estimateTime);
        //    }
        //    DataBaseRequest.updateProjectTime(project.name, project.projectTime, userId);
    }
}