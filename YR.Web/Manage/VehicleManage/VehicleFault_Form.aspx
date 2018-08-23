<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleFault_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleFault_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>故障信息编辑</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <style type="text/css">
        .auto-style1 {
            height: 25px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th class="auto-style1">
                    故障车辆:
                </th>
                <td class="auto-style1">
                    <input type="text" disabled="disabled" id="VehicleName" runat="server" />
                </td> 
            </tr>
            <tr>
                <th>
                    会员名称:
                </th>
                <td>
                    <input type="text" disabled="disabled" id="RealName" runat="server" />
                </td>
            </tr>
            <tr>
                    <th>
                        触发类型
                    </th>
                    <td>
                        <select id="selTriggerType" runat="server">
                        <option value="1">用户自报</option>
                        <option value="2">系统</option>
                        </select>
                    </td>
            </tr>
            <tr>
                <th>
                    故障类型:
                </th>
                <td>
                    <input type="text" id="FaultType" runat="server" />
                </td>
            </tr>
            <tr>
                <th>
                    故障描述:
                </th>
                <td>
                    <textarea id="Remark" class="txtRemark" runat="server" style="width: 552px;
                        height: 100px;"></textarea>
                </td>
            </tr>
            <tr>
                <th>
                    上报图片:
                </th>
                <td>
                    <asp:Literal ID="img_list" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />处理完成</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
            <input type="hidden" id="VehicleID" runat="server" />
            <input type="hidden" id="UserID" runat="server" />
    </div>
    </form>
</body>
</html>
