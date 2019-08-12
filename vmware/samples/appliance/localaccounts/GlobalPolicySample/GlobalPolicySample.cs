/**
 * *******************************************************
 * Copyright VMware, Inc. 2019.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */


namespace vmware.samples.appliance.LocalAccount.GlobalPolicy
{
    using CommandLine;
    using System;
    using System.Collections.Generic;
    using vmware.samples.common;
    using vapi.bindings;
    using vmware.appliance.local_accounts;
    using common.authentication;


    public class GlobalPolicySample : SamplesBase
    {
        private Policy localAccountsPolicy;
        private PolicyTypes.Info policyParamInfo;

        [Option(
           "min_days",
           HelpText = "OPTIONAL:min days to be set to localaccounts globalpolicy",
           Required = false)]
        public long? minDays { get; set; }

        [Option(
           "max_days",
           HelpText = "OPTIONAL:max days to be set to localaccounts globalpolicy",
           Required = false)]
        public long? maxDays { get; set; }

        [Option(
           "warn_days",
           HelpText = "OPTIONAL:warn days to be set to localaccounts globalpolicy",
           Required = false)]
        public long? warnDays { get; set; }


        public override void Cleanup()
        {
            VapiAuthHelper.Logout();
        }

        public override void Run()
        {
            // Login
            VapiAuthHelper = new VapiAuthenticationHelper();
            SessionStubConfiguration =
                VapiAuthHelper.LoginByUsernameAndPassword(
                    Server, UserName, Password);
            this.localAccountsPolicy = VapiAuthHelper.StubFactory.CreateStub<Policy>(SessionStubConfiguration);


            policyParamInfo = new PolicyTypes.Info();
            policyParamInfo.SetMaxDays(maxDays);
            policyParamInfo.SetMinDays(minDays);
            policyParamInfo.SetWarnDays(warnDays);
            Console.WriteLine(
                "Setting values now as per passed parameters maxDays: " + maxDays
                               + ", minDays: " + minDays + ", warnDays: " + warnDays);
            localAccountsPolicy.Set(policyParamInfo);
            Console.WriteLine(
                "Values which are set are displayed below after get call:");

            Console.WriteLine("Maximum number of days between password change:"
                               + localAccountsPolicy.Get().GetMaxDays());
            Console.WriteLine("Minimum number of days between password change:"
                               + localAccountsPolicy.Get().GetMinDays());
            Console.WriteLine("Number of days of warning before password expires:"
                               + localAccountsPolicy.Get().GetWarnDays());
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            new GlobalPolicySample().Execute(args);
            Console.ReadKey();
        }
    }
}
