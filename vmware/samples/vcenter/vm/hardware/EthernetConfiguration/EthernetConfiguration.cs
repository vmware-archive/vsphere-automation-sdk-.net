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

    /// <summary>
    /// Description: Demonstrates how to configure virtual ethernet adapters of a virtual
    /// machine.
    ///
    /// Author: VMware, Inc.
    /// Sample Prerequisites: The sample needs an existing VM.
    /// </summary>
    public class EthernetConfiguration : SamplesBase
    {
        private string vmId;
        private List<string> createdNics = new List<string>();
        private Power powerService;
        private Ethernet ethernetService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which to configure virtual "
            + "ethernet adapters.", Required = true)]
        public string VmName { get; set; }

        [Option(
            "datacenter",
            HelpText = "The name of the datacenter containing the vCenter "
            + "networks.", Required = true)]
        public string Datacenter { get; set; }

        [Option(
            "standardportgroup",
            HelpText = "The name of the standard portgroup", Required = true)]
        public string StandardPortGroup { get; set; }

        [Option(
            "distributedportgroup",
            HelpText = "The name of the distributed portgroup",
            Required = true)]
        public string DistributedPortGroup { get; set; }


        public override void Run()
        {
            // Login
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);

            this.powerService = VapiAuthHelper.StubFactory.CreateStub<Power>(
                SessionStubConfiguration);
            this.ethernetService =
                VapiAuthHelper.StubFactory.CreateStub<Ethernet>(
                    SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId +
                ") for ethernet adapter configuration sample");

            // List all ethernet adapters of the virtual machine
            List<EthernetTypes.Summary> nicSummaries =
                this.ethernetService.List(this.vmId);
            nicSummaries.ForEach(i => Console.WriteLine(i));

            Console.WriteLine("\n\n#### Print info for each Ethernet NIC on "
                + "the vm.");
            foreach(EthernetTypes.Summary nicSummary in nicSummaries)
            {
                EthernetTypes.Info info =
                    this.ethernetService.Get(this.vmId, nicSummary.GetNic());
                Console.WriteLine(info);
            }

            Console.WriteLine("\n\n#### Example: Create ethernet NIC using "
                + "standard portgroup and default settings");
            string stdNetworkId = NetworkHelper.GetStandardNetworkBacking(
                VapiAuthHelper.StubFactory, SessionStubConfiguration,
                Datacenter, StandardPortGroup);
            EthernetTypes.CreateSpec nicCreateSpec =
                    new EthernetTypes.CreateSpec();
            EthernetTypes.BackingSpec nicBackingSpec =
                new EthernetTypes.BackingSpec();
            nicBackingSpec.SetNetwork(stdNetworkId);
            nicBackingSpec.SetType(
                EthernetTypes.BackingType.STANDARD_PORTGROUP);
            nicCreateSpec.SetBacking(nicBackingSpec);
            string nicId =
                this.ethernetService.Create(this.vmId, nicCreateSpec);
            this.createdNics.Add(nicId);
            Console.WriteLine(nicCreateSpec);
            EthernetTypes.Info nicInfo =
                this.ethernetService.Get(this.vmId, nicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + nicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Example: Create Ethernet NIC using"
            + " standard portgroup and specifying start_connected=true,"
            + " allow_guest_control=true, mac_type, mac_address,"
            + " wake_on_lan=enabled.");
            nicBackingSpec = new EthernetTypes.BackingSpec();
            nicBackingSpec.SetNetwork(stdNetworkId);
            nicBackingSpec.SetType(
                EthernetTypes.BackingType.STANDARD_PORTGROUP);
            nicCreateSpec = new EthernetTypes.CreateSpec();
            nicCreateSpec.SetAllowGuestControl(true);
            nicCreateSpec.SetMacType(EthernetTypes.MacAddressType.MANUAL);
            nicCreateSpec.SetMacAddress("01:23:45:67:89:10");
            nicCreateSpec.SetBacking(nicBackingSpec);
            nicId = this.ethernetService.Create(this.vmId, nicCreateSpec);
            this.createdNics.Add(nicId);
            Console.WriteLine(nicCreateSpec);
            nicInfo = this.ethernetService.Get(this.vmId, nicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + nicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Example: Create Ethernet NIC using"
            + " distributed portgroup and specifying start_connected=true,"
            + " allow_guest_control=true, mac_type, mac_address,"
            + " wake_on_lan=enabled.");
            nicBackingSpec = new EthernetTypes.BackingSpec();
            nicBackingSpec.SetNetwork(stdNetworkId);
            nicBackingSpec.SetType(
                EthernetTypes.BackingType.STANDARD_PORTGROUP);
            nicCreateSpec = new EthernetTypes.CreateSpec();
            nicCreateSpec.SetAllowGuestControl(true);
            nicCreateSpec.SetMacType(EthernetTypes.MacAddressType.MANUAL);
            nicCreateSpec.SetMacAddress("24:68:10:12:14:16");
            nicCreateSpec.SetBacking(nicBackingSpec);
            nicId = this.ethernetService.Create(this.vmId, nicCreateSpec);
            this.createdNics.Add(nicId);
            Console.WriteLine(nicCreateSpec);
            nicInfo = this.ethernetService.Get(this.vmId, nicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + nicId);
            Console.WriteLine(nicInfo);

            String lastNicId = nicId;

            Console.WriteLine(
                "\n\n#### Example: Update Ethernet NIC with different"
                + " backing.");
            nicBackingSpec = new EthernetTypes.BackingSpec();
            nicBackingSpec.SetType(
                EthernetTypes.BackingType.STANDARD_PORTGROUP);
            nicBackingSpec.SetNetwork(stdNetworkId);
            EthernetTypes.UpdateSpec nicUpdateSpec =
                    new EthernetTypes.UpdateSpec();
            nicUpdateSpec.SetBacking(nicBackingSpec);
            this.ethernetService.Update(this.vmId, lastNicId, nicUpdateSpec);
            Console.WriteLine(nicUpdateSpec);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Example: Update the Ethernet NIC,"
                    + " wake_on_lan = false, mac_type=GENERATED,"
                    + " startConnected = false, allowGuestControl = false.");
            nicUpdateSpec = new EthernetTypes.UpdateSpec();
            nicUpdateSpec.SetAllowGuestControl(false);
            nicUpdateSpec.SetStartConnected(false);
            nicUpdateSpec.SetWakeOnLanEnabled(false);
            this.ethernetService.Update(this.vmId, lastNicId, nicUpdateSpec);
            Console.WriteLine(nicUpdateSpec);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Powering on VM to run "
            + "connect/disconnect example.");
            this.powerService.Start(this.vmId);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Example: Connect Ethernet NIC after"
                    + " powering on VM.");
            this.ethernetService.Connect(this.vmId, lastNicId);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);

            Console.WriteLine("\n\n#### Example: Disconnect Ethernet NIC after"
                    + " powering on VM.");
            this.ethernetService.Disconnect(this.vmId, lastNicId);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);

            // Power off the VM
            Console.WriteLine("\n\n#### Powering off the VM after"
                    + " connect/disconnect example.");
            this.powerService.Stop(this.vmId);
            nicInfo = this.ethernetService.Get(this.vmId, lastNicId);
            Console.WriteLine("VM ID=" + this.vmId);
            Console.WriteLine("Ethernet NIC ID=" + lastNicId);
            Console.WriteLine(nicInfo);
        }

        public override void Cleanup()
        {
            if (this.powerService.Get(this.vmId).GetState().Equals(
                PowerTypes.State.POWERED_ON))
            {
                Console.WriteLine("Power off the vm");
                this.powerService.Stop(this.vmId);
            }

            // List all ethernet adapters of the virtual machine
            List<EthernetTypes.Summary> nicSummaries =
                this.ethernetService.List(this.vmId);
            nicSummaries.ForEach(i => Console.WriteLine(i));

            Console.WriteLine("\n\n#### Cleanup: Delete all the created "
                    + "Ethernet NICs.");
            foreach (string nicId in createdNics)
            {
                this.ethernetService.Delete(this.vmId, nicId);
            }
            nicSummaries = this.ethernetService.List(this.vmId);
            nicSummaries.ForEach(i => Console.WriteLine(i));
            VapiAuthHelper.Logout();
        }

        public static void Main(string[] args)
        {
            new EthernetConfiguration().Execute(args);
        }
    }
}
