using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using TErm.Models;

namespace TErm.Helpers.DataBase
{
    public class DataBaseRequest
    {
        static ResourceManager resource = new ResourceManager("TErm3.Resource", Assembly.GetExecutingAssembly());
        private static string connectionString = resource.GetString("dbConnectionString");

        /// <summary>
        /// Удалить проект
        /// </summary>
        public static void deleteProject(int projectId)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Project WHERE projectID = " + projectId, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        public static void deleteIssue(int issueId)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Issue WHERE issueID = " + issueId, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Добавляет пользователя 
        /// </summary>
        public static void insertUser(string name, string token)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("INSERT INTO User(name, token) VALUES('" + name + "', '" + token + "')", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Добавляет проект 
        /// </summary>
        public static void insertProject(int projectId, string description, string name, int userId)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Project(projectID, description, name, userID) VALUES(" + projectId + ", '" + description + "', '" + name + "', " + userId + ")", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Добавляет задачу 
        /// </summary>
        public static void insertIssue(int issueId, int issueNumber, string title, string description, int projectId, double spentTime, double estimateTime)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Issue(issueID, issueNumber, title, description, projectID, spentTime, estimateTime) VALUES(" + issueId + ", " + issueNumber + ", '" + title + "', '" + description + "', " + projectId + ", " + spentTime + ", " + estimateTime +")", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Получить идентификатор пользователя по имени и токену
        /// </summary>
        public static int getUserId(string name, string token)
        {
            DataTable userTable = new DataTable();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("SELECT userID FROM User WHERE name = '" + name + "' AND token = '" + token + "'", connection);
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            userTable.Load(reader);
            connection.Close();
            connection.Dispose();
            if (userTable.Rows.Count == 0)
            {
                return 0;
            }
            return Convert.ToInt32(userTable.Rows[0][0]);
        }

        /// <summary>
        /// Получить пользователя по id
        /// </summary>
        public static DataTable getUser(int userId)
        {
            DataTable userTable = new DataTable();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM User WHERE userID = " + userId, connection);
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            userTable.Load(reader);
            connection.Close();
            connection.Dispose();
            return userTable;
        }

        /// <summary>
        /// Получить таблицу с проектами пользователя
        /// </summary>
        public static DataTable getProjects(int userId)
        {
            DataTable projectTable = new DataTable();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Project WHERE userID = " + userId, connection);
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            projectTable.Load(reader);
            connection.Close();
            connection.Dispose();
            return projectTable;
        }

        /// <summary>
        /// Получить задачи по идентификатору пользователя и имени проекта
        /// </summary>
        public static DataTable getIssues(int userId, string projectName)
        {
            DataTable issuesTable = new DataTable();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("SELECT Issue.title, Issue.description, Issue.spentTime, Issue.estimateTime, Issue.issueID FROM Project join Issue on Project.projectID = Issue.projectID WHERE Project.userID = " + userId + " AND Project.name = '" + projectName + "'", connection);
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            issuesTable.Load(reader);
            connection.Close();
            connection.Dispose();
            return issuesTable;
        }

        /// <summary>
        /// Обновить оценочное время задачи
        /// </summary>
        public static void updateEstimateTime(int issueID, double estimateTime)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("UPDATE Issue SET estimateTime = " + estimateTime + " WHERE issueID = " + issueID, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Обновить время проекта
        /// </summary>
        public static void updateProjectTime(string projectName, double projectTime, int userId)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("UPDATE Project SET projectTime = " + projectTime + " WHERE name = '" + projectName + "' AND userID = " + userId, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Получить время проекта
        /// </summary>
        public static double getProjectTime(string projectName, int userId)
        {
            double projectTime = 0;
            DataTable userTable = new DataTable();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand("SELECT projectTime FROM Project WHERE name = '" + projectName + "' AND userID = " + userId, connection);
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            userTable.Load(reader);
            connection.Close();
            connection.Dispose();
            if (userTable.Rows.Count == 0)
            {
                return 0;
            }
            try
            {
                projectTime = Convert.ToDouble(userTable.Rows[0][0]);
                return Convert.ToDouble(userTable.Rows[0][0]);
            }
            catch
            {
                return 0;
            }           
        }
    }
}