<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Customer_List.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.Customer_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>客户管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".link_0").hide();
            $(".RetCarFlag_0").html("不支持");
            $(".RetCarFlag_1").html("支持");
        })
        //新增
        function add() {
            var url = "/Manage/CustomerManage/Customer_Form.aspx";
            top.openDialog(url, 'Hotel_Form', '客户信息 - 添加', 700, 350, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/CustomerManage/Customer_Form.aspx?key=" + key;
                top.openDialog(url, 'Hotel_Form', '客户信息 - 编辑', 700, 350, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=客户信息&tableName=YR_Customer&pkName=ID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        //添加充电桩
        function addParking(id, name, address) {
            var url = "/Manage/VehicleManage/VehicleParking_Form.aspx?hotelID=" + id + "&hotelName=" + name + "&address=" + address;
            top.openDialog(url, 'VehicleParking_Form', '充电桩信息 - 添加', 710, 500, 50, 50);
        }
        //查看客户车辆
        function lookVehicle(key, name) {
            var url = "/Manage/VehicleManage/HotelVehicle_List.html?key=" + key + "&name=" + escape(name);
            top.openDialog(url, 'HotelVehicle_List', '客户车辆信息', 1020, 550, 50, 50);
        }
        function SetFlag() {
            $(".RetCarFlag_0").html("不支持");
            $(".RetCarFlag_1").html("支持");
            $(".link_0").hide();
            publicobjcss();
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
                    <div>客户管理</div>
                </div>
                <div class="btnbarcontetn">
                    <div style="text-align: right"><uc1:LoadButton ID="LoadButton1" runat="server" /></div>
                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>
                                    名称
                                </th>
                                <td>
                                    <input type="text" id="txtName" runat="server" placeholder="名称" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>
                                    地址
                                </th>
                                <td>
                                    <input type="text" id="txtAddress" runat="server" placeholder="地址" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>
                                    联系电话
                                </th>
                                <td>
                                    <input type="text" id="txtPhone" runat="server" placeholder="联系电话" tabindex="1" autocomplete="on" style="width:90px;"/>
                                </td>
                                <th>
                                    类型
                                </th>
                                <td>
                                    <select id="selCategory" runat="server" style="width:94px">
                                        <option value="-1">全部</option>
                                        <option value="1">酒店</option>
	                                    <option value="2">社区</option>
                                        <option value="3">写字楼</option>
                                        <option value="4">医院</option>
                                        <option value="5">景区</option>
                                        <option value="6">学校</option>
                                        <option value="7">企业单位</option>
                                        <option value="8">集团客户</option>
                                </select>
                                </td>
                                <th>
                                    创建时间
                                </th>
                                <td>
                                    <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                                    <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                                </td>
                            </tr>
                            <tr style="text-align: center">
                                <td colspan="10">
                                    <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>查 询</asp:LinkButton>
                                    <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click"><span class="icon-botton"></span>重 置</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="div-body">
                    <table id="table1" class="grid" singleselect="true">
                        <thead>
                            <tr>
                                <td style="width: 20px; text-align: center;">选择</td>
                                <td style="width: 80px; text-align: center;">名称</td>
                                <td style="width: 30px; text-align: center;">类型</td>
                                <%--<td style="width: 50px; text-align: center;">异地还车</td>--%>
                                <td style="width: 160px; text-align: center;">地址</td>
                                <td style="width: 60px; text-align: center;">联系电话</td>
                                <td style="width: 60px; text-align: center;">联系人</td>
                                <td style="width: 60px;text-align: center;">创建时间</td>
                                <%--<td style="width: 60px;text-align: center;">操作</td>--%>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                                <ItemTemplate>
                                    <tr id="tr_<%#Eval("ID")%>">
                                        <td style="text-align: center;">
                                            <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                        </td>
                                        <td>
                                            <%#Eval("Name")%><%--(<%#Eval("VehicleCount")%>)
                                            <a class="link link_<%#Eval("VehicleCount")%>" href="#" onclick="lookVehicle('<%#Eval("ID")%>','<%#Eval("Name")%>')"><img src="../../Themes/Images/113415814083796368.png" style="width:15px;height:15px;" alt="查看车辆"/></a>--%>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:Label ID="lblCategory" runat="server" Text='<%#Eval("Category")%>'></asp:Label>
                                        </td>
                                        <%--<td style="text-align: center;" class="RetCarFlag_<%#Eval("RetCarFlag")%>">
                                            <%#Eval("RetCarFlag")%>
                                        </td>--%>
                                        <td>
                                            <%#Eval("Address")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Tel")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Contacts")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CreateTime")%>
                                        </td>
                                        <%--<td style="text-align: center;">
                                           <a class="link" id="link_<%#Eval("ID")%>" href="#" style="text-decoration: underline;color: Blue;" onclick="addParking('<%#Eval("ID")%>','<%#Eval("Name")%>','<%#Eval("Address")%>')">添加充电桩</a>
                                        </td>--%>
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

