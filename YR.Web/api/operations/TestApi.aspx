<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestApi.aspx.cs" Inherits="YR.Web.api.partner_v2.TestApi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>api接口测试</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin:auto;width:80%;">
    
        <br />
        <asp:Label ID="Label1" runat="server" Text="接口名称"></asp:Label>
        <asp:TextBox ID="methodTxt" runat="server" style="width:100%;"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="接口参数"></asp:Label>
        <asp:TextBox ID="argsTxt" runat="server" TextMode="MultiLine" style="width:100%;height:50px;"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="调用" style="width:80px;height:40px;text-align:center;margin:auto;display:block;" />
        <br />
        <br />
        <asp:Label ID="Label3" runat="server" Text="返回结果"></asp:Label>
        <br />
        <br />
        <asp:Literal ID="resultLit" runat="server"></asp:Literal>
    
    </div>
    </form>
</body>
</html>
