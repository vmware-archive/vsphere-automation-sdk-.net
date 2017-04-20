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

    public class EmbeddedPscSsoWorkflow : SamplesBase
    {
        public static readonly String SSO_PATH = "/sts/STSService";

        public override void Run()
        {
            System.Net.ServicePointManager.SecurityProtocol |=
                System.Net.SecurityProtocolType.Tls12;

            Console.WriteLine("\n\n#### Example: Login to vCenter server with "
                              + "embedded Platform Services Controller");

            VapiAuthenticationHelper vapiAuthHelper =
                new VapiAuthenticationHelper();

            /*
             * Since the platform services controller is embedded, the sso
             * server is the same as the vcenter server.
             */
            String ssoUrl = "https://" + Server + SSO_PATH;

            SetupSslTrustForServer();

            Console.WriteLine("\nStep 1: Connect to the Single Sign-On URL "
                + "and retrieve the SAML bearer token.");
            SamlToken samlBearerToken = SsoHelper.GetSamlBearerToken(ssoUrl,
                UserName, Password);

            Console.WriteLine("\nStep 2. Login to vAPI services using the "
                + "SAML bearer token.");
            StubConfiguration sessionStubConfig =
                    vapiAuthHelper.LoginBySamlBearerToken(Server,
                        samlBearerToken);

            Console.WriteLine("\nStep 3: Perform certain tasks using the vAPI "
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
            EmbeddedPscSsoWorkflow embeddedPscSsoWorkflow =
                new EmbeddedPscSsoWorkflow();
            if (Parser.Default.ParseArguments(args, embeddedPscSsoWorkflow))
            {
                embeddedPscSsoWorkflow.Run();
            }
        }
    }
}
