<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ListVMsWebApp.ListVMsForm" Async="true"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>List VMs WebApp Sample</title>
</head>
<body>
    <h1 style="z-index: 1; left: 26px; top: 15px; position: absolute; height: 36px; width: 1150px">List VMs on a vCenter Server</h1>
    <form id="form1" runat="server">
        <div>
        <p style="width: 132px; z-index: 1; left: 10px; top: 15px; position: absolute; height: 19px;">
            <asp:Label ID="lblServer" runat="server" style="z-index: 1; left: 16px; top: 51px; position: absolute; margin-right: 295px; width: 270px;" Text="vCenter Server (Hostname / IP Address)"></asp:Label>
            <asp:Label ID="lblUsername" runat="server" style="z-index: 1; left: 319px; top: 54px; position: absolute; height: 20px; width: 224px; right: -411px; margin-top: 0px" Text="Username"></asp:Label>
            <asp:Label ID="lblPassword" runat="server" style="z-index: 1; left: 589px; top: 51px; position: absolute; width: 187px" Text="Password"></asp:Label>
        </p>
        <p style="height: 140px; width: 1213px">
            <asp:CheckBox ID="chkbSkipServerVerification" runat="server" style="z-index: 1; left: 23px; top: 160px; position: absolute" Text="Skip Server Certificate Verification" />
            </p>
            <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" style="z-index: 1; top: 195px; position: absolute; width: 125px; left: 28px; height: 26px" Text="Submit" />
        <asp:BulletedList ID="BulletedList1" runat="server" style="z-index: 1; left: 11px; top: 265px; position: absolute; height: 19px; width: 1134px" BulletStyle="Square">
        </asp:BulletedList>
        <asp:TextBox ID="txtbUsername" runat="server" style="z-index: 1; left: 332px; top: 115px; position: absolute; width: 218px"></asp:TextBox>
        <asp:TextBox ID="txtbPassword" runat="server" style="z-index: 1; left: 600px; top: 111px; position: absolute; width: 188px" TextMode="Password"></asp:TextBox>
        <asp:TextBox ID="txtbServer" runat="server" style="z-index: 1; left: 25px; top: 117px; position: absolute; right: 791px; width: 267px;"></asp:TextBox>
        </div>


    </form>
</body>
</html>
