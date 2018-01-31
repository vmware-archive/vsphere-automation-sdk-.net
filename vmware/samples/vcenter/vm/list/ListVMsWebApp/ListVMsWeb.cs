using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using vmware.samples.common;
using vmware.samples.common.authentication;
using vmware.vapi.bindings;
using vmware.vcenter;

namespace ListVMsWebApp
{
    public class ListVMsWeb : SamplesBase
    {
        BulletedList bulletedList;
        VM vmService;
        public ListVMsWeb(BulletedList bulletedList, string server,
            string username, string password, bool skipServerVerification)
        {
            this.bulletedList = bulletedList;
            this.Server = server;
            this.UserName = username;
            this.Password = password;
            this.SkipServerVerification = skipServerVerification;
        }

        public override void Cleanup()
        {
            // No cleanup
        }

        public override async void Run()
        {
            SetupSslTrustForServer();
            // Login
            this.VapiAuthHelper = new VapiAuthenticationHelper();

            this.SessionStubConfiguration =
                await this.VapiAuthHelper.LoginByUsernameAndPasswordAsync(
                    this.Server, this.UserName, this.Password);

            this.vmService =
                this.VapiAuthHelper.StubFactory.CreateStub<VM>(this.SessionStubConfiguration);

            List<VMTypes.Summary> vmList = await vmService.ListAsync(new VMTypes.FilterSpec());
            List<string> vmNamesList = new List<string>();
            foreach (VMTypes.Summary summary in vmList)
            {
                vmNamesList.Add(summary.GetName());
            }
            this.bulletedList.DataSource = vmNamesList;
            this.bulletedList.DataBind();
        }
    }
}