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
using TErm.Helpers.Integration;
using TErm.Models;
using Term3.Helpers.DataBase;
using Term3.Models;

namespace Term3.Controllers
{
    public class ProjectController : Controller
    {
        private static int userId = 0;        
        private ProjectListModel projects = new ProjectListModel();  

        // GET: Project
        public ActionResult Projects(int userID)
        {
            userId = userID;
            fillProjects();
            return View(projects);
        }

        [HttpPost]
        public ActionResult Projects(ProjectListModel projects, string action)
        {
            if (action == "Обновить проекты")
            {
                DataBaseHelper.update(userId);
                fillProjects();
                return View(projects);
            }     
            return RedirectToAction("Issues", "Issue", new { userID = userId, projectId = action });
        }

        private void fillProjects()
        {
            projects.projects = new List<ProjectModel>();
            DataTable projectsTable = new DataTable();
            if (userId != 0) //пользователь есть в базе данных
            {
                projectsTable = DataBaseRequest.getProjects(userId);
            }
            for (int i = 0; i < projectsTable.Rows.Count; i++)
            {
                projects.projects.Add(new ProjectModel(Convert.ToInt32(projectsTable.Rows[i]["projectID"]), projectsTable.Rows[i]["name"].ToString()));
            }
        }
    }
}