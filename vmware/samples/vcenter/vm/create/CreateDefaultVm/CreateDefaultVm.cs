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
    using System;
    using vmware.samples.common;
    using vmware.samples.vcenter.helpers;
    using vmware.vcenter;
    using vmware.vcenter.vm;

    /// <summary>
    /// Demonstrates how to create a VM with system provided defaults
    ///
    /// Sample Prerequisites:
    /// The sample needs a datacenter and the following resources:
    /// - vm folder
    /// - datastore
    /// - cluster
    /// - A standard switch network
    /// </summary>
    public class CreateDefaultVm : SamplesBase
    {
        private VM vmService;
        private string defaultVMId;
        private readonly GuestOS vmGuestOS = GuestOS.WINDOWS_9_64;
        private const string DefaultVmName = "Sample-Default-VM";

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
                    VapiAuthHelper.StubFactory, SessionStubConfiguration,
                    DatacenterName, ClusterName, VmFolderName, DatastoreName);

            // Create the default VM
            CreateVm(vmPlacementSpec);
        }

        public override void Cleanup()
        {
            if(this.defaultVMId != null)
            {
                this.vmService.Delete(this.defaultVMId);
            }
            VapiAuthHelper.Logout();
        }

        /*
         * Creates a VM on a cluster with selected Guest OS and name which
         * uses all the system provided defaults.
         */
        private void CreateVm(VMTypes.PlacementSpec vmPlacementSpec)
        {
            VMTypes.CreateSpec vmCreateSpec = new VMTypes.CreateSpec();
            vmCreateSpec.SetName(DefaultVmName);
            vmCreateSpec.SetGuestOS(vmGuestOS);
            vmCreateSpec.SetPlacement(vmPlacementSpec);
            Console.WriteLine("\n\n#### Example: Creating Default VM with "
                              + "spec:\n" + vmCreateSpec);
            this.vmService = VapiAuthHelper.StubFactory.CreateStub<VM>(
                SessionStubConfiguration);
            this.defaultVMId = this.vmService.Create(vmCreateSpec);

            Console.WriteLine("\nCreated default VM : " + DefaultVmName
                              + " with id: " + defaultVMId);
            VMTypes.Info vmInfo = this.vmService.Get(defaultVMId);
            Console.Write("\nDefault VM Info:\n" + vmInfo);
        }

        public static void Main(string[] args)
        {
            new CreateDefaultVm().Execute(args);
        }
    }
}
