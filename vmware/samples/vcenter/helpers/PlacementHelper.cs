
/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.helpers
{
    using System;
    using vmware.vapi.bindings;
    using vmware.vcenter;

    public class PlacementHelper
    {
        /// <summary>
        /// Returns a VM placement spec for a cluster. Ensures that the
        /// cluster, resource pool, vm folder and datastore are all in the same
        /// datacenter which is specified.
        ///
        /// Note: The method assumes that there is only one of each resource
        /// type (i.e.datacenter, resource pool, cluster, folder, datastore)
        /// with the specified names.
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="clusterName">name of the cluster</param>
        /// <param name="folderName">name of the folder</param>
        /// <param name="datastoreName">name of the datastore</param>
        /// <returns>a VM placement spec for the specified cluster</returns>
        public static VMTypes.PlacementSpec GetPlacementSpecForCluster(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string clusterName, string folderName,
            string datastoreName)
        {

            string clusterId =
                    ClusterHelper.GetCluster(stubFactory, sessionStubConfig,
                    datacenterName, clusterName);
            Console.WriteLine("Selecting cluster " + clusterName + "(id=" +
                              clusterId + ")");

            string folderId = FolderHelper.GetFolder(stubFactory,
                sessionStubConfig, datacenterName, folderName);
            Console.WriteLine("Selecting folder " + folderName + "(id=" +
                              folderId + ")");

            string datastoreId =
            DatastoreHelper.GetDatastore(stubFactory, sessionStubConfig,
            datacenterName, datastoreName);
            Console.WriteLine("Selecting datastore " + datastoreName + "(id=" +
                              datastoreId + ")");

            /*
             *  Create the vm placement spec with the datastore, resource pool,
             *  cluster and vm folder
             */
            VMTypes.PlacementSpec vmPlacementSpec =
                new VMTypes.PlacementSpec();
            vmPlacementSpec.SetDatastore(datastoreId);
            vmPlacementSpec.SetCluster(clusterId);
            vmPlacementSpec.SetFolder(folderId);

            return vmPlacementSpec;
        }
    }
}
