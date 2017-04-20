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
namespace vmware.samples.common
{
    using authentication;
    using System;
    using System.Collections.Generic;
    using vmware.vim25;

    /// <summary>
    /// Helper for calling VIM APIs.
    /// </summary>
    public class VimHelper
    {
        public static ManagedObjectReference GetCluster(string clusterName,
            VimAuthenticationHelper vimConnection)
        {
            var vimPortType = vimConnection.VimPortType;
            var serviceContent = vimConnection.ServiceContent;
            var morefType = "ClusterComputeResource";
            var morefProperties = new string[] { "name" };
            var objectContents = new List<ObjectContent>();
            var containerView = vimPortType.CreateContainerView(
                new CreateContainerViewRequest(
                    serviceContent.viewManager,
                    serviceContent.rootFolder,
                    new string[] { morefType },
                    true));

            var spec = new PropertyFilterSpec();
            spec.propSet = new PropertySpec[] { new PropertySpec() };
            spec.propSet[0].all = morefProperties == null || morefProperties.Length == 0;
            spec.propSet[0].allSpecified = spec.propSet[0].all;
            spec.propSet[0].type = morefType;
            spec.propSet[0].pathSet = morefProperties;
            spec.objectSet = new ObjectSpec[] { new ObjectSpec() };

            var ts = new TraversalSpec();
            ts.name = "view";
            ts.path = "view";
            ts.skip = false;
            ts.type = "ContainerView";
            spec.objectSet[0].obj = containerView.returnval;
            spec.objectSet[0].selectSet = new SelectionSpec[] { ts };

            var result = vimPortType.RetrievePropertiesEx(
                new RetrievePropertiesExRequest(
                    serviceContent.propertyCollector,
                    new PropertyFilterSpec[] { spec },
                    new RetrieveOptions()));
            if (result != null)
            {
                var token = result.returnval.token;
                objectContents.AddRange(result.returnval.objects);
                while (!string.IsNullOrWhiteSpace(token))
                {
                    var retrieveResult = vimPortType.ContinueRetrievePropertiesEx(
                        serviceContent.propertyCollector, token);
                    if (retrieveResult != null)
                    {
                        token = retrieveResult.token;
                        objectContents.AddRange(retrieveResult.objects);
                    }
                }
            }
            foreach (var content in objectContents)
            {
                if (content.propSet[0].val.ToString() == clusterName)
                {
                    return content.obj;
                }
            }
            throw new Exception("Could not find cluster with name '" +
                clusterName + "'");
        }
    }
}
