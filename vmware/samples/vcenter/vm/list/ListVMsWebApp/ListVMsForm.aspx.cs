using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ListVMsWebApp
{
    public partial class ListVMsForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string server = txtbServer.Text;
            string username = txtbUsername.Text;
            string password = txtbPassword.Text;
            bool skipServerVerification = chkbSkipServerVerification.Checked;
            ListVMsWeb listVMsWeb =
                new ListVMsWeb(BulletedList1, server, username, password,
                  skipServerVerification);
            listVMsWeb.Run();
        }
    }
}