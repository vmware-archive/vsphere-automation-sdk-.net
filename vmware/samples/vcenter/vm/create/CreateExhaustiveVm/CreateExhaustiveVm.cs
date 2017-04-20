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
    using common.authentication;
    using helpers;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.vcenter;
    using vmware.vcenter.vm;
    using vmware.vcenter.vm.hardware;
    using vmware.vcenter.vm.hardware.boot;

    /// <summary>
    /// Create an exhaustive VM with the following configuration:
    /// - Hardware Version = VMX_11 (for 6.0)
    /// - CPU (count = 2, coresPerSocket = 2, hotAddEnabled = false,
    ///   hotRemoveEnabled = false)
    /// - Memory (size_mib = 2 GB, hotAddEnabled = false)
    /// - 3 Disks and specify each of the HBAs and the unit numbers
    ///   (capacity=40 GB, name=<some value>, spaceEfficient=true)
    /// - Specify 2 ethernet adapters, one using a Standard Portgroup backing
    ///   and the other using a DISTRIBUTED_PORTGROUP networking backing.
    ///        # nic1: Specify Ethernet (macType=MANUAL,
    ///        macAddress=<some value>)
    ///        # nic2: Specify Ethernet (macType=GENERATED)
    /// - 1 CDROM (type=ISO_FILE, file="small.iso", startConnected=true)
    /// - 1 Serial Port (type=NETWORK_SERVER, file="tcp://localhost/16000",
    ///   startConnected=true)
    /// - 1 Parallel Port  (type=HOST_DEVICE, startConnected=false)
    /// - 1 Floppy Drive (type=CLIENT_DEVICE)
    /// - Boot, type=BIOS
    /// - BootDevice order: CDROM, DISK, ETHERNET
    /// </summary>
    public class CreateExhaustiveVm : SamplesBase
    {
        private VM vmService;
        private const string ExhaustiveVmName = "Sample-Exhaustive-VM";
        private const string SerialPortNetworkServiceLocation =
            "tcp://localhost:16000";
        private static long GB = 1024 * 1024 * 1024;
        private GuestOS vmGuestOS = GuestOS.WINDOWS_9_64;
        private HardwareTypes.Version HARDWARE_VERSION =
                HardwareTypes.Version.VMX_11;
        private string exhaustiveVMId;

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

        [Option(
             "distributedportgroup",
             HelpText = "The name of the distributed portgroup",
             Required = true)]
        public string DistributedPortgroupName { get; set; }

        [Option(
             "isodatastorepath",
             HelpText = "The path to the iso file on the datastore",
             Required = true)]
        public string IsoDatastorePath { get; set; }

        public override void Run()
        {
            // Login
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);

            // Get a placement spec
            VMTypes.PlacementSpec vmPlacementSpec =
                PlacementHelper.GetPlacementSpecForCluster(
                    VapiAuthHelper.StubFactory,
                    SessionStubConfiguration,
                    DatacenterName,
                    ClusterName,
                    VmFolderName,
                    DatastoreName);

            // Get a standard network backing
            string standardNetworkBacking =
                NetworkHelper.GetStandardNetworkBacking(
                    VapiAuthHelper.StubFactory, SessionStubConfiguration,
                    DatacenterName, StandardPortgroupName);

            // Get a distributed network backing
            string distributedNetworkBacking =
                NetworkHelper.GetDistributedNetworkBacking(
                    VapiAuthHelper.StubFactory, SessionStubConfiguration,
                    DatacenterName, DistributedPortgroupName);

            // Create the VM
            CreateVm(vmPlacementSpec, standardNetworkBacking,
                distributedNetworkBacking);
        }

        public override void Cleanup()
        {
            if (this.exhaustiveVMId != null)
            {
                this.vmService.Delete(this.exhaustiveVMId);
            }
            VapiAuthHelper.Logout();
        }

        private void CreateVm(VMTypes.PlacementSpec vmPlacementSpec,
            string standardNetworkBacking, string distributedNetworkBacking)
        {
            // CPU UpdateSpec
            CpuTypes.UpdateSpec cpuUpdateSpec = new CpuTypes.UpdateSpec();
            cpuUpdateSpec.SetCoresPerSocket(1L);
            cpuUpdateSpec.SetHotAddEnabled(false);
            cpuUpdateSpec.SetHotRemoveEnabled(false);

            // Memory UpdateSpec
            MemoryTypes.UpdateSpec memoryUpdateSpec =
                new MemoryTypes.UpdateSpec();
            memoryUpdateSpec.SetSizeMiB(2 * 1024L);
            memoryUpdateSpec.SetHotAddEnabled(false);

            // Boot disk
            ScsiAddressSpec scsiAddressSpec = new ScsiAddressSpec();
            scsiAddressSpec.SetBus(0L);
            scsiAddressSpec.SetUnit(0L);
            DiskTypes.VmdkCreateSpec vmdkCreateSpec1 =
                new DiskTypes.VmdkCreateSpec();
            vmdkCreateSpec1.SetCapacity(40 * GB);
            vmdkCreateSpec1.SetName("boot");
            DiskTypes.CreateSpec diskCreateSpec1 = new DiskTypes.CreateSpec();
            diskCreateSpec1.SetType(DiskTypes.HostBusAdapterType.SCSI);
            diskCreateSpec1.SetScsi(scsiAddressSpec);
            diskCreateSpec1.SetNewVmdk(vmdkCreateSpec1);

            // Data disk 1
            DiskTypes.VmdkCreateSpec vmdkCreateSpec2 =
                new DiskTypes.VmdkCreateSpec();
            vmdkCreateSpec2.SetCapacity(10 * GB);
            vmdkCreateSpec2.SetName("data1");
            DiskTypes.CreateSpec diskCreateSpec2 = new DiskTypes.CreateSpec();
            diskCreateSpec2.SetNewVmdk(vmdkCreateSpec2);

            // Data disk 2
            DiskTypes.VmdkCreateSpec vmdkCreateSpec3 =
                new DiskTypes.VmdkCreateSpec();
            vmdkCreateSpec3.SetCapacity(10 * GB);
            vmdkCreateSpec3.SetName("data2");
            DiskTypes.CreateSpec diskCreateSpec3 = new DiskTypes.CreateSpec();
            diskCreateSpec3.SetNewVmdk(vmdkCreateSpec3);

            // Ethernet CreateSpec (manual with standard portgroup)
            EthernetTypes.BackingSpec nicStandardNetworkBacking =
                new EthernetTypes.BackingSpec();
            nicStandardNetworkBacking.SetNetwork(standardNetworkBacking);
            nicStandardNetworkBacking.SetType(
                EthernetTypes.BackingType.STANDARD_PORTGROUP);
            EthernetTypes.CreateSpec manualEthernetSpec =
                    new EthernetTypes.CreateSpec();
            manualEthernetSpec.SetStartConnected(true);
            manualEthernetSpec.SetMacType(EthernetTypes.MacAddressType.MANUAL);
            manualEthernetSpec.SetMacAddress("11:23:58:13:21:34");
            manualEthernetSpec.SetBacking(nicStandardNetworkBacking);


            EthernetTypes.BackingSpec nicDistributedNetworkBacking =
                new EthernetTypes.BackingSpec();
            nicDistributedNetworkBacking.SetNetwork(distributedNetworkBacking);
            nicDistributedNetworkBacking.SetType(
                EthernetTypes.BackingType.DISTRIBUTED_PORTGROUP);
            EthernetTypes.CreateSpec generatedEthernetSpec =
                new EthernetTypes.CreateSpec();
            generatedEthernetSpec.SetStartConnected(true);
            generatedEthernetSpec.SetMacType(
                EthernetTypes.MacAddressType.GENERATED);
            generatedEthernetSpec.SetBacking(nicDistributedNetworkBacking);

            // Cdrom CreateSpec
            CdromTypes.BackingSpec cdromBackingSpec =
                new CdromTypes.BackingSpec();
            cdromBackingSpec.SetType(CdromTypes.BackingType.ISO_FILE);
            cdromBackingSpec.SetIsoFile(IsoDatastorePath);
            CdromTypes.CreateSpec cdromCreateSpec =
                    new CdromTypes.CreateSpec();
            cdromCreateSpec.SetBacking(cdromBackingSpec);

            // Serial Port CreateSpec
            SerialTypes.BackingSpec serialBackingSpec =
                new SerialTypes.BackingSpec();
            serialBackingSpec.SetType(SerialTypes.BackingType.NETWORK_SERVER);
            serialBackingSpec.SetNetworkLocation(
                new Uri(SerialPortNetworkServiceLocation));
            SerialTypes.CreateSpec serialCreateSpec =
                new SerialTypes.CreateSpec();
            serialCreateSpec.SetStartConnected(false);
            serialCreateSpec.SetBacking(serialBackingSpec);

            // Parallel port CreateSpec
            ParallelTypes.BackingSpec parallelBackingSpec =
                new ParallelTypes.BackingSpec();
            parallelBackingSpec.SetType(ParallelTypes.BackingType.HOST_DEVICE);
            ParallelTypes.CreateSpec parallelCreateSpec =
                    new ParallelTypes.CreateSpec();
            parallelCreateSpec.SetBacking(parallelBackingSpec);
            parallelCreateSpec.SetStartConnected(false);

            // Floppy CreateSpec
            FloppyTypes.BackingSpec floppyBackingSpec =
                new FloppyTypes.BackingSpec();
            floppyBackingSpec.SetType(FloppyTypes.BackingType.CLIENT_DEVICE);
            FloppyTypes.CreateSpec floppyCreateSpec =
                new FloppyTypes.CreateSpec();
            floppyCreateSpec.SetBacking(floppyBackingSpec);

            // Specify the boot order
            DeviceTypes.EntryCreateSpec cdromEntry =
                new DeviceTypes.EntryCreateSpec();
            cdromEntry.SetType(DeviceTypes.Type.CDROM);

            DeviceTypes.EntryCreateSpec diskEntry =
                new DeviceTypes.EntryCreateSpec();
            diskEntry.SetType(DeviceTypes.Type.DISK);

            DeviceTypes.EntryCreateSpec ethernetEntry =
                new DeviceTypes.EntryCreateSpec();
            ethernetEntry.SetType(DeviceTypes.Type.ETHERNET);

            List<DeviceTypes.EntryCreateSpec> bootDevices =
                new List<DeviceTypes.EntryCreateSpec> { cdromEntry, diskEntry,
                ethernetEntry };

            // Create a VM with above configuration
            VMTypes.CreateSpec vmCreateSpec = new VMTypes.CreateSpec();
            vmCreateSpec.SetBootDevices(bootDevices);
            vmCreateSpec.SetCdroms(
                new List<CdromTypes.CreateSpec> { cdromCreateSpec });
            vmCreateSpec.SetCpu(cpuUpdateSpec);
            vmCreateSpec.SetDisks(new List<DiskTypes.CreateSpec> {
                diskCreateSpec1, diskCreateSpec2, diskCreateSpec3 });
            vmCreateSpec.SetFloppies(
                new List<FloppyTypes.CreateSpec> { floppyCreateSpec });
            vmCreateSpec.SetHardwareVersion(HARDWARE_VERSION);
            vmCreateSpec.SetMemory(memoryUpdateSpec);
            vmCreateSpec.SetGuestOS(vmGuestOS);
            vmCreateSpec.SetName(ExhaustiveVmName);
            vmCreateSpec.SetNics(
                new List<EthernetTypes.CreateSpec> { manualEthernetSpec,
                    generatedEthernetSpec });
            vmCreateSpec.SetParallelPorts(
                new List<ParallelTypes.CreateSpec> { parallelCreateSpec });
            vmCreateSpec.SetPlacement(vmPlacementSpec);
            vmCreateSpec.SetSerialPorts(
                new List<SerialTypes.CreateSpec> { serialCreateSpec });

            Console.WriteLine("\n\n#### Example: Creating exhaustive VM with "
                   + "spec:\n" + vmCreateSpec);
            this.vmService = VapiAuthHelper.StubFactory.CreateStub<VM>(
                SessionStubConfiguration);
            this.exhaustiveVMId = vmService.Create(vmCreateSpec);
            Console.WriteLine("\nCreated exhaustive VM : " + ExhaustiveVmName
                               + " with id: " + this.exhaustiveVMId);
            VMTypes.Info vmInfo = vmService.Get(this.exhaustiveVMId);
            Console.WriteLine("\nExhaustive VM Info:\n" + vmInfo);
        }

        public static void Main(string[] args)
        {
            new CreateExhaustiveVm().Execute(args);
        }
    }
}
