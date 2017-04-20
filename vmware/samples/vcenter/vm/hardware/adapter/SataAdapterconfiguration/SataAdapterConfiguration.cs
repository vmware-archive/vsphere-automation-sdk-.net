/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.vm.hardware.adapter
{
    using CommandLine;
    using common.authentication;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter.vm.hardware.adapter;

    /// <summary>
    /// Demonstrates how to configure virtual SATA adapters of a virtual
    /// machine.
    ///
    /// Sample Prerequisites: The sample needs an existing VM.
    /// </summary>
    public class ScsiAdapterConfiguration : SamplesBase
    {
        private string vmId;
        private List<string> createdSataAdapters = new List<string>();
        private Sata sataService;

        [Option(
            "vmname",
            HelpText = "The name of the vm for which the virtual SCSI adapter "
            + "needs to be configured.", Required = true)]
        public string VmName { get; set; }


        public override void Run()
        {
            // Login
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);

            this.sataService = VapiAuthHelper.StubFactory.CreateStub<Sata>(
                SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId +
                ") for SATA adapter configuration sample");

            Console.WriteLine("\n\n#### Example: List of all SATA adapters "
                + "on the VM");
            List<SataTypes.Summary> sataSummaries =
                this.sataService.List(this.vmId);
            sataSummaries.ForEach(i => Console.WriteLine(i));

            Console.WriteLine("\n\n#### Display information about each "
                + "adapter");
            foreach(SataTypes.Summary sataSummary in sataSummaries)
            {
                SataTypes.Info info = this.sataService.Get(this.vmId,
                    sataSummary.GetAdapter());
                Console.WriteLine(info);
            }

            Console.WriteLine("\n\n#### Example: Create SATA adapter with "
                + "defaults.");
            SataTypes.CreateSpec sataCreateSpec = new SataTypes.CreateSpec();
            string sataId = this.sataService.Create(this.vmId, sataCreateSpec);
            Console.WriteLine(sataCreateSpec);
            SataTypes.Info sataInfo = this.sataService.Get(this.vmId, sataId);
            Console.WriteLine("SATA Adapter ID=" + sataId);
            Console.WriteLine(sataInfo);
            this.createdSataAdapters.Add(sataId);

            Console.WriteLine("\n\n#### Create SATA adapter with specific "
                + "bus");
            sataCreateSpec = new SataTypes.CreateSpec();
            sataCreateSpec.SetBus(2L);
            sataId = this.sataService.Create(this.vmId, sataCreateSpec);
            Console.WriteLine(sataCreateSpec);
            sataInfo = this.sataService.Get(this.vmId, sataId);
            Console.WriteLine("SATA Adapter ID=" + sataId);
            Console.WriteLine(sataInfo);
            this.createdSataAdapters.Add(sataId);

            // List all SATA adapters for a VM
            Console.WriteLine("\n\n#### List all SATA adapters on the VM");
            sataSummaries = this.sataService.List(this.vmId);
            sataSummaries.ForEach(i => Console.WriteLine(i));


        }

        public override void Cleanup()
        {
            Console.WriteLine("\n\n#### Cleanup: Deleting all adapters that "
                + "were created");
            foreach(string sataId in createdSataAdapters)
            {
                this.sataService.Delete(this.vmId, sataId);
            }
            Console.WriteLine("\n\n#### List all SATA adapters on the VM");
            List<SataTypes.Summary> sataSummaries =
                this.sataService.List(this.vmId);
            sataSummaries.ForEach(i => Console.WriteLine(i));
            VapiAuthHelper.Logout();
        }

        public static void Main(string[] args)
        {
            new ScsiAdapterConfiguration().Execute(args);
        }
    }
}
