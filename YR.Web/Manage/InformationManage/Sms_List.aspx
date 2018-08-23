<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sms_List.aspx.cs" Inherits="YR.Web.Manage.InfomationManage.Sms_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>短信管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })

        //新增
        function add() {
            var url = "/Manage/InformationManage/Sms_Form.aspx";
            top.openDialog(url, 'Pits_Form', '短消息 - 添加', 700, 350, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/InformationManage/Sms_Form.aspx?key=" + key;
                top.openDialog(url, 'Pits_Form', '短消息 - 编辑', 700, 350, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

        //详细信息
        function detail() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/Vehicle_Info.aspx?key=" + key;
                top.openDialog(url, 'Vehicle_Info', '车辆信息 - 详细', 700, 350, 50, 50);
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="btnbartitle">
        <div>
            短信管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="float: left;">
            手机号：
            <input type="text" id="txt_Search" runat="server" style="width: 100px;" />
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
                    <td style="width: 20px; text-align: center;"> 选择</td>
                    <td style="width: 60px; text-align: center;">接收者</td>
                    <td style="width: 120px; text-align: center;">发送内容</td>
                    <td style="width: 60px; text-align: center;">发送状态</td>
                    <td style="width: 60px; text-align: center;">消息类型</td>
                    <td style="width: 60px; text-align: center;">发送时间</td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Receiver")%>
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("AllMessage")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.SMSMessageState), Convert.ToInt32(Eval("MessageState")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.SMSMessageType), Convert.ToInt32(Eval("MessageType")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("SendTime")%>
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
