/**
 * *******************************************************
 * Copyright VMware, Inc. 2013, 2016.  All Rights Reserved.
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
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using vmware.lookupservice;

    /// <summary>
    /// Lookup service connection.
    /// </summary>
    public class LookupServiceHelper
    {
        private string lsUrl;
        private LsPortTypeClient lsPortType;
        private LookupServiceContent serviceContent;

        public LookupServiceHelper(string lsUrl)
        {
            this.lsUrl = lsUrl;

            var binding = GetCustomBinding();
            var address = new EndpointAddress(lsUrl);
            lsPortType = new LsPortTypeClient(binding, address);

            var serviceInstanceRef = new ManagedObjectReference();
            serviceInstanceRef.type = "LookupServiceInstance";
            serviceInstanceRef.Value = "ServiceInstance";
            serviceContent = lsPortType.RetrieveServiceContent(
                serviceInstanceRef);
        }

        public string GetServiceEndpointUrl(string mgmtNodeId,
            string productType, string serviceType, string endpointType,
            string endpointProtocol)
        {
            var services = GetRegistrationInfos(mgmtNodeId, productType,
                serviceType, endpointType, endpointProtocol);
            if (services != null && services.Length != 0 &&
                services[0] != null && services[0].serviceEndpoints != null &&
                services[0].serviceEndpoints.Length != 0)
            {
                return services[0].serviceEndpoints[0].url;
            }

            var errorMsg = string.Format("Could not find endpoint URL for " +
                "service '{0}' and protocol '{1}'", serviceType, endpointType);
            throw new Exception(errorMsg);
        }

        public LookupServiceRegistrationInfo[] GetRegistrationInfos(
            string mgmtNodeId, string productType,
            string serviceType, string endpointType, string endpointProtocol)
        {
            var filterServiceType =
                new LookupServiceRegistrationServiceType();
            filterServiceType.product = productType;
            filterServiceType.type = serviceType;

            var filterEndpointType =
                    new LookupServiceRegistrationEndpointType();
            filterEndpointType.protocol = endpointProtocol;
            filterEndpointType.type = endpointType;

            var filterCriteria = new LookupServiceRegistrationFilter();
            filterCriteria.serviceType = filterServiceType;
            filterCriteria.endpointType = filterEndpointType;
            filterCriteria.nodeId = mgmtNodeId;

            var registrationInfos = lsPortType.List(
                serviceContent.serviceRegistration, filterCriteria);
            return registrationInfos;
        }

        public string FindSsoUrl()
        {
            return GetServiceEndpointUrl(null, "com.vmware.cis",
                "cs.identity", "com.vmware.cis.cs.identity.sso", "wsTrust");
        }

        public string FindVapiUrl(string mgmtNodeId)
        {
            return GetServiceEndpointUrl(mgmtNodeId,
                "com.vmware.cis", "cs.vapi", "com.vmware.vapi.endpoint",
                "vapi.json.https.public");
        }

        public string FindVimUrl(string mgmtNodeId)
        {
            return GetServiceEndpointUrl(mgmtNodeId,
                "com.vmware.cis", "vcenterserver", "com.vmware.vim", "vmomi");
        }

        public string FindVimPbmUrl(string mgmtNodeId)
        {
            return GetServiceEndpointUrl(mgmtNodeId,
                "com.vmware.vim.sms", "sms", "com.vmware.vim.pbm", "https");
        }

        public string GetManagementNodeId(ref string mgmtNodeName)
        {
            var services = GetRegistrationInfos(null,
                "com.vmware.cis", "vcenterserver", "com.vmware.vim", "vmomi");

            if (services != null && services.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(mgmtNodeName))
                {
                    // if management node name is not specified and there is
                    // only one management node, return that node
                    if (services.Length == 1)
                    {
                        // get management node name
                        mgmtNodeName = services[0].serviceAttributes.
                            FirstOrDefault(attr => attr.key ==
                                "com.vmware.vim.vcenter.instanceName").value;
                        return services[0].nodeId;
                    }
                    throw new Exception("There is more than one " +
                        "management node, specify the desired node name.");
                }

                /*
                 * Get the management node hostname.
                 *
                 * Note: This assumes that the vCenter server is setup with a
                 * DNS registration.
                 */
                mgmtNodeName =
                    System.Net.Dns.GetHostEntry(mgmtNodeName).HostName;
                foreach (var service in services)
                {
                    foreach (var serviceAttribute in service.serviceAttributes)
                    {
                        if (serviceAttribute.key.Equals(
                            "com.vmware.vim.vcenter.instanceName",
                            StringComparison.CurrentCultureIgnoreCase) &&
                            mgmtNodeName.Equals(serviceAttribute.value,
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            return service.nodeId;
                        }
                    }
                }
            }
            throw new Exception(string.Format(
                    "Could not find management node '{0}'", mgmtNodeName));
        }

        public void PrintAllServices()
        {
            var services = lsPortType.List(serviceContent.serviceRegistration,
                new LookupServiceRegistrationFilter());

            foreach (var service in services)
            {
                Console.WriteLine("Product: " + service.serviceType.product);
                Console.WriteLine("Service: " + service.serviceType.type);
                foreach (var endpoint in service.serviceEndpoints)
                {
                    Console.WriteLine("   Endpoint protocol: " +
                        endpoint.endpointType.protocol);
                    Console.WriteLine("   Endpoint URL: " +
                        endpoint.endpointType.protocol);
                }
            }
        }

        private static Binding GetCustomBinding()
        {
            var customBinding = new CustomBinding();

            var textMessageEncoding = new TextMessageEncodingBindingElement();
            textMessageEncoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 2147483647;

            customBinding.Elements.Add(textMessageEncoding);
            customBinding.Elements.Add(transport);

            return customBinding;
        }

        public override string ToString()
        {
            return string.Format("LS++ Connection ({0})", lsUrl);
        }
    }
}
