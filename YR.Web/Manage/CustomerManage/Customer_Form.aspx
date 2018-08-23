<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Customer_Form.aspx.cs" Inherits="YR.Web.Manage.CustomerManage.Customer_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>客户信息表单</title>
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
                <th>名称:</th>
                <td>
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="名称" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>地址:</th>
                <td>
                    <input id="Address" runat="server" type="text" class="txt" datacol="yes" err="地址" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>电话:</th>
                <td>
                    <input id="Tel" runat="server" type="text" class="txt" datacol="yes" err="电话"  style="width: 200px" />
                </td>
                <th>联系人:</th>
                <td>
                    <input id="Contacts" runat="server" type="text" class="txt" datacol="yes" err="联系人" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th style="display:none;">分成比例:</th>
                <td style="display:none;">
                    <input id="CommissionRate" runat="server" type="text" class="txt" datacol="yes" err="分成比例" style="width: 100px" />（如0.1,代表10%）
                </td>
                <th>类型:</th>
                <td>
                    <select id="Category" runat="server" style="width:204px">
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
                <th></th>
                <td>
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