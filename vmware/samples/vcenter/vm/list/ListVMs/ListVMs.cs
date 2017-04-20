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
namespace vmware.samples.vcenter.vm.list
{
    using common.authentication;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.vcenter;

    /// <summary>
    /// Demonstrates getting list of VMs present in vCenter
    ///
    /// Sample Prerequisites:
    /// vCenter Server
    /// </summary>
    public class ListVMs : SamplesBase
    {
        private VM vmService;

        public override void Run()
        {
            // Login
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);

            this.vmService =
                VapiAuthHelper.StubFactory.CreateStub<VM>(
                    SessionStubConfiguration);
            List<VMTypes.Summary> vmList =
                this.vmService.List(new VMTypes.FilterSpec());
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("List of VMs");
            foreach(VMTypes.Summary vmsummary in vmList)
            {
                Console.WriteLine(vmsummary);
            }
            Console.WriteLine("---------------------------------------------");
        }

        public override void Cleanup()
        {
            VapiAuthHelper.Logout();
        }

        public static void Main(string[] args)
        {
            new ListVMs().Execute(args);
        }
    }
}
