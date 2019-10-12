using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TErm.Models;

namespace TErm.Helpers.Clustering
{
    public class InputDataConverter
    {
        private static List<string> DICTIONARY = new List<string>();
        private static int COUNTWORDS = 0;

        /// <summary>
        /// Преобразует объекты списка IssuesModel в список ClusterObject
        /// </summary>
        public List<ClusterObject> convertListToClusterObject(List<IssuesModel> issuesModel)
        {
            if (DICTIONARY.Count == 0)
            {
                DICTIONARY = dictionaryInitialize(issuesModel);
                COUNTWORDS = DICTIONARY.Count();
            }            
            List<ClusterObject> clusterObject = new List<ClusterObject>();
            foreach (IssuesModel issue in issuesModel)
            {                
                clusterObject.Add(convertToClusterObject(issue));
            }
            return clusterObject;
        }

        /// <summary>
        /// Преобразует задачу в объект кластера
        /// </summary>
        public ClusterObject convertToClusterObject(IssuesModel issues)
        {
            double[] issueArray = new double[COUNTWORDS];
            string[] issueWordsArray = String.Concat(issues.title.ToLower(), " ", issues.description.ToLower()).Split(' ');
            for (int i = 0; i < COUNTWORDS; i++)
            {
                if (issueWordsArray.Contains(DICTIONARY[i]))
                {
                    issueArray[i] = 1;
                }
                else
                {
                    issueArray[i] = 0;
                }
            }
            return new ClusterObject(issues.iid.ToString(), issueArray, issues.title, issues.time_stats.total_time_spent, issues.time_stats.time_estimate);
        }

        /// <summary>
        /// Возвращает список словаря
        /// </summary>
        private List<string> dictionaryInitialize(List<IssuesModel> issuesModel)
        {
            List<string> totalWordsList = new List<string>();
            List<string> dictionary = new List<string>();
            foreach (IssuesModel issue in issuesModel)
            {
                List<string> issueWordsList = String.Concat(issue.title.ToLower(), " ", issue.description.ToLower()).Split(' ').ToList();
                issueWordsList.RemoveAll(l => l.Length < 4 && l != "бд");
                totalWordsList.AddRange(issueWordsList);
            }
            foreach (string word in totalWordsList)
            {
                if (totalWordsList.Count(l => l == word) > 2)
                {
                    dictionary.Add(word);
                }
            }
            return dictionary.Distinct().ToList();
        }


    }
}