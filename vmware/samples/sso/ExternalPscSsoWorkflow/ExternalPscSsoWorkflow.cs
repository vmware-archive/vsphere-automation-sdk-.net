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
namespace vmware.samples.sso
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.samples.common.authentication;
    using vmware.vapi.bindings;
    using vmware.vapi.util.security;
    using vmware.vcenter;

    public class ExternalPscSsoWorkflow : SamplesBase
    {
        [Option("lookupserviceurl", HelpText = "url of the lookup service",
            Required = true)]
        public string LookupServiceUrl { get; set; }

        public override void Run()
        {
            System.Net.ServicePointManager.SecurityProtocol |=
                System.Net.SecurityProtocolType.Tls12;

            Console.WriteLine("\n\n#### Example: Login to vCenter server with "
                              + "external Platform Services Controller");

            VapiAuthenticationHelper vapiAuthHelper =
                new VapiAuthenticationHelper();

            SetupSslTrustForServer();

            Console.WriteLine("\nStep 1: Connect to the lookup service on the "
                               + "Platform Services Controller node.");
            LookupServiceHelper lookupServiceHelper = new LookupServiceHelper(
                LookupServiceUrl);

            Console.WriteLine("\nStep 2: Discover the Single Sign-On service "
                               + "URL from lookup service.");
            String ssoUrl = lookupServiceHelper.FindSsoUrl();

            Console.WriteLine("\nStep 3: Connect to the Single Sign-On URL and"
                   + " retrieve the SAML bearer token.");
            SamlToken samlBearerToken = SsoHelper.GetSamlBearerToken(ssoUrl,
                UserName, Password);

            Console.WriteLine("\nStep 4. Login to vAPI services using the "
                + "SAML bearer token.");
            StubConfiguration sessionStubConfig =
                    vapiAuthHelper.LoginBySamlBearerToken(Server,
                        samlBearerToken);

            Console.WriteLine("\nStep 5: Perform certain tasks using the vAPI "
                + "services.");
            Datacenter datacenterService =
                vapiAuthHelper.StubFactory.CreateStub<Datacenter>(
                    sessionStubConfig);
            List<DatacenterTypes.Summary> dcList =
                datacenterService.List(new DatacenterTypes.FilterSpec());
            Console.WriteLine("\nList of datacenters on the vcenter server:");
            foreach (DatacenterTypes.Summary dcSummary in dcList)
            {
                Console.WriteLine(dcSummary);
            }
            vapiAuthHelper.Logout();
        }

        public override void Cleanup()
        {
            // No cleanup required for the sample
        }

        public static void Main(string[] args)
        {
            ExternalPscSsoWorkflow externalPscSsoWorkflow =
                new ExternalPscSsoWorkflow();
            if (Parser.Default.ParseArguments(args, externalPscSsoWorkflow))
            {
                externalPscSsoWorkflow.Run();
            }
        }
    }
}
