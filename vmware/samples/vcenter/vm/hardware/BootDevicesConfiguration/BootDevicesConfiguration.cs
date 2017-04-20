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
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter;
    using vmware.vcenter.vm.hardware;
    using vmware.vcenter.vm.hardware.boot;

    /// <summary>
    /// Demonstrates how to modify the boot devices used by a virtual machine,
    /// and in what order they are tried.
    ///
    /// Sample Prerequisites:
    /// The sample needs an existing VM with the following minimum number of
    /// devices:
    /// - 1 Ethernet adapter
    /// - 1 CD-ROM
    /// - 1 Floppy drive
    /// - 3 Disks
    /// </summary>
    public class BootDevicesConfiguration : SamplesBase
    {
        private string vmId;
        private List<DeviceTypes.Entry> orginalBootDeviceEntries;
        private Device bootDeviceService;
        private Disk diskService;
        private Ethernet ethernetService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which to configure the boot "
            + "device order", Required = true)]
        public string VmName { get; set; }


        public override void Run()
        {
            this.diskService = VapiAuthHelper.StubFactory.CreateStub<Disk>(
                SessionStubConfiguration);
            this.ethernetService =
                VapiAuthHelper.StubFactory.CreateStub<Ethernet>(
                    SessionStubConfiguration);
            this.bootDeviceService =
                VapiAuthHelper.StubFactory.CreateStub<Device>(
                    SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("\nUsing VM: " + VmName + " (vmId="
                + this.vmId + " ) for boot device configuration sample");

            Console.WriteLine("\nValidate whether the VM has the required "
                + "minimum number of devices");
            VM vmService = VapiAuthHelper.StubFactory.CreateStub<VM>(
                SessionStubConfiguration);
            VMTypes.Info vmInfo = vmService.Get(this.vmId);
            if(vmInfo.GetCdroms().Count < 1 || vmInfo.GetFloppies().Count < 1
                || vmInfo.GetDisks().Count < 1 || vmInfo.GetNics().Count < 1)
            {
                throw new Exception("\n Selected VM does not have the required "
                    + "minimum number of devices: i.e. 1 Ethernet adapter, "
                    + "1 CD-ROM, 1 Floppy drive, 3 disks");
            }

            Console.WriteLine("\n\n#### Example: Print the current boot device"
                + " configuration");
            List<DeviceTypes.Entry> bootDeviceEntries =
                this.bootDeviceService.Get(this.vmId);
            bootDeviceEntries.ForEach(i => Console.WriteLine(i));

            // Save the current boot info to revert it during cleanup
            this.orginalBootDeviceEntries = bootDeviceEntries;

            Console.WriteLine("\n\n#### Example: Set boot order to be Floppy, "
                + "Disk1, Disk2, Disk3, Ethernet NIC, CD-ROM");

            // Get the device identifiers for disks
            List<DiskTypes.Summary> diskSummaries =
                this.diskService.List(this.vmId);
            Console.WriteLine("\nList of disks attached to the VM: \n"
                + diskSummaries);
            List<String> diskIds = new List<String>();
            foreach(DiskTypes.Summary diskSummary in diskSummaries)
            {
                diskIds.Add(diskSummary.GetDisk());
            }

            // Get device identifiers for Ethernet NICs
            List<EthernetTypes.Summary> ethernetSummaries =
                this.ethernetService.List(this.vmId);
            Console.WriteLine("\nList of Ethernet NICs attached to the VM: \n"
                + ethernetSummaries);
            List<String> ethernetIds = new List<String>();
            foreach(EthernetTypes.Summary ethernetSummary in ethernetSummaries)
            {
                ethernetIds.Add(ethernetSummary.GetNic());
            }

            List<DeviceTypes.Entry> devices = new List<DeviceTypes.Entry>(4);
            devices.Add(new DeviceTypes.Entry());
            devices[0].SetType(DeviceTypes.Type.FLOPPY);

            devices.Add(new DeviceTypes.Entry());
            devices[1].SetDisks(diskIds);
            devices[1].SetType(DeviceTypes.Type.DISK);

            devices.Add(new DeviceTypes.Entry());
            devices[2].SetNic(ethernetIds[0]);
            devices[2].SetType(DeviceTypes.Type.ETHERNET);

            devices.Add(new DeviceTypes.Entry());
            devices[3].SetType(DeviceTypes.Type.CDROM);

            this.bootDeviceService.Set(this.vmId, devices);
            bootDeviceEntries = this.bootDeviceService.Get(this.vmId);
            Console.WriteLine("\nNew boot device configuration");
            bootDeviceEntries.ForEach(i => Console.WriteLine(i));
        }

        public override void Cleanup()
        {
            Console.WriteLine("\n#### Cleanup: Revert boot device "
                + "configuration");
            this.bootDeviceService.Set(this.vmId,
                this.orginalBootDeviceEntries);
            List<DeviceTypes.Entry> bootDeviceEntries =
                this.bootDeviceService.Get(this.vmId);
            bootDeviceEntries.ForEach(i => Console.WriteLine(i));
        }

        public static void Main(string[] args)
        {
            new BootDevicesConfiguration().Execute(args);
        }
    }
}
