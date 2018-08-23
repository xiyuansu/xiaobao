<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceStation_List.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.ServiceStation_List" %>
<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>充电站管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#txtName,#txtAddress,#txtLongitude,#txtContacts,#txtContactsTel,#txtManager,#txtManagerTel').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })
        //新增
        function add() {
            var url = "/Manage/CustomerManage/ServiceStation_Form.aspx";
            top.openDialog(url, 'VehicleParking_Form', '充电站信息 - 添加', 710, 500, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/CustomerManage/ServiceStation_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleParking_Form', '充电站信息 - 编辑', 710, 500, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=充电站管理&tableName=YR_VehicleParking&pkName=ID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        function lookVehicle(key, name) {
            var url = "/Manage/VehicleManage/ParkingVehicle_List.aspx?key=" + key + "&name=" + name;
            top.openDialog(url, 'ParkingVehicle_List', '充电站车辆信息 - 编辑', 710, 500, 50, 50);
        }
        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
        }

        function callback() {
            publicobjcss();
            $.each($(".tdCustomer"), function (i, station) {
                var id = $(station).attr("id")
                if (id != "tdCustomer_") {
                    id = id.replace("tdCustomer_", "");
                    $.each($("#selCustomer option"), function (j, option) {
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
                    <div>充电站管理</div>
                </div>
                <div class="btnbarcontetn">
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>充电站名称</th>
                                <td>
                                    <input type="text" id="txtName" runat="server" placeholder="充电站名称" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>充电站地址</th>
                                <td>
                                    <input type="text" id="txtAddress" runat="server" placeholder="充电站地址" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>经纬度</th>
                                <td>
                                    <input type="text" id="txtLongitude" runat="server" placeholder="经纬度" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>联系人</th>
                                <td>
                                    <input type="text" id="txtContacts" runat="server" placeholder="联系人" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>联系人电话</th>
                                <td>
                                    <input type="text" id="txtContactsTel" runat="server" style="width:158px" placeholder="联系人电话" tabindex="1"  autocomplete="on"/>
                                </td>
                            </tr>
                            <tr>
                                <th>客户</th>
                                <td>
                                    <select id="selCustomer" runat="server" style="width:94px"></select>
                                </td>
                                <th>充电站状态</th>
                                <td>
                                    <select id="selState" runat="server" style="width:94px">
                                        <option value="-1">全部</option>
                                        <option value="1">有效</option>
	                                    <option value="0">无效</option>
                                    </select>
                                </td>
                                <th>负责人</th>
                                <td>
                                    <input type="text" id="txtManager" runat="server" placeholder="负责人" tabindex="1" style="width:90px;"/>
                                </td>
                                <th>负责人电话</th>
                                <td>
                                    <input type="text" id="txtManagerTel" runat="server" placeholder="负责人电话" tabindex="1"  style="width:90px;"/>
                                </td>
                                <th>创建时间</th>
                                <td>
                                    <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                                    <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                                </td>
                            </tr>
                            <tr style="text-align: center">
                                <td colspan="10">
                                    <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                                        <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>
                                        查 询
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                                        <span class="icon-botton"></span>
                                        重 置
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
                                <td style="width: 30px; text-align: center;">选择</td>
                                <td style="width: 100px; text-align: center;">充电站名称</td>
                                <td style="width: 100px; text-align: center;">所属客户</td>
                                <td style="width: 40px; text-align: center;">联系人</td>
                                <td style="width:60px; text-align: center;">联系人电话</td>
                                <td style="width: 160px; text-align: center;">地址</td>
                                <td style="width: 80px; text-align: center;">经度</td>
                                <td style="width: 80px; text-align: center;">纬度</td>
                                <td style="width: 40px;text-align: center;">充电站状态</td>
                                <td style="width: 100px;text-align: center;">创建时间</td>
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
                                            <%--<a class="link link_<%#Eval("VehicleTotal")%>" href="#" onclick="lookVehicle('<%#Eval("ID")%>','<%#Eval("Name")%>')"><img src="../../Themes/Images/113415814083796368.png" style="width:15px;height:15px;" alt="查看车辆"/></a>--%>
                                        </td>
                                        <td style="text-align: center;" id="tdCustomer_<%#Eval("CustomerID")%>" class="tdCustomer">
                                            <asp:Label ID="lblCustomer" runat="server" Text='<%#Eval("CustomerID")%>'></asp:Label>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Contacts")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("ContactTel")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Address")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Longitude")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Latitude")%>
                                        </td>
                                        <%--<td style="text-align: center;">
                                            <%#Eval("Total")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("UsableCount")%>
                                        </td>--%>
                                        <td style="text-align: center;">
                                            <%#Eval("State").ToString() == "1" ? "有效" : "<font color='red'>无效</font>"%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CreateTime")%>
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
                    </table>
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
