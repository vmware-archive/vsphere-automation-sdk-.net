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

    public class DatacenterHelper
    {
        /// <summary>
        /// Returns the identifier of the datacenter.
        ///
        /// Note: The method assumes that there is only one datacenter with the
        /// specified name
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <returns>identifier of a datacenter</returns>
        public static String GetDatacenter(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName)
        {
            Datacenter datacenterService =
                stubFactory.CreateStub<Datacenter>(sessionStubConfig);
            HashSet<String> datacenterNames = new HashSet<String>
            {
                datacenterName
            };
            DatacenterTypes.FilterSpec dcFilterSpec =
                new DatacenterTypes.FilterSpec();
            dcFilterSpec.SetNames(datacenterNames);
            List<DatacenterTypes.Summary> dcSummaries =
                datacenterService.List(dcFilterSpec);

            if (dcSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one datacenter" +
                    " with the specified name {0} exist", datacenterName));
            }

            if (dcSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Datacenter with name {0}" +
                    " not found !", datacenterName));
            }

            return dcSummaries[0].GetDatacenter();
        }
    }
}
