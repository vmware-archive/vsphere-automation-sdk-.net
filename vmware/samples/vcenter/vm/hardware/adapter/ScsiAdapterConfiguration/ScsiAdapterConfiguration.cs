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
    /// Demonstrates how to configure virtual SCSI adapters of a virtual
    /// machine.
    ///
    /// Sample Prerequisites: The sample needs an existing VM.
    /// </summary>
    public class ScsiAdapterConfiguration : SamplesBase
    {
        private string vmId;
        private List<string> createdScsiAdapters = new List<string>();
        private Scsi scsiService;

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

            this.scsiService = VapiAuthHelper.StubFactory.CreateStub<Scsi>(
                SessionStubConfiguration);

            Console.WriteLine("\n\n#### Setup: Get the virtual machine id");
            this.vmId = VmHelper.GetVm(VapiAuthHelper.StubFactory,
                SessionStubConfiguration, VmName);
            Console.WriteLine("Using VM: " + VmName + " (vmId=" + this.vmId +
                ") for SCSI adapter configuration sample");

            Console.WriteLine("\n\n#### Example: List of all SCSI adapters "
                + "on the VM");
            List<ScsiTypes.Summary> scsiSummaries =
                this.scsiService.List(this.vmId);
            scsiSummaries.ForEach(i => Console.WriteLine(i));

            Console.WriteLine("\n\n#### Display information about each "
                + "adapter");
            foreach(ScsiTypes.Summary scsiSummary in scsiSummaries)
            {
                ScsiTypes.Info info = this.scsiService.Get(this.vmId,
                    scsiSummary.GetAdapter());
                Console.WriteLine(info);
            }

            Console.WriteLine("\n\n#### Example: Create SCSI adapter with "
                + "defaults.");
            ScsiTypes.CreateSpec scsiCreateSpec = new ScsiTypes.CreateSpec();
            string scsiId = this.scsiService.Create(this.vmId, scsiCreateSpec);
            Console.WriteLine(scsiCreateSpec);
            ScsiTypes.Info scsiInfo = this.scsiService.Get(this.vmId, scsiId);
            Console.WriteLine("SCSI Adapter ID=" + scsiId);
            Console.WriteLine(scsiInfo);
            this.createdScsiAdapters.Add(scsiId);

            Console.WriteLine("\n\n#### Create SCSI adapter with specific "
                + "bus and sharing=true");
            scsiCreateSpec = new ScsiTypes.CreateSpec();
            scsiCreateSpec.SetBus(2L);
            scsiCreateSpec.SetSharing(ScsiTypes.Sharing.VIRTUAL);
            scsiId = this.scsiService.Create(this.vmId, scsiCreateSpec);
            Console.WriteLine(scsiCreateSpec);
            scsiInfo = this.scsiService.Get(this.vmId, scsiId);
            Console.WriteLine("SCSI Adapter ID=" + scsiId);
            Console.WriteLine(scsiInfo);
            this.createdScsiAdapters.Add(scsiId);

            Console.WriteLine("\n\n#### Update SCSI adapter by setting "
                + "sharing=false");
            ScsiTypes.UpdateSpec scsiUpdateSpec = new ScsiTypes.UpdateSpec();
            scsiUpdateSpec.SetSharing(ScsiTypes.Sharing.NONE);
            this.scsiService.Update(this.vmId, scsiId, scsiUpdateSpec);
            Console.WriteLine(scsiUpdateSpec);
            scsiInfo = this.scsiService.Get(this.vmId, scsiId);
            Console.WriteLine("SCSI Adapter ID=" + scsiId);
            Console.WriteLine(scsiInfo);

            // List all SCSI adapters for a VM
            Console.WriteLine("\n\n#### List all SCSI adapters on the VM");
            scsiSummaries = this.scsiService.List(this.vmId);
            scsiSummaries.ForEach(i => Console.WriteLine(i));


        }

        public override void Cleanup()
        {
            Console.WriteLine("\n\n#### Cleanup: Deleting all adapters that "
                + "were created");
            foreach(string scsiId in createdScsiAdapters)
            {
                this.scsiService.Delete(this.vmId, scsiId);
            }
            Console.WriteLine("\n\n#### List all SCSI adapters on the VM");
            List<ScsiTypes.Summary> scsiSummaries =
                this.scsiService.List(this.vmId);
            scsiSummaries.ForEach(i => Console.WriteLine(i));
            VapiAuthHelper.Logout();
        }

        public static void Main(string[] args)
        {
            new ScsiAdapterConfiguration().Execute(args);
        }
    }
}
