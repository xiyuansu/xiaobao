<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Charge_Form.aspx.cs" Inherits="YR.Web.Manage.UserManage.Charge_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>充值规则设置</title>
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
    <div class="frmtop"></div>
    <div class="div-frm" style="height: 275px;">
        <input type="hidden" runat="server" id="SettingType" name="SettingType" value="1"/>
        <input type="hidden" runat="server" id="SettingKey" name="SettingKey"/>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>名称:</th>
                <td colspan="3">
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="名称" checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>                
                <th>充值金额:</th>
                <td>
                    <input id="ChargeMoney" runat="server" type="text" class="txt" datacol="yes" err="赠送金额" checkexpession="Double" style="width: 200px" />
                </td>
                <th>赠送金额:</th>
                <td>
                    <input id="PresentMoney" runat="server" type="text" class="txt" datacol="yes" err="赠送金额" checkexpession="Double" style="width: 200px" />
                </td>
            </tr>
            <tr>                
                <th>有效开始日期:</th>
                <td>
                    <input id="BeginTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" style="width: 200px" err="有效开始日期" autocomplete="off" />
                </td>
                <th>有效终止日期:</th>
                <td>
                    <input id="EndTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })" style="width: 200px" err="有效终止日期" tabindex="1" autocomplete="off" />
                </td>
            </tr>
            <tr>
                <th>次序:</th>
                <td colspan="3">
                    <input id="Sort" runat="server" type="text" class="txt" datacol="yes" err="次序" checkexpession="Num" style="width: 200px" />
                </td>
            </tr>
        </table>
    </div>
    
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
    </div>
    </form>
</body>
</html>
