<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleFault_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleFault_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>故障管理</title>
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

        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/VehicleFault_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleFault_Form', '车辆故障 - 编辑', 700, 350, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=车辆故障信息&tableName=YR_VehicleFault&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
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
            故障管理
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
                        车辆名称
                    </th>
                    <td>
                        <input type="text" id="txtVehicleName" runat="server" placeholder="车辆名称" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        会员
                    </th>
                    <td>
                        <input type="text" id="txtUserName" runat="server" placeholder="用户名" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        触发类型
                    </th>
                    <td>
                        <select id="selTriggerType" runat="server">
                        <option value="-1">全部</option>
                        <option value="1">用户自报</option>
                        <option value="2">系统</option>
                        </select>
                    </td>
                    <th>
                        故障类型
                    </th>
                    <td>
                        <input type="text" id="txtFaultType" runat="server" placeholder="故障类型" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        创建时间
                    </th>
                    <td>
                        <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="10">
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
                    <td style="width: 60px; text-align: center;">
                        车辆名称
                    </td>
                    <td style="width: 60px; text-align: center;">
                        会员姓名
                    </td>
                    <td style="width: 60px; text-align: center;">
                        触发类型
                    </td>
                    <td style="width: 60px; text-align: center;">
                        故障类型
                    </td>
                    <td style="width: 60px; text-align: center;">
                        处理状态
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
                                <%#Eval("SubmitUserName")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                    <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleFaultTriggerType), Convert.ToInt32(Eval("TriggerType")))%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("FaultType")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("FaultDoState").ToString()=="1"?"未处理":"已处理"%>
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
