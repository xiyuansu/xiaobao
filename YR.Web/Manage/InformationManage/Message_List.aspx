<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Message_List.aspx.cs" Inherits="YR.Web.Manage.InfomationManage.Message_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>消息管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {

        })

        //编辑
        function send() {
            var url = "/Manage/InformationManage/Message_Send.aspx";
            top.openDialog(url, 'Pits_Form', '短消息 - 添加', 700, 350, 50, 50);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="btnbartitle">
        <div>
            消息管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="float: left;">
            标题：
            <input type="text" id="txt_Search" class="txtSearch SearchImg" runat="server" style="width: 100px;" />
            <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton>
        </div>
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 60px; text-align: center;">
                        消息标题
                    </td>
                    <td style="width: 60px; text-align: center;">
                        消息类型
                    </td>
                    <td style="width: 60px; text-align: center;">
                        模块类型
                    </td>
                    <td style="width: 60px; text-align: center;">
                        操作者
                    </td>
                    <td style="width: 60px; text-align: center;">
                        发布时间
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="width: 20px; text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("MessageTitle")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.MessageType), Convert.ToInt32(Eval("MessageType")))%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.MessageModuleType), Convert.ToInt32(Eval("ModuleType")))%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("User_Name")%>
                            </td>
                            <td style="width: 200px; text-align: center;">
                                <%#Eval("ReleaseTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='6' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
    </form>
</body>
</html>
