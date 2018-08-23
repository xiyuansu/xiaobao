<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Platform_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Platform_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>平台管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })

        //新增
        function add() {
            var url = "/Manage/VehicleManage/Platform_Form.aspx";
            top.openDialog(url, 'Platform_Form', '平台信息 - 添加', 710, 300, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/Platform_Form.aspx?key=" + key;
                top.openDialog(url, 'Platform_Form', '平台信息 - 编辑', 710, 300, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=平台管理&tableName=YR_VehiclePlatform&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

        // 详细信息
        function detail() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/Platform_Form.aspx?key=" + key;
                top.openDialog(url, 'Platform_Form', '平台信息 - 详细', 710, 300, 50, 50);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2"> 
        <ContentTemplate>
    <div class="btnbartitle">
        <div>
            平台管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div style="float: left;">
            <table class="tabCondition">
                <tr>
                    <th>
                        平台名称
                    </th>
                    <td>
                        <input type="text" id="txtPlatform" runat="server" placeholder="平台名称" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        平台协议代码
                    </th>
                    <td>
                        <input type="text" id="txtCode" runat="server" placeholder="平台协议代码" tabindex="1" autocomplete="off" />
                    </td>
                    <td colspan="4">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton><asp:LinkButton ID="lbtInit" runat="server" class="button green"
            OnClick="lbtInit_Click"><span class="icon-botton">
        </span>重 置</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 100px; text-align: center;">
                        平台名称
                    </td>
                    <td style="width: 100px; text-align: center;">
                        平台协议代码
                    </td>
                    <td style="width: 200px; text-align: center;">
                        平台说明
                    </td>
                    <td style="width: 100px; text-align: center;">
                        显示次序
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td style="width: 20px; text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="width:100px; text-align: center;">
                                <%#Eval("Platform")%>
                            </td>
                            <td style="width:100px; text-align: center;">
                                <%#Eval("Code")%>
                            </td>
                            <td style="width:200px; text-align: center;">
                                <%#Eval("Memo")%>
                            </td>
                            <td style="width:100px; text-align: center;">
                                <%#Eval("Sort")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='12' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
            </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
