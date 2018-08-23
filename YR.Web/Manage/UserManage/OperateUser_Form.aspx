<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperateUser_Form.aspx.cs" Inherits="YR.Web.Manage.UserManage.OperateUser_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>运维人员表单</title>
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
</head>
<body>
    <form id="form1" runat="server">
    <div class="div-frm">
        <%--基本信息--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    身份证号:
                </th>
                <td>
                    <input id="IDCardNum" runat="server" type="text" class="txt" datacol="yes" err="身份证号" style="width: 200px" />
                </td>
                <th>
                    用户姓名:
                </th>
                <td>
                    <input id="UserName" runat="server" type="text" class="txt" datacol="yes" err="用户姓名" checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    用户昵称:
                </th>
                <td>
                    <input id="NickName" runat="server" type="text" class="txt" datacol="yes" err="用户昵称" style="width: 200px" />
                </td>
                <th>
                    性别:
                </th>
                <td>
                    <select id="UserSex" runat="server" style="width:204px">
                        <option value="1">男</option>
	                    <option value="2">女</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>
                    联系电话:
                </th>
                <td>
                    <input id="Tel" runat="server" type="text" class="txt" datacol="yes" err="联系电话" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    电子邮箱:
                </th>
                <td>
                    <input id="Email" runat="server" type="text" class="txt" datacol="yes" err="电子邮箱" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    接收报警短信:
                </th>
                <td>
                    <select id="ReceiveSMS" runat="server" style="width:204px">
                        <option value="0">否</option>
	                    <option value="1">是</option>
                    </select>
                </td>
                <th></th>
                <td></td>
            </tr>
            <tr>
                <th style="vertical-align:top;">
                   网点列表:
                </th>
                <td colspan="3">
                    <asp:CheckBoxList ID="ParkingList" runat="server" Width="100%" RepeatColumns="2">
                    </asp:CheckBoxList>
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