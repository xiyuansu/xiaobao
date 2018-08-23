<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDepositReturn_Form.aspx.cs" Inherits="YR.Web.Manage.UserManage.UserDepositReturn_Form" %>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>押金退款详情</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery.uploadify/jquery-1.3.2.min.js"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script>

        $(document).ready(function () {
            
        });

        //获取表单值
        function CheckValid() {
            var returnValue = true;
            return returnValue;
        }

    </script>
    <style type="text/css">
        .transfer-btn{display:none;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; margin-left: 30px; margin-right: 30px">
            <table style="margin-top: 20px; width: 98%; border: 1px solid black; line-height: 50px;">
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">用户姓名:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Label ID="RealName" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">手机号:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Label ID="BindPhone" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">押金金额:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Label ID="DepositMoney" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">押金支付方式:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Label ID="DepositPayWay" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">押金支付交易号:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Label ID="DepositTradeNo" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">申请状态:</td>
                    <td style="width: 80%; text-align: left;">
                        <select id="State" class="Searchwhere1" runat="server">
                            <%--<option value="1">处理中</option>--%>
                            <option value="2">退款成功</option>
                            <option value="3">退款失败</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">操作备注:</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="Remark" runat="server" Height="69px" TextMode="MultiLine" Width="70%"></asp:TextBox>
                        <asp:LinkButton ID="Transfer" runat="server" class="transfer-btn" OnClientClick="return CheckValid();" OnClick="Transfer_Click">
                            已通过转账退款
                        </asp:LinkButton>
                    </td>
                </tr>
            </table>
            <div class="frmbottom">
                <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClientClick="return CheckValid();" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
                <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
                    <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
            </div>
        </div>
    </form>
</body>
</html>

