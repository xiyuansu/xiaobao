<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Platform_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Platform_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>平台信息表单</title>
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
    <div class="div-frm" style="height:162px;">
        <%--基本信息--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    平台名称:
                </th>
                <td>
                    <input id="Platform" runat="server" type="text" class="txt" datacol="yes" err="平台名称" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    平台协议代码:
                </th>
                <td>
                    <input id="Code" runat="server" type="text" class="txt" datacol="yes" err="平台协议代码" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    平台说明:
                </th>
                <td colspan="3">
                    <input id="Memo" runat="server" type="text" class="txt" datacol="yes" err="平台说明" style="width:400px" />
                </td>
            </tr>
            <tr>
                <th>
                    排序顺序:
                </th>
                <td colspan="3">
                    <input ID="Sort" runat="server" type="text" class="txt" datacol="yes" style="width:100px" value="9999"/>
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
