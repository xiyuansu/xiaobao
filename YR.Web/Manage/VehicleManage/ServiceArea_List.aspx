<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceArea_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.ServiceArea_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>服务范围管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })
        //新增
        function add() {
            var url = "/Manage/VehicleManage/ServiceArea_Form.aspx";
            top.openDialog(url, 'ChargingPies_Form', '服务范围信息 - 添加', 700, 400, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/ServiceArea_Form.aspx?key=" + key;
                top.openDialog(url, 'ChargingPies_Form', '服务范围信息 - 编辑', 700, 400, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=服务范围信息&tableName=YR_ServiceArea&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        function editPolyline(id,city,type) {
            var url = "/Manage/VehicleManage/map-edit-polyline.html?key=" + id+"&city="+city+"&type="+type;
            top.openDialog(url, 'map-edit-polyline', '设置范围', 1100, 550, 50, 50);
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
            服务范围管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        <div>
            <table class="tabCondition">
                <tr>
                    <th>城市</th>
                    <td><select id="selCity" runat="server"></select></td>
                    <th>类型</th>
                    <td>
                        <select id="selAreaType" runat="server">
		                    <option value="-1">----全部----</option>
		                    <option value="1">服务范围</option>
                            <option value="2">停车网点</option>
                            <option value="3">禁停区域</option>
                        </select>
                    </td>
                    <th>状态</th>
                    <td>
                        <select id="selStatus" runat="server">
		                    <option value="-1">----全部----</option>
		                    <option value="1">启用</option>
                            <option value="0">禁用</option>
                        </select>
                    </td>
                    <td>
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>查 询</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width:20px; text-align: center;">选择</td>
                    <td style="width:50px; text-align: center;">城市</td>
                    <td style="width:50px; text-align: center;">类型</td>
                    <td style="width:50px; text-align: center;">名称</td>
                    <td style="width:100px; text-align: center;">坐标点</td>
                    <td style="width:150px; text-align: center;">地址</td>
                    <td style="width:100px; text-align: center;">状态</td>
                    <td style="width:50px;text-align: center;">创建时间</td>
                    <td style="width:30px;text-align: center;">操作</td>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("CityName")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.AreaType), Convert.ToInt32(Eval("AreaType")))%>
                                <%--<%#Eval("AreaType").ToString()=="1"?"服务范围":"停车网点"%>--%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("ThisName")%>
                            </td>
                            <td style="width:100px;max-width:100px;overflow:hidden;text-align: center; text-overflow:ellipsis;white-space:nowrap;" title="<%#Eval("Coordinates")%>">
                                <%#Eval("Coordinates")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Address")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Status").ToString()=="1"?"启用":"禁用"%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("CreateTime")%>
                            </td>
                            <td style="text-align: center;">
                                <a href="javascript:editPolyline('<%#Eval("ID")%>','<%#Eval("CityCode")%>','<%#Eval("AreaType")%>');" style="text-decoration: underline;color: Blue;">设置范围</a>
                                <%--<%#Eval("AreaType").ToString()=="2"?"<a href=\"javascript:selectPoint('"+Eval("ID")+"','"+Eval("CityID")+"');\" style=\"text-decoration: underline;color: Blue;\">选取停车点</a>":""%>--%>
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
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
