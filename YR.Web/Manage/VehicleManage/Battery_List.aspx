<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Battery_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Battery_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>电池管理</title>
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

        //更换电池
        function ReplaceBattery() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var url = "/Manage/VehicleManage/VehiclePitLog_Form.aspx?vid=" + key;
                top.openDialog(url, 'Vehicle_Info', '维修信息 - 详细', 700, 350, 50, 50);
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
            电池管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="float: left;">
                        电量
                        <input type="text" id="txtStartElectricity" runat="server" placeholder="开始电量"
                            tabindex="1" autocomplete="off">-
                        <input type="text" id="txtEndElectricity" runat="server" placeholder="结束电量"
                            tabindex="1" autocomplete="off">
                    
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton>
        </div>
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
    </div>
    <div class="div-body">
        <table id="table1" class="grid">
            <thead>
                <tr>
                    <td style="width: 20px; text-align: center;">
                        <label id="checkAllOff" onclick="CheckAllLine()" title="全选">
                            &nbsp;</label>
                    </td>
                    <td style="width: 60px; text-align: center;">
                        车辆名称
                    </td>
                    <td style="width: 60px; text-align: center;">
                        车主姓名
                    </td>
                    <td style="width: 60px; text-align: center;">
                        车牌号
                    </td>
                    <td style="width: 60px; text-align: center;">
                        车辆颜色
                    </td>
                    <td style="width: 60px; text-align: center;">
                        车辆排放
                    </td>
                    <td style="width: 100px; text-align: center;">
                        燃料方式
                    </td>
                    <td style="width: 80px;text-align: center;">
                        使用状态
                    </td>
                    <td style="width: 100px;text-align: center;">
                        车辆状态
                    </td>
                    <td style="width: 100px; text-align: center;">
                        电量(%)
                    </td>
                    <td style="width: 180px;text-align: center;">
                        创建时间
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
                                <%#Eval("VehicleName")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <%#Eval("VehicleOwnerName")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Eval("LicenseNumber")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("VehicleColor")%>
                            </td>
                            <td style="width: 200px; text-align: center;">
                                <%#Eval("Displacement")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("FuelStyle")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleUseState), Convert.ToInt32(Eval("UseState")))%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleState), Convert.ToInt32(Eval("VehicleState")))%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%# Eval("Electricity")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("CreateTime")%>
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
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
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
