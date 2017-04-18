/*
 * Copyright 2017 VMware, Inc.  All rights reserved.
 */

namespace vmware.samples.vcenter.vm.list
{
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
            // Cleanup after the sample run
        }

        public static void Main(string[] args)
        {
            new ListVMs().Execute(args);
        }
    }
}
