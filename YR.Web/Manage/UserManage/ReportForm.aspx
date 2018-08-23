<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportForm.aspx.cs" Inherits="YR.Web.Manage.UserManage.ReportForm" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>统计报表</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .grid thead td {background:none;}
    </style>
    <script type="text/javascript">
        $(function () {
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();
        })
        function Export(){
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
            统计报表
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div>
            <table class="tabCondition">
                <tr>
                    <th>开始日期</th>
                    <td>
                        <input id="txtStartDate" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" placeholder="开始日期" tabindex="1" autocomplete="off" />
                        
                    </td>
                    <th>结束时间</th>
                    <td>
                        <input id="txtEndDate" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                    <td colspan="10">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                            <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>
                            查 询
                        </asp:LinkButton>
                        <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                            <span class="icon-botton"></span>重 置
                        </asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 40px;text-align: center;">日期</td>
                    <td style="width: 40px;text-align: center;">意向单数/<br />转化率</td>
                    <td style="width: 40px;text-align: center;">预订次数/<br />预订率</td>
                    <td style="width: 40px;text-align: center;">付订单数/<br />付订率</td>
                    <td style="width: 40px;text-align: center;">付订车数/<br />付订车数比</td>
                    <td style="width: 40px;text-align: center;">取车单数/<br />取车率</td>
                    <td style="width: 40px;text-align: center;">行驶单数/<br />行驶率</td>
                    <td style="width: 40px;text-align: center;">同意单数/<br />同意率</td>
                    <td style="width: 40px;text-align: center;">失败单数/<br />失败率</td>
                    <td style="width: 40px;text-align: center;">付款单数/<br />付款率</td>
                    <td style="width: 40px;text-align: center;">违押单数/<br />违押率</td>
                    <td style="width: 40px;text-align: center;">退款单数/<br />退款率</td>
                    <td style="width: 40px;text-align: center;">日车均订单数/<br />日车均收入(出租)</td>
                    <td style="width: 40px;text-align: center;">还车单数/<br />还车单率</td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <%#Eval("ReportTime")%></td>
                            <td style="text-align: center;">
                                <%#Eval("OrderCount")%>/<%#Eval("ConversionRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("OrderCount")%>/<%#Eval("ReservationRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("OrderCount")%>/<%#Eval("SubscriptionRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("VehicleCount")%>/<%#Eval("SubscriptionVehicleRatio")%></td>
                            <td style="text-align: center;">
                                <%#Eval("StartCount")%>/<%#Eval("StartRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("NormalCount")%>/<%#Eval("DrivingRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("OrderCount")%>/<%#Eval("ConsentRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("AbnormalCount")%>/<%#Eval("FailureRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("PaymentCount")%>/<%#Eval("PaymentRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("OrderCount")%>/<%#Eval("ViolationRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("CancelCount")%>/<%#Eval("RefundRate")%></td>
                            <td style="text-align: center;">
                                <%#Eval("AverageDailyOrder")%>/ <%#Eval("AverageDailyIncome")%>
                            <td style="text-align: center;">
                                <%#Eval("EndCount")%>/<%#Eval("ReturnRate")%></td>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='10' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
        </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
