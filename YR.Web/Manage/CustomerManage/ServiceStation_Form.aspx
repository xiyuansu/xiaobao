<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceStation_Form.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.ServiceStation_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>充电站信息表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $('#HotelID').change(function () {
                $("#Name").val($(this).children('option:selected').text());
            })
        })
        //获取表单值
        function CheckValid() {
            if (!CheckDataValid('#form1')) {
                return false;
            }
            if (!confirm('您确认要保存此操作吗？')) {
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="div-frm" style="height:362px;">
        <%--基本信息--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    所属客户:
                </th>
                <td>
                    <select id="CustomerID" runat="server" style="width: 204px"></select>
                </td>
                <th>
                    充电站地址:
                </th>
                <td>
                    <input id="Address" runat="server" type="text" class="txt" datacol="yes" err="充电站地址" checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    充电站名称:
                </th>
                <td>
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="充电站名称" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    状态:
                </th>
                <td>
                    <select id="State" runat="server" style="width:204px">
                        <option value="1">有效</option>
	                    <option value="0">无效</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>
                    联系人:
                </th>
                <td>
                    <input id="Contacts" runat="server" type="text" class="txt" datacol="yes" err="联系人" style="width: 200px" />
                </td>
                <th>
                    联系人电话:
                </th>
                <td>
                    <input id="ContactTel" runat="server" type="text" class="txt" datacol="yes" err="联系人电话" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    负责人:
                </th>
                <td>
                    <input id="Managers" runat="server" type="text" class="txt" datacol="yes" err="负责人" style="width: 200px" />
                </td>
                <th>
                    负责人电话:
                </th>
                <td>
                    <input id="ManagerTel" runat="server" type="text" class="txt" datacol="yes" err="负责人电话" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    经度:
                </th>
                <td>
                    <input id="Longitude" runat="server" type="text" class="txt" datacol="yes" err="经度" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    纬度:
                </th>
                <td>
                    <input id="Latitude" runat="server" type="text" class="txt" datacol="yes" err="纬度" checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    还车半径:
                </th>
                <td>
                    <input id="ParkingRadius" runat="server" type="text" class="txt" datacol="yes" err="还车半径" checkexpession="Double" style="width: 100px" />
                    （单位:千米）
                </td>
                <th>
                    
                </th>
                <td>
                    
                </td>
            </tr>
            <%--<tr>
                <th>
                    车辆总数:
                </th>
                <td>
                    <input id="VehicleTotal" runat="server" type="text" class="txt" datacol="yes" err="车辆总数" style="width: 200px" />
                </td>
                <th>
                    可用车辆数:
                </th>
                <td>
                    <input id="VehicleUsable" runat="server" type="text" class="txt" datacol="yes" err="可用车辆数" style="width: 200px" />
                </td>
            </tr>--%>
                </table>
        </div>
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click" OnClientClick="return CheckValid();"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
    </div>
    </form>
</body>
</html>