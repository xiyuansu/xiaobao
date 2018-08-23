<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChargingPoints_Form.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.ChargingPoints_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>充电桩信息表单</title>
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
        //初始化
        $(function () {
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
    <div class="div-frm" style="height:422px;">
        <%--基本信息 ID	Name	IPAddr	IMEI	Longitude	Latitude	Address	State	UseState	StationID	DeleteMark	CreateTime	Phone	RegSocketIP	RegSocketPort	BizSocketIP	BizSocketPort	LastUpdateTime--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    充电桩名称:
                </th>
                <td>
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="充电桩名称"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    地址:
                </th>
                <td>
                    <input id="Address" runat="server" type="text" class="txt" datacol="yes" err="地址"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    经度:
                </th>
                <td>
                    <input id="Longitude" runat="server" type="text" class="txt" datacol="yes" err="经度"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    纬度:
                </th>
                <td>
                    <input id="Latitude" runat="server" type="text" class="txt" datacol="yes" err="纬度"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    IMEI:
                </th>
                <td>
                    <input id="IMEI" runat="server" type="text" class="txt" datacol="yes" err="IMEI" style="width: 200px" />
                </td>
                <th>
                    手机号:
                </th>
                <td>
                    <input id="Phone" runat="server" type="text" class="txt" datacol="yes" err="手机号"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    使用状态:
                </th>
                <td>
                    <select id="UseState" runat="server" style="width: 200px">
                        <option value="1">空闲</option>
                        <option value="2">预约</option>
                        <option value="3">使用中</option>
                    </select>
                </td>
                <th>
                    充电桩状态:
                </th>
                <td>
                    <select id="State" runat="server" style="width: 200px">
                        <option value="1">可用</option>
                        <option value="2">不可用</option>
                        <option value="3">维修</option>
                        <option value="4">丢失</option>
                        <option value="5">其它</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>
                    所属充电站：
                </th>
                <td>
                    <select id="selStation" runat="server" style="width: 200px"></select>
                </td>
                <th>IP地址：</th>
                <td>
                    <input id="IPAddr" runat="server" type="text" class="txt" datacol="yes" err="IP地址" style="width: 200px" />
                </td>
            </tr>
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
