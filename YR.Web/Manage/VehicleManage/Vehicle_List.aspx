<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Vehicle_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Vehicle_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>车辆管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .link_1{display:none;}
    </style>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();
        })

        //新增
        function add() {
            var url = "/Manage/VehicleManage/Vehicle_Form.aspx";
            top.openDialog(url, 'Vehicle_Form', '车辆信息 - 添加', 710, 500, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/Vehicle_Form.aspx?key=" + key;
                top.openDialog(url, 'Vehicle_Form', '车辆信息 - 编辑', 710, 500, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=车辆管理&tableName=YR_Vehicles&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

        // 车辆控制
        function VehicleControl(type, isTrue, key) {
            var parm = '&type=' + type + '&opr=' + isTrue + '&id=' + key;
            showConfirmMsg('注：您确认要控制当前车辆吗？', function (r) {
                if (r) {
                    getAjax('VehicleControl.ashx', parm, function (rs) {
                        if (rs.toLowerCase() == 'true') {
                            showTipsMsg("操作成功！", 2000, 4);
                            load();
                        }
                        else {
                            showTipsMsg("<span style='color:red'>操作失败，请稍后重试！</span>", 4000, 5);
                        }
                    });
                }
            });
        }

        //车辆订单
        function lookOrder(key, usestate) {
            if (usestate == 2 || usestate == 3) {
                var url = "/Manage/VehicleManage/VehicleUse_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleUse_Form', '车辆使用信息 - 详细', 700, 400, 50, 50);
            }
            else if (usestate == 4) {
                var url = "/Manage/VehicleManage/VehicleUse_Operator_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleUse_Form', '运维用车信息 - 详细', 700, 350, 50, 50);
            }
        }

        //查看地图
        function lookMap(key) {
            //var url = "/Manage/VehicleManage/VehicleMap.aspx?VehicleID=" + key;
            var url = "/Manage/VehicleManage/VehicleTrace.html?vid=" + key;
            top.openDialog(url, 'VehicleMap', '车辆定位信息', 800, 700, 50, 50);
        }

        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
        }

        //导出
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
            车辆管理
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
                        <input type="text" id="txt_Search" runat="server" placeholder="车辆名称" tabindex="1" autocomplete="off" />
                    </td>

                    <th>
                        车辆GPS编号
                    </th>
                    <td>
                        <input type="text" id="txtVehicleGPSNum" runat="server" placeholder="车辆GPS编号" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        在线状态
                    </th>
                    <td>
                        <select id="selLiveState" runat="server" style="width:134px">
                            <option value="-1">全部</option>
                            <option value="1">在线</option>
                            <option value="2">离线</option>
                        </select>
                    </td>
                    <th>无单车辆</th>
                    <td>
                        <input type="text" id="txtNoOrderDays" runat="server" class="txt" style="width: 70px" datacol="yes" checkexpession="NumOrNull" placeholder="请输入无单天数" maxlength="2" tabindex="1" autocomplete="off" />天
                    </td>
                </tr>
                <tr>
                    <th>
                        所属平台
                    </th>
                    <td>
                        <select id="selPlatform" runat="server" style="width:134px"></select>
                    </td>
                    <th>
                        使用状态
                    </th>
                    <td>
                        <select id="selUseState" runat="server" style="width:134px">
                            <option value="-1">全部</option>
                            <option value="1">空闲</option>
                            <option value="3">预约中</option>
                            <option value="2">客户使用</option>
                            <option value="4">运维操作</option>
                        </select>
                    </td>
                    <th>
                        车辆状态
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
                    <td style="width: 90px; text-align: center;">车辆名称</td>
                    <td style="width: 40px; text-align: center;">型号</td>
                    <td style="width: 50px; text-align: center;">车牌号</td>
                    <td style="width: 40px; text-align: center;">电量</td>
                    <td style="width: 60px; text-align: center;">盒子类型</td>
                    <td style="width: 100px; text-align: center;">盒子号</td>
                    <td style="width: 40px; text-align: center;">城市</td>
                    <td style="width: 40px;text-align: center;">使用状态</td>
                    <td style="width: 40px;text-align: center;">车辆状态</td>
                    <td style="width: 40px;text-align: center;">电源状态</td>
                    <td style="width: 40px;text-align: center;">在线状态</td>
                    <td style="width: 80px;text-align: center;">最近更新</td>
                    <td style="width: 100px;text-align: center;">操作</td>
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
                                <a class="link_<%#Eval("UseState")%>" href="#" onclick="lookOrder('<%#Eval("ID")%>',<%#Eval("UseState")%>)">
                                <img src="../../Themes/Images/ico_vehicle.png" style="width: 15px; height: 15px;" alt="查看使用情况" /></a>
                                <a href="#" onclick="lookMap('<%#Eval("ID")%>')">
                                <img alt="" src="../../Themes/Images/32/map.png" style="width: 15px; height: 15px; margin-left: 5px" /></a>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("modelName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("LicenseNumber")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Electricity").ToString()==""?"":""+(int)(decimal.Parse(Eval("Electricity").ToString()))+"%"%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Platform")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("VehicleGPSNum")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("VehicleCityName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleUseState), Convert.ToInt32(Eval("UseState")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.VehicleState), Convert.ToInt32(Eval("VehicleState")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("LockState").ToString() == "lock" ? "<font color='red'>已关</font>" : "<font color='green'>已开</font>"%>
                            </td>
                            <td style="text-align: center; display:none">
                                <%#Eval("LightState").ToString() == "on" ? "<font color='green'>已开灯</font>" : "<font color='red'>已关灯</font>"%>
                            </td>
                            <td style="text-align: center;">
                                <%#int.Parse(Eval("diffminutes").ToString())<10 ? "<font color='green'>在线</font>" : "<font color='red'>离线</font>"%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("LastUpdateTime")%>
                            </td>
                            <td style="text-align: center;">
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
