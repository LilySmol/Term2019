using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TErm.Helpers.Clustering
{
    public class Clustering
    {
        private List<ClusterCenter> centersList;
        private List<ClusterObject> objectsList;        
        private int countClusters;
        private List<Cluster> clusterList;
        public List<Cluster> ClusterList
        {
            get { return clusterList; }
            set { clusterList = value; }
        }

        public Clustering() { }

        /// <summary>
        /// Осуществляется инициализация списка объектов кластеризации и количества кластеров.
        /// </summary>
        public Clustering(List<ClusterObject> objectsList, int countClusters)
        {
            this.objectsList = objectsList;
            this.countClusters = countClusters;
            centersList = new List<ClusterCenter>();
            clusterList = new List<Cluster>();
        }

        /// <summary>
        /// Осуществляется начальная инициализация списка центров кластеров.
        /// </summary>
        public void initializationClusterCenters()
        {
            int step = objectsList.Count / countClusters;
            int numberCenterCluster = step;
            for (int i = 0; i < objectsList.Count; i++)
            {
                if (i == numberCenterCluster)
                {
                    double[] clasterObjectArray = new double[objectsList[i].AttributeArray.Count()];
                    Array.Copy(objectsList[i].AttributeArray, clasterObjectArray, objectsList[i].AttributeArray.Count());
                    centersList.Add(new ClusterCenter("center" + (i + 1), clasterObjectArray));
                    numberCenterCluster += step;
                }                
            }
        }

        /// <summary>
        /// Расчитывает евклидово расстояние между объектом и центром.
        /// </summary>
        public double getDistance(ClusterObject clusterObject, ClusterCenter clusterCenter)
        {
            double distance = 0;
            for (int i = 0; i < clusterObject.AttributeArray.Count(); i++)
            {
                distance += Math.Pow(clusterObject.AttributeArray[i] - clusterCenter.AttributeArray[i], 2);
            }
            return Math.Sqrt(distance);
        }

        /// <summary>
        /// Перерасчет вектора признаков центра. Возвращает новый центр.
        /// </summary>
        public ClusterCenter recalculationCenter(ClusterObject clusterObject, ClusterCenter clusterCenter)
        {
            double[] newAttributeArray = new double[clusterCenter.AttributeArray.Count()];
            for (int i = 0; i < clusterCenter.AttributeArray.Count(); i++)
            {
                newAttributeArray[i] = (clusterObject.AttributeArray[i] + clusterCenter.AttributeArray[i]) / 2;
            }
            for (int j = 0; j < clusterCenter.AttributeArray.Count(); j++)
            {
                clusterCenter.AttributeArray[j] = newAttributeArray[j];
            }
            return clusterCenter;
        }

        /// <summary>
        /// Возвращает номер ближайшего к объекту центра
        /// </summary>
        public int getNumberNearestCenter(ClusterObject clusterObject)
        {
            double distance = getDistance(clusterObject, centersList[0]);
            double newDistance = 0;
            int numberCenter = 0;
            for (int i = 0; i < centersList.Count; i++)
            {
                newDistance = getDistance(clusterObject, centersList[i]);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    numberCenter = i;
                }
            }
            return numberCenter;
        }

        /// <summary>
        /// Уточнение центров
        /// </summary>
        public void clarifyCenters()
        {
            foreach (ClusterObject clusterObject in objectsList)
            {
                int numberCenter = getNumberNearestCenter(clusterObject);
                centersList[numberCenter] = recalculationCenter(clusterObject, centersList[numberCenter]);
            }
        }

        /// <summary>
        /// Кластеризация
        /// </summary>
        public void clustering()
        {
            int numberIteration = 0;
            List<string> changeList = new List<string>();
            string stringChange;
            bool flag = true;
            while (flag)
            {
                stringChange = "";
                clarifyCenters();
                foreach (ClusterObject clusterObject in objectsList)
                {
                    stringChange += centersList[getNumberNearestCenter(clusterObject)].ClusterName;
                }
                changeList.Add(stringChange);
                if (numberIteration > 0)
                {
                    if (changeList[numberIteration] == changeList[numberIteration - 1]) //центры кластеров сформировались окончательно
                    {
                        flag = false;
                        foreach (ClusterCenter clusterCenter in centersList)
                        {
                            List<ClusterObject> clusterObjects = getClusterObjectList(clusterCenter);
                            if (clusterObjects.Count != 0)
                            {
                                ClusterObject clusterObject = getNearestObject(clusterCenter, clusterObjects);
                                clusterList.Add(new Cluster(clusterCenter, clusterObjects, clusterObject));
                            }                            
                        }
                    }
                }
                numberIteration++;
            }
        }

        /// <summary>
        /// Возвращает список объектов указанного центра кластера
        /// </summary>
        public List<ClusterObject> getClusterObjectList(ClusterCenter clusterCenter)
        {
            List<ClusterObject> clusterObjects = new List<ClusterObject>();
            foreach (ClusterObject clusterObject in objectsList)
            {
                if (centersList[getNumberNearestCenter(clusterObject)].ClusterName == clusterCenter.ClusterName)
                {
                    clusterObjects.Add(clusterObject);
                }
            }
            return clusterObjects;
        }

        /// <summary>
        /// Возвращает ближайший к центру объект из списка объектов этого центра
        /// </summary>
        public ClusterObject getNearestObject(ClusterCenter clusterCenter, List<ClusterObject> clusterObjects)
        {
            double distance = getDistance(clusterObjects[0], clusterCenter);
            double newDistance = 0;
            ClusterObject clusterObject = new ClusterObject();
            for (int i = 0; i < clusterObjects.Count; i++)
            {
                newDistance = getDistance(clusterObjects[i], clusterCenter);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    clusterObject = clusterObjects[i];
                }
            }
            return clusterObject;
        }
    }
}