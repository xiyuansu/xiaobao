<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleAlarm_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleAlarm_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>车辆报警</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/ImageUpload.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Themes/Scripts/ckeditor/ckeditor.js"></script>
    <script type="text/javascript">
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
    <style type="text/css">
        .auto-style1 {
            width: 1152px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="div-frm">
        <%--基本信息--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>车辆名称:</th>
                <td colspan="3">
                    <label id="VehicleName" runat="server"></label>
                </td>
            </tr>
            <tr>
                <th>报警类别:</th>
                <td>
                    <label id="AlarmType" runat="server"></label>
                </td>
                <th>报警时间:</th>
                <td>
                    <label id="AlarmTime" runat="server"></label>
                </td>
            </tr>
            <tr>
                <th>处理状态:</th>
                <td>
                    <label id="AlarmStatus" runat="server"></label>
                </td>
                <th>处理时间:</th>
                <td>
                    <label id="OperateTime" runat="server"></label>
                </td>
            </tr>
            <tr>
                <th>操作人员:</th>
                <td colspan="3">
                    <label id="OperatorID" runat="server"></label>
                </td>
            </tr>
            <tr>
                <th>处理说明:</th>
                <td colspan="3">
                    <textarea id="OperateRemark" runat="server" type="text" class="txt" datacol="yes" err="处理说明" style="width:500px;height:150px;"></textarea>
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