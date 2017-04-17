/*
 * Copyright 2016 VMware, Inc.  All rights reserved.
 */
namespace vmware.samples.vcenter.helpers
{
    using System;
    using System.Collections.Generic;
    using vmware.vcenter;
    using vmware.vapi.bindings;
    using vmware.samples.common;

    public class VmHelper
    {
        /// <summary>
        /// Returns the identifier of a VM
        ///
        /// Note: The method assumes that there is only one VM and datacenter
        ///  with the specified names.
        /// </summary>
        /// <param name="stubFactory">Stub factory for api endpoint</param>
        /// <param name="sessionStubConfig">stub configuration for the current
        ///  session
        /// </param>
        /// <param name="vmName">name of the vm</param>
        /// <returns>the identifier of a VM</returns>
        public static String GetVm(StubFactory stubFactory,
            StubConfiguration sessionStubConfig, string vmName)
        {
            VMTypes.FilterSpec vmFilterSpec = new VMTypes.FilterSpec();
            vmFilterSpec.SetNames(new HashSet<String> { vmName });

            VM vmService = stubFactory.CreateStub<VM>(sessionStubConfig);
            List<VMTypes.Summary> vmSummaries = vmService.List(vmFilterSpec);

            if (vmSummaries.Count > 1)
            {
                throw new Exception(String.Format("More than one vm" +
                    " with the specified name {0} exist",
                    vmName));

            }

            if (vmSummaries.Count <= 0)
            {
                throw new Exception(String.Format("VM with name {0}" +
                    "not found !", vmName));
            }

            return vmSummaries[0].GetVm();
        }
    }
}
