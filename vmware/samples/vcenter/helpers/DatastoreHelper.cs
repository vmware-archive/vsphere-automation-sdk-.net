/**
 * *******************************************************
 * Copyright VMware, Inc. 2016.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */
namespace vmware.samples.vcenter.helpers
{
    using System;
    using System.Collections.Generic;
    using vmware.vapi.bindings;
    using vmware.vcenter;

    public class DatastoreHelper
    {
        /// <summary>
        /// Returns the identifier of a datastore.
        ///
        /// Note: The method assumes that there is only one datacenter and
        /// datastore with the specified names.
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="datastoreName">name of the datastore</param>
        /// <returns>identifier of a datastore</returns>
        public static String GetDatastore(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string datastoreName)
        {
            HashSet<string> datacenters = new HashSet<string>
            {
                DatacenterHelper.GetDatacenter(
                    stubFactory, sessionStubConfig, datacenterName)
            };
            DatastoreTypes.FilterSpec dsFilterSpec =
                new DatastoreTypes.FilterSpec();
            dsFilterSpec.SetNames(new HashSet<string> { datastoreName });
            dsFilterSpec.SetDatacenters(datacenters);

            Datastore datastoreService =
                stubFactory.CreateStub<Datastore>(sessionStubConfig);
            List<DatastoreTypes.Summary> dsSummaries =
                datastoreService.List(dsFilterSpec);

            if (dsSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one datastore" +
                    " with the specified name {0} exist", datastoreName));
            }

            if (dsSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Datastore with name {0}" +
                                    "not found !", datastoreName));
            }

            return dsSummaries[0].GetDatastore();
        }
    }
}
