<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestApi.aspx.cs" Inherits="YR.Web.api.app.TestApi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>api接口测试</title>
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin:auto;width:80%;">
       <select name="selectTypeName" id="selectTypeName">
	        <option value="-1">请选择接口</option>
	        <option value="ReturnVehicle">还车</option>
        </select>
        <br />
        <asp:Label ID="Label1" runat="server" Text="接口名称"></asp:Label>
        <asp:TextBox ID="methodTxt" runat="server" style="width:100%;"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="接口参数"></asp:Label>
        <asp:TextBox ID="argsTxt" runat="server" style="width:100%;"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="调用" style="width:80px;height:40px;text-align:center;margin:auto;display:block;" />
        <br />
        <asp:RadioButtonList ID="ClientList" runat="server">
            <asp:ListItem Value="android" Selected="True">android</asp:ListItem>
            <asp:ListItem Value="iOS">iOS</asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:Label ID="Label3" runat="server" Text="返回结果"></asp:Label>
        <br />
        <br />
        <asp:Literal ID="resultLit" runat="server"></asp:Literal>
    
    </div>
    </form>
    <script type="text/javascript">
        $(function () {
            $("#selectTypeName").change(function () {
                var name = $(this).val();
                $("#Label3").empty();
                if (name != "-1") {
                    $("#methodTxt").val(name);
                    $("#argsTxt").val("UID=1d520af2-064f-4d82-b3b1-7796de865caa");
                } else {
                    $("#methodTxt").val("");
                    $("#argsTxt").val("");
                }
            });
        })
    </script>
</body>
</html>
