<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleCommentsLog_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.VehicleCommentsLog_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>评论管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();
        })
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

        // 审核
        function audit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/VehicleCommentsLog_Audit.aspx?key=" + key;
                top.openDialog(url, 'User_Audit', '用户评论内容 - 审核', 700, 350, 50, 50);
            }
        }

        function Export() {
            __doPostBack('lbtExport', '');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="lbtExport" runat="server" class="button green" OnClick="lbtExport_Click">导出</asp:LinkButton>
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <div class="btnbartitle">
                    <div>
                        评论管理
                    </div>
                </div>
                <div class="btnbarcontetn">
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>

                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>用户
                                </th>
                                <td>
                                    <input type="text" id="txt_Search" runat="server" placeholder="昵称/真实姓名/手机号码" tabindex="1" autocomplete="off">
                                </td>
                                <th>车辆
                                </th>
                                <td>
                                    <input type="text" id="txtVehicleName" runat="server" placeholder="车辆名称" tabindex="1" autocomplete="off" />
                                </td>
                                <th>评论时间
                                </th>
                                <td>
                                    <input id="txtStartCommentTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                                        checkexpession="NotNull" style="width: 100px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCommentTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                                </td>
                                <th>评论状态
                                </th>
                                <td>
                                    <select id="selState" runat="server">
                                        <option value="-1">全部</option>
                                        <option value="1">新提交</option>
                                        <option value="2">审核成功</option>
                                        <option value="3">审核失败</option>
                                    </select>
                                </td>
                                <th>客户端
                                </th>
                                <td>
                                    <select id="selClientType" runat="server">
                                        <option value="-1">全部</option>
                                        <option value="iOS">iOS</option>
                                        <option value="Android">Android</option>
                                    </select>
                                </td>
                            </tr>
                            <tr style="text-align: center">
                                <td colspan="9">
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
                                <td style="width: 40px; text-align: center;">选择
                                </td>
                                <td style="width: 60px; text-align: center;">车辆名称
                                </td>
                                <td style="width: 90px; text-align: center;">盒子号
                                </td>
                                <td style="width: 80px; text-align: center;">会员手机号
                                </td>
                                <td style="width: 60px; text-align: center;">会员姓名
                                </td>
                                <td style="width: 60px; text-align: center;">会员昵称
                                </td>
                                <td style="width: 250px; text-align: center;">评论内容
                                </td>
                                <td style="width: 80px; text-align: center;">评论时间
                                </td>
                                <td style="width: 60px; text-align: center;">评论状态
                                </td>
                                <td style="width: 60px; text-align: center;">客户端
                                </td>
                            </tr>
                        </thead>
                        <tbody>

                            <asp:Repeater ID="rp_Item" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td style="text-align: center;">
                                            <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("LicenseNumber")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("VehicleGPSNum")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("BindPhone")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("RealName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("UserNickName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CommentsContent")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CommentsTime")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserRaiseReplyState), Convert.ToInt32(Eval("State")))%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("ClientType")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <% if (rp_Item != null)
                                       {
                                           if (rp_Item.Items.Count == 0)
                                           {
                                               Response.Write("<tr><td colspan='7' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                                           }
                                       } %>
                                </FooterTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <br />
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
