<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehiclePrice_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehiclePrice_List" %>
<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>车型价格管理</title>
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
            var url = "/Manage/VehicleManage/VehiclePrice_Form.aspx";
            top.openDialog(url, 'VehicleParking_Form', '价格信息 - 添加', 710, 500, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/VehiclePrice_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleParking_Form', '价格信息 - 编辑', 710, 500, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=价格信息&tableName=YR_VehiclePriceRule&pkName=ID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
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
                    <div>车型价格管理</div>
                </div>
                <div class="btnbarcontetn">
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>城市</th>
                                <td>
                                    <select id="selCity" runat="server" style="width:94px">
                                    </select>
                                </td>
                                <th>车型</th>
                                <td>
                                    <select id="selModel" runat="server" style="width:94px">
                                    </select>
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
                                <td style="width: 80px; text-align: center;">车型名称</td>
                                <td style="width: 80px; text-align: center;">城市</td>
                                <td style="width: 80px; text-align: center;">分钟价格</td>
                                <td style="width: 80px; text-align: center;">公里价格</td>
                                <td style="width: 80px; text-align: center;">起步金额</td>
                                <td style="width: 80px; text-align: center;">封顶金额</td>
                                <td style="width: 80px;text-align: center;">创建时间</td>
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
                                            <%#Eval("ModelName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CityName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("MinutePrice")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("KmPrice")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("MinPrice")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("MaxPrice")%>
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
