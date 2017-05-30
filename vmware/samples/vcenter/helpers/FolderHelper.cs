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

    public class FolderHelper
    {
        /// <summary>
        /// Returns the identifier of a folder.
        ///
        /// Note: The method assumes that there is only one folder and
        ///  datacenter with the specified names.
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="datacenterName">name of the datacenter</param>
        /// <param name="folderName">name of the folder</param>
        /// <returns>identifier of a folder</returns>
        public static String GetFolder(
            StubFactory stubFactory, StubConfiguration sessionStubConfig,
            string datacenterName, string folderName)
        {
            HashSet<string> datacenters = new HashSet<string>
            {
                DatacenterHelper.GetDatacenter(
                    stubFactory, sessionStubConfig, datacenterName)
            };
            FolderTypes.FilterSpec folderFilterSpec =
                new FolderTypes.FilterSpec();
            folderFilterSpec.SetNames(new HashSet<String> { folderName });
            folderFilterSpec.SetDatacenters(datacenters);

            Folder folderService = stubFactory.CreateStub<Folder>(
                sessionStubConfig);
            List<FolderTypes.Summary> folderSummaries =
                folderService.List(folderFilterSpec);

            if (folderSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one folder" +
                    " with the specified name {0} exist", folderName));
            }

            if (folderSummaries.Count <= 0)
            {
                throw new Exception(String.Format("Folder with name {0}" +
                                    "not found !", folderName));
            }

            return folderSummaries[0].GetFolder();
        }
    }
}
