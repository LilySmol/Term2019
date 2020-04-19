using EP.Morph;
using EP.Ner;
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
                string issueText = String.Concat(issue.title.ToLower(), " ", issue.description.ToLower());               
                List<string> issueWordsList = getHandledTextList(issueText);
                //issueWordsList.RemoveAll(l => l.Length < 4 && l != "бд");
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

        public List<string> getHandledTextList(string text)
        {
            List<string> textInInfinitiveList = new List<string>();
            Sdk.Initialize(MorphLang.RU | MorphLang.EN);
            var textHandled = ProcessorService.EmptyProcessor.Process(new SourceOfAnalysis(text));
            for (Token t = textHandled.FirstToken; t != null; t = t.Next)
            {
                if (t.Morph.Class.IsAdjective || t.Morph.Class.IsAdverb || t.Morph.Class.IsConjunction || t.Morph.Class.IsPreposition || t.Morph.Class.IsVerb)
                {
                    continue;
                }
                textInInfinitiveList.Add(t.GetNormalCaseText(null, true).ToLower());
            }
            return textInInfinitiveList;
        }

        public string getHandledTextString(string text)
        {
            string textInInfinitiveString = "";
            //Sdk.Initialize(MorphLang.RU | MorphLang.EN);
            var textHandled = ProcessorService.EmptyProcessor.Process(new SourceOfAnalysis(text));
            for (Token t = textHandled.FirstToken; t != null; t = t.Next)
            {
                if (t.Morph.Class.IsAdjective || t.Morph.Class.IsAdverb || t.Morph.Class.IsConjunction || t.Morph.Class.IsPreposition || t.Morph.Class.IsVerb)
                {
                    continue;
                }
                string noun = t.GetNormalCaseText(null, true).ToLower();
                if (!textInInfinitiveString.Contains(noun)){
                    textInInfinitiveString += noun + " ";
                }               
            }
            return textInInfinitiveString;
        }

        public List<IssuesModel> getSimilarIssues(List<IssuesModel> issues, List<string> nounsList)
        {
            List<IssuesModel> similarIssuesList = new List<IssuesModel>();
            
            foreach (IssuesModel issue in issues)
            {
                string handledIssueNounsString = getHandledTextString(issue.title + " " + issue.description);
                for (int i = 0; i < nounsList.Count; i++)
                {
                    if (handledIssueNounsString.Contains(nounsList[i]))
                    {
                        similarIssuesList.Add(issue);
                        break;
                    }
                }
            }

            return similarIssuesList;
        }
    }
}