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
    /// Demonstrates how to configure the memory related settings of a virtual
    /// machine.
    ///
    /// Sample Prerequisites: This sample needs an existing VM.
    /// </summary>
    public class MemoryConfiguration : SamplesBase
    {
        private string vmId;
        private MemoryTypes.Info originalMemoryInfo;
        private Memory memoryService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which memory needs to be "
            + "configured.", Required = true)]
        public string VmName { get; set; }

        public override void Run()
        {
            this.memoryService =
                VapiAuthHelper.StubFactory.CreateStub<Memory>(
                    SessionStubConfiguration);

            Console.WriteLine("\n\n#### Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId +
                " ) for memory configuration sample.");

            // Get the current memory info
            Console.WriteLine("\n\n#### Print original memory info.");
            MemoryTypes.Info memoryInfo = memoryService.Get(this.vmId);
            Console.WriteLine(memoryInfo);

            /*
             * Save the current memory info to verify that we have cleaned up
             * properly.
             */
            this.originalMemoryInfo = memoryInfo;

            Console.WriteLine("\n\n#### Example: Update memory size field of "
            + "memory configuration.");
            MemoryTypes.UpdateSpec memoryUpdateSpec =
                new MemoryTypes.UpdateSpec();
            memoryUpdateSpec.SetSizeMiB(8 * 1024L);
            memoryService.Update(this.vmId, memoryUpdateSpec);
            Console.WriteLine(memoryUpdateSpec);
            memoryInfo = memoryService.Get(this.vmId);
            Console.WriteLine(memoryInfo);

            Console.WriteLine("\n\n#### Example: Update hot add enabled field "
            + "of memory configuration.");
            memoryUpdateSpec = new MemoryTypes.UpdateSpec();
            memoryUpdateSpec.SetHotAddEnabled(true);
            memoryService.Update(this.vmId, memoryUpdateSpec);
            Console.WriteLine(memoryUpdateSpec);
            memoryInfo = memoryService.Get(this.vmId);
            Console.WriteLine(memoryInfo);
        }

        public override void Cleanup()
        {
            Console.WriteLine("\n\n#### Cleanup:Revert memory configuration.");
            MemoryTypes.UpdateSpec memoryUpdateSpec =
                new MemoryTypes.UpdateSpec();
            memoryUpdateSpec.SetHotAddEnabled(
                this.originalMemoryInfo.GetHotAddEnabled());
            memoryUpdateSpec.SetSizeMiB(this.originalMemoryInfo.GetSizeMiB());
            Console.WriteLine(memoryUpdateSpec);
            MemoryTypes.Info memoryInfo = memoryService.Get(this.vmId);
            Console.WriteLine(memoryInfo);
        }

        public static void Main(string[] args)
        {
            new MemoryConfiguration().Execute(args);
        }
    }
}
