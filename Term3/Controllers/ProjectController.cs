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

namespace Term3.Controllers
{
    public class ProjectController : Controller
    {
        private static int userId = 0;        
        private ProjectModel project = new ProjectModel();  

        // GET: Project
        public ActionResult Projects(int userID)
        {
            userId = userID;
            fillComboBox();
            return View(project);
        }

        [HttpPost]
        public ActionResult Projects(ProjectModel projectModel)
        { 
            if (projectModel.name == null) //обновить проекты и задачи пользователя
            {
                DataBaseHelper.update(userId);
                fillComboBox();
                return View(project);
            }      
            return RedirectToAction("Issues", "Issue", new { userID = userId, projectTitle = projectModel.name });
        }

        private void fillComboBox()
        {
            DataTable projectsTable = new DataTable();
            if (userId != 0) //пользователь есть в базе данных
            {
                projectsTable = DataBaseRequest.getProjects(userId);
            }
            project.projectsList = new List<SelectListItem>();
            for (int i = 0; i < projectsTable.Rows.Count; i++)
            {
                project.projectsList.Add(new SelectListItem { Text = projectsTable.Rows[i]["name"].ToString(), Value = projectsTable.Rows[i]["name"].ToString() });
            }
        }
    }
}