<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleAlarm_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleAlarm_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>车辆报警管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#txtVehicleName').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })

        //详情
        function detail() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/VehicleAlarm_Form.aspx?action=view&key=" + key;
                top.openDialog(url, 'VehicleFault_Form', '车辆报警 - 详情', 700, 350, 50, 50);
            }
        }

        //查看地图
        function lookMap(key) {
            var url = "/Manage/VehicleManage/VehicleTrace.html?vid=" + key;
            top.openDialog(url, 'VehicleMap', '车辆定位信息', 900, 550, 50, 50);
        }

        //报警处理
        function handle() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var AlarmStatus = $("#" + key).val();
                if (AlarmStatus == "1")
                {
                    showWarningMsg("此报警数据已被处理");
                    return;
                }
                var url = "/Manage/VehicleManage/VehicleAlarm_Form.aspx?action=edit&key=" + key;
                top.openDialog(url, 'VehicleFault_Form', '车辆报警 - 报警处理', 700, 350, 50, 50);
            }
        }

        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
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
            报警管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        <div>
            <table class="tabCondition">
                <tr>
                    <th>
                        车辆名称
                    </th>
                    <td>
                        <input type="text" id="txtVehicleName" runat="server" placeholder="车辆名称" tabindex="1" autocomplete="on" style="width:90px;"/>
                    </td>
                    <th>
                        异常类型
                    </th>
                    <td>
                        <select id="selAlarmType" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">无单移动</option>
                            <option value="2">断电</option>
                            <%--<option value="3">倒地</option>--%>
                            <option value="4">越界</option>
                            <option value="5">离线</option>
                            <option value="6">超速</option>
                            <option value="7">振动</option>
                        </select>
                    </td>
                    <th>
                        处理状态
                    </th>
                    <td>
                        <select id="selAlarmStatus" runat="server">
                            <option value="-1">全部</option>
                            <option value="0">未处理</option>
                            <option value="1">已处理</option>
                        </select>
                    </td>
                    <th>
                        报警时间
                    </th>
                    <td>
                        <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                     <td>
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                            <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>查 询
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
                    <td style="width: 5%; text-align: center;">选择</td>
                    <td style="width:10%; text-align: center;">
                        车辆名称
                    </td>
                    <td style="width:15%; text-align: center;">
                        盒子号
                    </td>
                    <td style="width:10%; text-align: center;">
                        报警类别
                    </td>
                    <td style="width:5%; text-align: center;">
                        速度
                    </td>
                    <td style="width:15%;text-align: center;">
                        报警时间
                    </td>
                    <td style="width:10%;text-align: center;">
                        处理状态
                    </td>
                    <td style="width:15%;text-align: center;">
                        处理时间
                    </td>
                    <td style="width:15%;text-align: center;">
                        操作人员
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                <input type="hidden" id="<%#Eval("ID")%>" value="<%#Eval("AlarmStatus")%>" />
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("VehicleName")%>
                                <a href="#" onclick="lookMap('<%#Eval("VehicleID")%>')">
                                <img alt="" src="../../Themes/Images/32/map.png" style="width: 15px; height: 15px; margin-left: 5px" /></a>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("IMEI")%>
                            </td>
                            <td style="text-align: center;">
                                <%#GetAlarmType(Eval("AlarmType").ToString())%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Speed")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("AlarmTime")%>
                            </td>
                            <td style="text-align: center;">
                                <%#GetAlarmStatus(Eval("AlarmStatus").ToString())%>
                            </td>
                            <td style=" text-align: center;">
                                <%#Eval("OperateTime")%>
                            </td>
                            <td style="text-align: center;">
                                <%#GetOperator(Eval("OperatorID").ToString(),Eval("OperatorType").ToString())%>
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
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
            </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
