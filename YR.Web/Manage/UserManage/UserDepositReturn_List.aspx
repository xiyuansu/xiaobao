<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDepositReturn_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.UserDepositReturn_List" %>


<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>押金退款列表</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();

            $('#txt_Search').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })

        //处理申请
        function handle() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/UserDepositReturn_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleModel_Form', '申请信息 - 编辑', 800, 570, 50, 50);
            }
        }

        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
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
                        押金退款列表
                    </div>
                </div>
                <div class="btnbarcontetn">
                    <div style="float: left;">
                        用户手机号:<input type="text" id="txtTel" runat="server" style="width: 176px;" />
                        申请状态:
                        <select id="selState" class="Searchwhere1" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">待处理</option>
                            <option value="2">退款成功</option>
                            <option value="3">退款失败</option>
                        </select>
                        申请时间
                        <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"> </span>查 询</asp:LinkButton>
                    </div>
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                </div>
                <div class="div-body">
                    <table id="table1" class="grid" singleselect="true">
                        <thead>
                            <tr>
                                <td style="width: 10px; text-align: center;">选择</td>
                                <td style="width: 50px; text-align: center;">用户姓名</td>
                                <td style="width: 50px; text-align: center;">手机号</td>
                                <td style="width: 50px; text-align: center;">押金金额</td>
                                <td style="width: 50px; text-align: center;">押金支付方式</td>
                                <td style="width: 50px; text-align: center;">押金支付交易号</td>
                                <td style="width: 50px; text-align: center;">申请状态</td>
                                <td style="width: 50px; text-align: center;">创建时间</td>
                                <td style="width: 30px; text-align: center;">更新时间</td>
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
                                            <%#Eval("RealName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("BindPhone")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("DepositMoney")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialOperatorWay), Convert.ToInt32(Eval("DepositPayWay")))%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("DepositTradeNo")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#GetStateTxt(Eval("State").ToString())%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CreateTime")%>
                                        </td>
                                         <td style="text-align: center;">
                                            <%#Eval("UpdateTime")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <% if (rp_Item != null)
                                       {
                                           if (rp_Item.Items.Count == 0)
                                           {
                                               Response.Write("<tr><td colspan='5' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
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
                <asp:PostBackTrigger ControlID="PageControl1" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
