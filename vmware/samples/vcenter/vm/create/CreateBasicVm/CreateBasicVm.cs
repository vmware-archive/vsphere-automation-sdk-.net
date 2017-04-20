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
namespace vmware.samples.vcenter.vm.create
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter;
    using vmware.vcenter.vm;
    using vmware.vcenter.vm.hardware;
    using vmware.vcenter.vm.hardware.boot;

    /// <summary>
    /// Demonstrates how to create a basic VM with following configuration:
    /// Basic VM (2 disks, 1 nic)
    ///
    /// Sample Prerequisites:
    /// The sample needs a datacenter and the following resources:
    /// - vm folder
    /// - datastore
    /// - cluster
    /// - A standard switch network
    /// </summary>
    public class CreateBasicVm : SamplesBase
    {
        private VM vmService;
        private string basicVmId;
        private readonly GuestOS vmGuestOS = GuestOS.WINDOWS_9_64;
        private const string BasicVmName = "Sample-Basic-VM";

        [Option(
            "datacenter",
            HelpText = "The name of the datacenter on which to create the vm.",
            Required = true)]
        public string DatacenterName { get; set; }

        [Option(
            "cluster",
            HelpText = "The name of the cluster on which to create the vm.",
            Required = true)]
        public string ClusterName { get; set; }

        [Option(
            "vmfolder",
            HelpText = "The name of the vm folder on which to create the vm.",
            Required = true)]
        public string VmFolderName { get; set; }

        [Option(
            "datastore",
            HelpText = "The name of the datastore on which to create the vm",
            Required = true)]
        public string DatastoreName { get; set; }

        [Option(
            "standardportgroup",
            HelpText = "The name of the standard portgroup",
            Required = true)]
        public string StandardPortgroupName { get; set; }

        public override void Run()
        {
            // Get a placement spec
            VMTypes.PlacementSpec vmPlacementSpec =
                PlacementHelper.GetPlacementSpecForCluster(
                    VapiAuthHelper.StubFactory, SessionStubConfiguration,
                    DatacenterName, ClusterName,
                    VmFolderName, DatastoreName);

            // Get a standard network backing
            string standardNetworkBacking =
                NetworkHelper.GetStandardNetworkBacking(
                VapiAuthHelper.StubFactory, SessionStubConfiguration,
                DatacenterName, StandardPortgroupName);

            // Create the vm
            CreateVm(vmPlacementSpec, standardNetworkBacking);
        }

        public override void Cleanup()
        {
            if (this.basicVmId != null)
            {
                this.vmService.Delete(this.basicVmId);
            }
        }

        /*
         * Creates a basic VM on a cluster with the following configuration:
         * - Create 2 disks and specify one of them on scsi0:0 since
         * it's the boot disk.
         * - Specify 1 ethernet adapter using a Standard Portgroup backing.
         * - Setup for PXE install by selecting network as first boot device.
         * - Use guest and system provided defaults for most configuration settings.
         */
        private void CreateVm(VMTypes.PlacementSpec vmPlacementSpec, string standardNetworkBacking)
        {
            // Create the scsi disk as a boot disk
            DiskTypes.CreateSpec bootDiskCreateSpec =
                new DiskTypes.CreateSpec();
            bootDiskCreateSpec.SetType(DiskTypes.HostBusAdapterType.SCSI);
            ScsiAddressSpec scsiAddressSpec = new ScsiAddressSpec();
            scsiAddressSpec.SetBus(0L);
            scsiAddressSpec.SetUnit(0L);
            bootDiskCreateSpec.SetScsi(scsiAddressSpec);
            bootDiskCreateSpec.SetNewVmdk(new DiskTypes.VmdkCreateSpec());

            // Create a data disk
            DiskTypes.CreateSpec dataDiskCreateSpec =
                new DiskTypes.CreateSpec();
            dataDiskCreateSpec.SetNewVmdk(new DiskTypes.VmdkCreateSpec());

            // Create a Ethernet NIC with standard network backing
            EthernetTypes.BackingSpec nicBackingSpec =
                new EthernetTypes.BackingSpec();
            nicBackingSpec.SetType(EthernetTypes.BackingType.STANDARD_PORTGROUP);
            nicBackingSpec.SetNetwork(standardNetworkBacking);
            EthernetTypes.CreateSpec nicCreateSpec =
                new EthernetTypes.CreateSpec();
            nicCreateSpec.SetStartConnected(true);
            nicCreateSpec.SetBacking(nicBackingSpec);

            // Specify the boot order
            List<DeviceTypes.EntryCreateSpec> bootDevices =
                new List<DeviceTypes.EntryCreateSpec>();
            bootDevices.Add(new DeviceTypes.EntryCreateSpec());
            bootDevices.Add(new DeviceTypes.EntryCreateSpec());
            bootDevices[0].SetType(DeviceTypes.Type.ETHERNET);
            bootDevices[1].SetType(DeviceTypes.Type.DISK);

            // Create the VM
            VMTypes.CreateSpec vmCreateSpec = new VMTypes.CreateSpec();
            vmCreateSpec.SetGuestOS(vmGuestOS);
            vmCreateSpec.SetName(BasicVmName);
            vmCreateSpec.SetBootDevices(bootDevices);
            vmCreateSpec.SetPlacement(vmPlacementSpec);
            vmCreateSpec.SetNics(new List<EthernetTypes.CreateSpec>
            {
                nicCreateSpec
            });
            vmCreateSpec.SetDisks(new List<DiskTypes.CreateSpec>
            {
                bootDiskCreateSpec,
                dataDiskCreateSpec
            });

            Console.WriteLine(dataDiskCreateSpec.ToString());
            Console.WriteLine("\n\n#### Example: Creating Basic VM with spec:\n"
                              + vmCreateSpec);
            this.vmService = VapiAuthHelper.StubFactory.CreateStub<VM>(
                SessionStubConfiguration);
            this.basicVmId = this.vmService.Create(vmCreateSpec);
            VMTypes.Info vmInfo = this.vmService.Get(basicVmId);
            Console.WriteLine("\nBasic VM Info:\n" + vmInfo);
        }

        public static void Main(string[] args)
        {
            new CreateBasicVm().Execute(args);
        }
    }
}
