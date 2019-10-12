using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TErm.Helpers.Clustering
{
    public class ClusterCenter
    {
        private string clusterCenterName;
        private double[] attributeArray;

        public string ClusterName
        {
            get { return clusterCenterName; }
            set { clusterCenterName = value; }
        }
        public double[] AttributeArray
        {
            get { return attributeArray; }
            set { attributeArray = value; }
        }

        public ClusterCenter(string clusterName, double[] attributeArray)
        {
            this.clusterCenterName = clusterName;
            this.attributeArray = attributeArray;
        }
    }
}