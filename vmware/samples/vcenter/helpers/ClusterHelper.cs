/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.helpers
{
    using System;
    using System.Collections.Generic;
    using vmware.vapi.bindings;
    using vmware.vcenter;

    public class ClusterHelper
    {
        /// <summary>
        /// Returns the identifier of a cluster.
        ///
        /// Note: The method assumes that there is only one cluster and
        /// datacenter with the specified names
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="clusterName">name of the cluster</param>
        /// <returns>identifier of the cluster</returns>
        public static String GetCluster(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string clusterName)
        {

            HashSet<string> datacenters = new HashSet<string>
            {
                DatacenterHelper.GetDatacenter(stubFactory, sessionStubConfig,
                datacenterName)
            };

            ClusterTypes.FilterSpec clusterFilterSpec =
                new ClusterTypes.FilterSpec();
            HashSet<string> clusters = new HashSet<string> { clusterName };

            clusterFilterSpec.SetNames(clusters);
            clusterFilterSpec.SetDatacenters(datacenters);

            Cluster clusterService = stubFactory.CreateStub<Cluster>(
                sessionStubConfig);
            List<ClusterTypes.Summary> clusterSummaries =
                clusterService.List(clusterFilterSpec);

            if (clusterSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one cluster with"
                    + " the specified name {0} exist", clusterName));
            }

            if (clusterSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Cluster with name {0}" +
                                    " not found !", clusterName));
            }

            return clusterSummaries[0].GetCluster();
        }
    }
}