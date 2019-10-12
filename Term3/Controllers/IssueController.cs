using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using TErm.Helpers.Clustering;
using TErm.Helpers.DataBase;
using TErm.Models;
using Term3.Helpers.DataBase;

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
        public ActionResult Issues(int userID, string projectTitle)
        {
            DataTable issuesTable = DataBaseRequest.getIssues(userID, projectTitle);
            project.issuesList = new List<IssuesModel>();
            project.name = projectTitle;
            userId = userID;
            double estimateTime = 0;
            double spentTime = 0;
            foreach (DataRow row in issuesTable.Rows)
            {
                spentTime = Convert.ToDouble(row["spentTime"]) / 3600;
                estimateTime = Convert.ToDouble(row["estimateTime"]) / 3600;
                project.issuesList.Add(new IssuesModel(Convert.ToInt32(row["issueID"]), row["title"].ToString(), row["description"].ToString(), spentTime, estimateTime));
            }
            project.projectTime = DataBaseRequest.getProjectTime(project.name, userId);
            return View(project);
        }

        [HttpPost]
        public ActionResult Issues()
        {
            createClusters();
            prognosis();
            foreach (IssuesModel issue in project.issuesList)
            {
                double estimateTime = issue.time_stats.time_estimate * 3600;
                DataBaseRequest.updateEstimateTime(issue.id, estimateTime);
            }
            DataBaseRequest.updateProjectTime(project.name, project.projectTime, userId);
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
    }
}