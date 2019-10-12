using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TErm.Helpers.Clustering
{
    public class Cluster
    {        
        private ClusterCenter clusterCenter;
        private List<ClusterObject> clusterObject;
        private ClusterObject nearestObject;

        public ClusterCenter ClusterCenter
        {
            get { return clusterCenter; }
            set { clusterCenter = value; }
        }
        public List<ClusterObject> ClusterObject
        {
            get { return clusterObject; }
            set { clusterObject = value; }
        }
        public ClusterObject NearestObject
        {
            get { return nearestObject; }
            set { nearestObject = value; }
        }

        public Cluster(ClusterCenter clusterCenter, List<ClusterObject> clusterObject, ClusterObject estimateTime)
        {
            this.clusterCenter = clusterCenter;
            this.clusterObject = clusterObject;
            this.nearestObject = estimateTime;
        }
    }
}