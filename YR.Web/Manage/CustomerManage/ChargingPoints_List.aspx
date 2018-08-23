<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChargingPoints_List.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.ChargingPoints_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>充电桩管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
        })

        //新增
        function add() {
            var url = "/Manage/CustomerManage/ChargingPoints_Form.aspx";
            top.openDialog(url, 'ChargingPoints_Form', '充电桩信息 - 添加', 710, 500, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/CustomerManage/ChargingPoints_Form.aspx?key=" + key;
                top.openDialog(url, 'ChargingPoints_Form', '充电桩信息 - 编辑', 710, 500, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=充电桩管理&tableName=YR_ChargingPoints&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

        // 充电桩控制
        function VehicleControl(type, isTrue, key) {
            var parm = '&charging=charging&type=' + type + '&opr=' + isTrue + '&id=' + key;
            showConfirmMsg('注：您确认要控制当前充电桩吗？', function (r) {
                if (r) {
                    getAjax('../VehicleManage/VehicleControl.ashx', parm, function (rs) {
                        if (rs.toLowerCase() == 'true') {
                            showTipsMsg("操作成功！", 2000, 4);
                            windowload();
                        }
                        else {
                            showTipsMsg("<span style='color:red'>操作失败，请稍后重试！</span>", 4000, 5);
                        }
                    });
                }
            });
        }

        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
        }

        function callback() {
            publicobjcss();
            $.each($(".tdStation"), function (i,station) {
                var id = $(station).attr("id")
                if (id != "tdStation_") {
                    id = id.replace("tdStation_", "");
                    $.each($("#selStation option"), function (j,option) {
                        if (id == $(option).val()) {
                            $(station).html($(option).text());
                        }
                    });
                }
            });
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
            充电桩管理
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
                        所属站点
                    </th>
                    <td>
                        <select id="selStation" runat="server" style="width:134px"></select>
                    </td>
                    <th>
                        充电桩名称
                    </th>
                    <td>
                        <input type="text" id="txt_Search" runat="server" placeholder="充电桩名称" tabindex="1" autocomplete="on" style="width:130px;"/>
                    </td>
                    <th>
                        手机号
                    </th>
                    <td>
                        <input type="text" id="txtVehicleMobile" runat="server" placeholder="手机号" tabindex="1" autocomplete="off" style="width:156px;"/>
                    </td>
                </tr>
                <tr>
                    <th>
                        使用状态
                    </th>
                    <td>
                        <select id="selUseState" runat="server" style="width:134px">
                            <option value="-1">全部</option>
                            <option value="1">空闲</option>
                            <option value="2">预约</option>
                            <option value="3">使用中</option>
                        </select>
                    </td>
                    <th>
                        状态
                    </th>
                    <td>
                        <select id="selVehicleState" runat="server" style="width:134px">
                            <option value="-1">全部</option>
                            <option value="1">可用</option>
	                        <option value="2">不可用</option>
	                        <option value="3">维修</option>
	                        <option value="4">丢失</option>
	                        <option value="5">其它</option>
                        </select>
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
                    <td style="width: 30px; text-align: center;">选择</td>
                    <td style="width: 100px; text-align: center;">充电桩名称</td>
                    <td style="width: 50px; text-align: center;">所属站点</td>
                    <td style="width: 80px;text-align: center;">IMEI</td>
                    <td style="width: 80px;text-align: center;">手机号</td>
                    <td style="width: 40px;text-align: center;">使用状态</td>
                    <td style="width: 40px;text-align: center;">充电桩状态</td>
                    <td style="width: 40px;text-align: center;">在线状态</td>
                    <td style="width: 100px;text-align: center;">经纬度</td>
                    <td style="width: 80px;text-align: center;">最近更新</td>
                    <td style="width: 120px;text-align: center;">操作</td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("Name")%>
                            </td>
                            <td style="text-align: left;" id="tdStation_<%#Eval("StationID")%>" class="tdStation">
                                <asp:Label ID="lblStation" runat="server" Text='<%#Eval("StationID")%>'></asp:Label>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("IMEI")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Phone")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleUseState), Convert.ToInt32(Eval("UseState")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleState), Convert.ToInt32(Eval("State")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#int.Parse(Eval("diffminutes").ToString())<5 ? "<font color='green'>在线</font>" : "<font color='red'>离线</font>"%>
                            </td>
                            <td style="text-align: left;" title="<%#Eval("Address")%>">
                                <%#Eval("Longitude")%>,<%#Eval("Latitude")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("LastUpdateTime")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                            <a href="#" style="text-decoration: underline;color: Blue;" onclick="VehicleControl(1,'false','<%#Eval("ID")%>');">开电源</a>
                            <a href="#" style="text-decoration: underline;color: Blue;margin-left:5px;" onclick="VehicleControl(1,'true','<%#Eval("ID")%>');">关电源</a>
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
