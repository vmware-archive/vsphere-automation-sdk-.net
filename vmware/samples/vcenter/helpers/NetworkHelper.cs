
/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.helpers
{
    using System;
    using System.Collections.Generic;
    using vapi.bindings;
    using vmware.vcenter;

    public class NetworkHelper
    {
        /// <summary>
        /// Returns the identifier of a standard network
        ///
        /// Note: The method assumes that there is only one standard portgroup
        ///  and datacenter with the specified names.
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="stdPortgroupName">name of the standard portgroup</param>
        /// <returns>identifier of the standard network</returns>
        public static string GetStandardNetworkBacking(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string stdPortgroupName)
        {
            HashSet<string> datacenters = new HashSet<string>
            {
                DatacenterHelper.GetDatacenter(
                    stubFactory, sessionStubConfig, datacenterName)
            };
            NetworkTypes.FilterSpec networkFilterSpec =
                new NetworkTypes.FilterSpec();
            networkFilterSpec.SetNames(
                new HashSet<string> { stdPortgroupName });
            networkFilterSpec.SetDatacenters(datacenters);
            networkFilterSpec.SetTypes(new HashSet<NetworkTypes.Type>
            {
                NetworkTypes.Type.STANDARD_PORTGROUP
            });

            Network networkService =
                stubFactory.CreateStub<Network>(sessionStubConfig);
            List<NetworkTypes.Summary> networkSummaries =
                networkService.List(networkFilterSpec);

            if (networkSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one standard " +
                    " portgroup with the specified name {0} exist",
                    stdPortgroupName));

            }
            if (networkSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Standard portgroup with " +
                                    "name {0} not found !", stdPortgroupName));
            }

            return networkSummaries[0].GetNetwork();
        }

        /// <summary>
        /// Returns the identifier of a distributed network
        ///
        /// Note: The method assumes that there is only one distributed portgroup
        ///  and datacenter with the specified names.
        /// </summary>
        /// <param name="serviceManager">Helper for instantiating vapi services</param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="distPortgroupName">name of the distributed portgroup</param>
        /// <returns>identifier of the distributed network</returns>
        public static string GetDistributedNetworkBacking(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string distPortgroupName)
        {
            HashSet<string> datacenters = new HashSet<string>
            {
                DatacenterHelper.GetDatacenter(
                    stubFactory, sessionStubConfig, datacenterName)
            };
            NetworkTypes.FilterSpec networkFilterSpec =
                new NetworkTypes.FilterSpec();
            networkFilterSpec.SetNames(
                new HashSet<string> { distPortgroupName });
            networkFilterSpec.SetDatacenters(datacenters);
            networkFilterSpec.SetTypes(new HashSet<NetworkTypes.Type>
            {
                NetworkTypes.Type.DISTRIBUTED_PORTGROUP
            });
            Network networkService =
                stubFactory.CreateStub<Network>(sessionStubConfig);
            List<NetworkTypes.Summary> networkSummaries =
                networkService.List(networkFilterSpec);

            if (networkSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one distributed" +
                    " portgroup with the specified name {0} exist",
                    distPortgroupName));

            }

            if (networkSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Distributed portgroup " +
                                    "with name {0} not found !",
                                    distPortgroupName));
            }

            return networkSummaries[0].GetNetwork();
        }
    }
}
