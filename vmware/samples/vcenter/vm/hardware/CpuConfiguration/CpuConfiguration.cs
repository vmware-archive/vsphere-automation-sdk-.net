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
namespace vmware.samples.vcenter.vm.hardware
{
    using CommandLine;
    using System;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter.vm.hardware;

    /// <summary>
    /// Demonstrates how to configure a CPU for a VM.
    ///
    /// Sample Prerequisites: The sample needs an existing VM.
    /// </summary>
    public class CpuConfiguration : SamplesBase
    {
        private string vmId;
        private CpuTypes.Info originalCpuInfo;
        private Cpu cpuService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which CPU settings needs to be "
            + "configured.", Required = true)]
        public string VmName { get; set; }


        public override void Run()
        {
            this.cpuService = VapiAuthHelper.StubFactory.CreateStub<Cpu>(
                SessionStubConfiguration);

            Console.WriteLine("\n\n### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId
                + " ) for CPU configuration sample.");

            Console.WriteLine("\n\n### Example: Print original cpu info");
            CpuTypes.Info cpuInfo = cpuService.Get(this.vmId);
            Console.WriteLine(cpuInfo);

            /*
             * Save the current cpu info to verify that we have cleaned up
             * properly
             */
            this.originalCpuInfo = cpuInfo;

            Console.WriteLine("\n\n### Example: Update count field of CPU " +
                "configuration");
            CpuTypes.UpdateSpec cpuUpdateSpec = new CpuTypes.UpdateSpec();
            cpuUpdateSpec.SetCount(2L);
            cpuService.Update(this.vmId, cpuUpdateSpec);
            Console.WriteLine(cpuUpdateSpec);
            cpuInfo = cpuService.Get(this.vmId);
            Console.WriteLine(cpuInfo);

            Console.WriteLine("\n\n### Example: Update cpu fields, number of "
                + "cores per socket=2, enable hot add");
            cpuUpdateSpec = new CpuTypes.UpdateSpec();
            cpuUpdateSpec.SetCoresPerSocket(2L);
            cpuUpdateSpec.SetHotAddEnabled(true);
            cpuService.Update(this.vmId, cpuUpdateSpec);
            Console.WriteLine(cpuUpdateSpec);
            cpuInfo = this.cpuService.Get(this.vmId);
            Console.WriteLine(cpuInfo);
        }

        public override void Cleanup()
        {
            Console.WriteLine("\n\n### Cleanup: Revert the CPU configuration");
            CpuTypes.UpdateSpec cpuUpdateSpec = new CpuTypes.UpdateSpec();
            cpuUpdateSpec.SetCoresPerSocket(
                this.originalCpuInfo.GetCoresPerSocket());
            cpuUpdateSpec.SetCount(this.originalCpuInfo.GetCount());
            cpuUpdateSpec.SetHotAddEnabled(
                this.originalCpuInfo.GetHotAddEnabled());
            cpuUpdateSpec.SetHotRemoveEnabled(
                this.originalCpuInfo.GetHotRemoveEnabled());
            cpuService.Update(this.vmId, cpuUpdateSpec);
            Console.WriteLine(cpuUpdateSpec);
            CpuTypes.Info cpuInfo = cpuService.Get(this.vmId);
            Console.WriteLine("VM ID = " + this.vmId);
            Console.WriteLine(cpuInfo);
        }

        public static void Main(string[] args)
        {
            new CpuConfiguration().Execute(args);
        }
    }
}
