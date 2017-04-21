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
    using common.authentication;
    using helpers;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.vcenter.vm;
    using vmware.vcenter.vm.hardware;
    using vmware.vcenter.vm.hardware.adapter;

    /// <summary>
    /// Description: Demonstrates how to configure a CD-ROM device for a VM.
    ///
    /// Author: VMware, Inc.
    /// Sample Prerequisites: The sample needs an existing VM and an iso file
    ///  on a datastore.
    /// </summary>
    public class CdromConfiguration : SamplesBase
    {
        private string vmId;
        private string sataId;
        private Cdrom cdromService;
        private Power powerService;
        private Sata sataService;
        private List<string> createdCdroms = new List<string>();

        [Option(
            "vmname",
            HelpText = "The name of the vm for which virtual CD-ROM needs to "
            + "be configured", Required = true)]
        public string VmName { get; set; }

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
            this.cdromService =
                VapiAuthHelper.StubFactory.CreateStub<Cdrom>(
                    SessionStubConfiguration);
            this.powerService =
                    VapiAuthHelper.StubFactory.CreateStub<Power>(
                        SessionStubConfiguration);
            this.sataService =
                VapiAuthHelper.StubFactory.CreateStub<Sata>(
                    SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId="
                + this.vmId + " ) for the CD-ROM configuration sample.");

            Console.WriteLine("\n\n#### Setup: Create SATA controller");
            SataTypes.CreateSpec sataCreateSpec = new SataTypes.CreateSpec();
            this.sataId = sataService.Create(this.vmId, sataCreateSpec);
            Console.WriteLine(sataCreateSpec);

            Console.WriteLine("\n\n### Example: List all CD-ROMs");
            ListAllCdroms();

            Console.WriteLine("\n\n### Example: Create CD-ROM with ISO_FILE"
                    + " backing");
            CreateCdrom(CdromTypes.BackingType.ISO_FILE);

            Console.WriteLine("\n\n### Example: Create CD-ROM with "
                               + "CLIENT_DEVICE backing");
            CreateCdrom(CdromTypes.BackingType.CLIENT_DEVICE);

            Console.WriteLine("\n\n### Example: Create SATA CD-ROM with"
                               + " CLIENT_DEVICE backing");
            CreateCdromForAdapterType(CdromTypes.HostBusAdapterType.SATA,
                CdromTypes.BackingType.CLIENT_DEVICE);

            Console.WriteLine("\n\n### Example: Create SATA CD-ROM on specific"
                               + " bus with CLIENT_DEVICE backing");
            CreateSataCdromAtSpecificLocation(
                CdromTypes.BackingType.CLIENT_DEVICE, 0L, null);

            Console.WriteLine("\n\n### Example: Create SATA CD-ROM on specific"
                               + " bus and unit number with CLIENT_DEVICE "
                               + "backing");
            CreateSataCdromAtSpecificLocation(
                CdromTypes.BackingType.CLIENT_DEVICE, 0L, 10L);

            Console.WriteLine("\n\n### Example: Create IDE CD-ROM with"
                               + " CLIENT_DEVICE backing");
            CreateCdromForAdapterType(CdromTypes.HostBusAdapterType.IDE,
                CdromTypes.BackingType.CLIENT_DEVICE);

            Console.WriteLine("\n\n### Example: Create IDE CD-ROM as a slave"
                               + " device with HOST_DEVICE backing");
            CreateIdeCdromAsSpecificDevice(
                CdromTypes.BackingType.HOST_DEVICE, false);
        }

        /// <summary>
        /// Displays info of each CD-ROM on the VM
        /// </summary>
        private void ListAllCdroms()
        {
            List<CdromTypes.Summary> cdromSummaries =
                this.cdromService.List(this.vmId);
            Console.WriteLine(cdromSummaries);

            foreach (CdromTypes.Summary cdromSummary in cdromSummaries)
            {
                String cdromId = cdromSummary.GetCdrom();
                CdromTypes.Info cdromInfo = this.cdromService.Get(this.vmId,
                    cdromId);
                Console.WriteLine(cdromInfo);
            }
        }

        /// <summary>
        /// Creates a CD-ROM device with the specified backing type
        /// </summary>
        /// <param name="backingType">backing type for the CD-ROM device
        /// </param>
        private void CreateCdrom(CdromTypes.BackingType backingType)
        {
            CdromTypes.CreateSpec cdromCreateSpec =
                    new CdromTypes.CreateSpec();
            CdromTypes.BackingSpec backingSpec = new CdromTypes.BackingSpec();
            backingSpec.SetType(backingType);
            cdromCreateSpec.SetBacking(backingSpec);

            if (backingType.Equals(CdromTypes.BackingType.ISO_FILE))
            {
                cdromCreateSpec.GetBacking().SetIsoFile(IsoDatastorePath);
            }

            String cdromId =
                this.cdromService.Create(this.vmId, cdromCreateSpec);
            Console.WriteLine(cdromCreateSpec);
            this.createdCdroms.Add(cdromId);
            CdromTypes.Info cdromInfo =
                this.cdromService.Get(this.vmId, cdromId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("CD-ROM ID=" + cdromId);
            Console.WriteLine(cdromInfo);
        }

        /// <summary>
        /// Creates a CD-ROM device for the specified host bus adapter type
        ///  and backing type
        /// </summary>
        /// <param name="hostBusAdapterType">host bus adapter type for CD-ROM
        /// </param>
        /// <param name="backingType">backing type for the CD-ROM</param>
        private void CreateCdromForAdapterType(
            CdromTypes.HostBusAdapterType hostBusAdapterType,
            CdromTypes.BackingType backingType)
        {

            CdromTypes.CreateSpec cdromCreateSpec =
                    new CdromTypes.CreateSpec();

            CdromTypes.BackingSpec backingSpec =
                new CdromTypes.BackingSpec();
            backingSpec.SetType(backingType);
            cdromCreateSpec.SetBacking(backingSpec);
            cdromCreateSpec.SetType(hostBusAdapterType);
            String cdromId =
                this.cdromService.Create(this.vmId, cdromCreateSpec);
            Console.WriteLine(cdromCreateSpec);
            this.createdCdroms.Add(cdromId);
            CdromTypes.Info cdromInfo =
                this.cdromService.Get(this.vmId, cdromId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("CD-ROM ID=" + cdromId);
            Console.WriteLine(cdromInfo);
        }

        /// <summary>
        /// Creates an IDE CD-ROM as either a master or a slave device with the
        ///  specified backing type
        /// </summary>
        /// <param name="backingType">backing type for CD-ROM</param>
        /// <param name="isMaster">true, if CD-ROM should be created as a
        ///  master device, false otherwise </param>
        private void CreateIdeCdromAsSpecificDevice(
            CdromTypes.BackingType backingType, bool isMaster)
        {

            CdromTypes.CreateSpec cdromCreateSpec;
            String cdromId = null;

            cdromCreateSpec = new CdromTypes.CreateSpec();
            CdromTypes.BackingSpec backingSpec =
                new CdromTypes.BackingSpec();
            backingSpec.SetType(backingType);
            cdromCreateSpec.SetBacking(backingSpec);
            cdromCreateSpec.SetType(CdromTypes.HostBusAdapterType.IDE);

            IdeAddressSpec ideAddressSpec = new IdeAddressSpec();
            ideAddressSpec.SetMaster(isMaster);
            cdromCreateSpec.SetIde(ideAddressSpec);

            cdromId = this.cdromService.Create(this.vmId, cdromCreateSpec);
            Console.WriteLine(cdromCreateSpec);
            CdromTypes.Info cdromInfo =
                this.cdromService.Get(this.vmId, cdromId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("CD-ROM ID=" + cdromId);
            Console.WriteLine(cdromInfo);
            this.createdCdroms.Add(cdromId);
        }


        /// <summary>
        /// Creates a SATA CD-ROM on a specific bus/unit number with the
        ///  specified backing type.
        /// </summary>
        /// <param name="backingType">backing type for CD-ROM</param>
        /// <param name="bus">bus number</param>
        /// <param name="unit">unit number</param>
        private void CreateSataCdromAtSpecificLocation(
            CdromTypes.BackingType backingType, long? bus, long? unit)
        {

            CdromTypes.CreateSpec cdromCreateSpec;
            String cdromId = null;
            if (unit == null)
            {
                cdromCreateSpec = new CdromTypes.CreateSpec();
                CdromTypes.BackingSpec backingSpec =
                    new CdromTypes.BackingSpec();
                backingSpec.SetType(backingType);
                cdromCreateSpec.SetBacking(backingSpec);
                cdromCreateSpec.SetType(CdromTypes.HostBusAdapterType.SATA);
                SataAddressSpec sataAddressSpec = new SataAddressSpec();
                sataAddressSpec.SetBus(0L);
                cdromCreateSpec.SetSata(sataAddressSpec);

                cdromId = this.cdromService.Create(this.vmId, cdromCreateSpec);
                Console.WriteLine(cdromCreateSpec);
                CdromTypes.Info cdromInfo = this.cdromService.Get(this.vmId,
                    cdromId);
                Console.WriteLine("VM ID=" + this.vmId);
                Console.WriteLine("CD-ROM ID=" + cdromId);
                Console.WriteLine(cdromInfo);
            }
            else
            {
                cdromCreateSpec = new CdromTypes.CreateSpec();
                CdromTypes.BackingSpec backingSpec =
                    new CdromTypes.BackingSpec();
                backingSpec.SetType(backingType);
                cdromCreateSpec.SetBacking(backingSpec);
                cdromCreateSpec.SetType(CdromTypes.HostBusAdapterType.SATA);
                SataAddressSpec sataAddressSpec = new SataAddressSpec();
                sataAddressSpec.SetBus(0L);
                sataAddressSpec.SetUnit(10L);
                cdromCreateSpec.SetSata(sataAddressSpec);

                cdromId = this.cdromService.Create(this.vmId, cdromCreateSpec);
                Console.WriteLine(cdromCreateSpec);
                CdromTypes.Info cdromInfo = this.cdromService.Get(this.vmId,
                    cdromId);
                Console.WriteLine("VM ID=" + this.vmId);
                Console.WriteLine("CD-ROM ID=" + cdromId);
                Console.WriteLine(cdromInfo);
            }
            this.createdCdroms.Add(cdromId);
        }

        public override void Cleanup()
        {
            PowerTypes.Info powerInfo = this.powerService.Get(this.vmId);
            if (powerInfo.GetState().Equals(PowerTypes.State.POWERED_ON))
            {
                Console.WriteLine("\n\n#### Cleanup: Powering off the VM");
                this.powerService.Stop(this.vmId);
            }

            if (this.sataId != null)
            {
                Console.WriteLine("\n\n#### Cleanup: Deleting the SATA "
                                   + "controller");
                this.sataService.Delete(this.vmId, this.sataId);
            }

            Console.WriteLine("\n\n#### Cleanup: Deleting all the created"
                               + " CD-ROMs");
            foreach (String cdromId in createdCdroms)
            {
                this.cdromService.Delete(this.vmId, cdromId);
            }
            ListAllCdroms();
            VapiAuthHelper.Logout();
        }

        public static void Main(string[] args)
        {
            new CdromConfiguration().Execute(args);
        }
    }
}
