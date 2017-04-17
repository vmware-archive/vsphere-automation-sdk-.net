/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.vm.hardware
{
    using CommandLine;
    using System;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter.vm.hardware;

    /// <summary>
    /// Demonstrates how to configure the settings used when booting a virtual
    /// machine.
    ///
    /// Sample Prerequisites: The sample needs an existing VM.
    /// </summary>
    public class BootConfiguration : SamplesBase
    {
        private string vmId;
        private BootTypes.Info originalBootInfo;
        private Boot bootService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which boot options needs to be "
            + "configured.", Required = true)]
        public string VmName { get; set; }


        public override void Run()
        {
            this.bootService = VapiAuthHelper.StubFactory.CreateStub<Boot>(
                SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId +
                ") for boot configuration sample");

            // Print the current boot configuration
            Console.WriteLine("\n\n#### Print the original Boot Info");
            BootTypes.Info bootInfo = this.bootService.Get(this.vmId);
            Console.WriteLine(bootInfo);

            // Save the current boot info to revert settings after cleanup
            this.originalBootInfo = bootInfo;

            Console.WriteLine("\n\n#### Example: Update firmware to EFI for " +
                "boot configuration");
            BootTypes.UpdateSpec bootUpdateSpec = new BootTypes.UpdateSpec();
            bootUpdateSpec.SetType(BootTypes.Type.EFI);
            this.bootService.Update(this.vmId, bootUpdateSpec);
            bootInfo = this.bootService.Get(this.vmId);

            Console.WriteLine("\n\n#### Example: Update boot firmware to tell "
                + "it to enter setup mode on next boot.");
            bootUpdateSpec = new BootTypes.UpdateSpec();
            bootUpdateSpec.SetEnterSetupMode(true);
            this.bootService.Update(this.vmId, bootUpdateSpec);
            Console.WriteLine(bootUpdateSpec);
            bootInfo = this.bootService.Get(this.vmId);
            Console.WriteLine(bootInfo);

            Console.WriteLine("\n\n#### Example: Update firmware to introduce "
                + "a delay in boot process and automatically reboot after a "
                + "failure to boot, retry delay = 30000 ms");
            bootUpdateSpec = new BootTypes.UpdateSpec();
            bootUpdateSpec.SetDelay(10000L);
            bootUpdateSpec.SetRetry(true);
            bootUpdateSpec.SetRetryDelay(30000L);
            this.bootService.Update(this.vmId, bootUpdateSpec);
            bootInfo = this.bootService.Get(this.vmId);
            Console.WriteLine(bootInfo);
        }

        public override void Cleanup()
        {
            Console.WriteLine("\n\n#### Cleanup: Revert the boot "
                + "configuration");
            BootTypes.UpdateSpec bootUpdateSpec =
                new BootTypes.UpdateSpec();
            bootUpdateSpec.SetDelay(this.originalBootInfo.GetDelay());
            bootUpdateSpec.SetEfiLegacyBoot(
                this.originalBootInfo.GetEfiLegacyBoot());
            bootUpdateSpec.SetEnterSetupMode(
                this.originalBootInfo.GetEnterSetupMode());
            bootUpdateSpec.SetNetworkProtocol(
                this.originalBootInfo.GetNetworkProtocol());
            bootUpdateSpec.SetRetry(this.originalBootInfo.GetRetry());
            bootUpdateSpec.SetRetryDelay(
                this.originalBootInfo.GetRetryDelay());
            bootUpdateSpec.SetType(this.originalBootInfo.Get_Type());
            this.bootService.Update(this.vmId, bootUpdateSpec);
            BootTypes.Info bootInfo = this.bootService.Get(this.vmId);
            Console.WriteLine(bootInfo);
        }

        public static void Main(string[] args)
        {
            new BootConfiguration().Execute(args);
        }
    }
}
