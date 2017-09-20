/**
 * *******************************************************
 * Copyright VMware, Inc. 2017.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */
namespace vmware.samples.vcenter.sample_template
{
    using common.authentication;
    using System;
    using common;

    class SampleTemplate : SamplesBase
    {
        public override void Run()
        {
            // Login
            Console.WriteLine("Connecting...");
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);
            //TODO Add customer's custom code
        }

        public override void Cleanup()
        {
            Console.WriteLine("Disconnecting...");
            VapiAuthHelper.Logout();
            Console.WriteLine("Successfully disconnected");
        }
        public static void Main(string[] args)
        {
            new SampleTemplate().Execute(args);
            Console.WriteLine("Press <Enter> to exit...");
            Console.Read();
        }
    }
}
