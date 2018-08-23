<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User_UpdateBalance.aspx.cs"
    Inherits="YR.Web.Manage.UserManage.User_UpdateBalance" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>更新用户余额</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/TreeTable/jquery.treeTable.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/TreeTable/css/jquery.treeTable.css" rel="stylesheet"
        type="text/css" />
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化
        $(function () {
            
        })
        
        function CalculateBalance() {
            var startBalance = $("#Balance").text();                // 起始余额
            var oprType = $("#OprType").val();                      // 操作类型（1=增加，2=减少）
            var AdjustmentMoney = $("#AdjustmentMoney").val();      // 调整金额
            if (startBalance == '') {
                startBalance = '0.00';
            }
            if (AdjustmentMoney == '') {
                AdjustmentMoney = '0.00';
            }
            var endBalance = 0.00;

            if (oprType == 3) {
                endBalance = parseFloat(startBalance) + parseFloat(AdjustmentMoney);
            }
            else {
                endBalance = parseFloat(startBalance) - parseFloat(AdjustmentMoney);
                if (endBalance < 0) {
                    alert('调整金额太大，导致余额小于0！');
                    $("#AdjustmentMoney").val("0.00");
                    endBalance = startBalance;
                }
            }
            $("#EndBalance").html(endBalance);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="div-frm" style="height: 275px;">
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    昵称:
                </th>
                <td colspan="2">
                    <label id="NickName" runat="server">
                        liuya</label>
                </td>
            </tr>
            <tr>
                <th>
                    真实姓名:
                </th>
                <td colspan="2">
                    <label id="RealName" runat="server">
                        刘亚</label>
                </td>
            </tr>
            <tr>
                <th>
                    主帐户余额:
                </th>
                <td>
                    <label id="Balance" runat="server">0.00</label>元
                </td>
                <td>
                    <select id="OprType" runat="server" onchange="CalculateBalance();" style="margin: 0px 10px">
                        <option value="3">增加</option>
                        <option value="4">减少</option>
                    </select>
                    <input id="AdjustmentMoney" type="text" runat="server" onblur="CalculateBalance();"
                        style="width: 50px" />
                    <span style="padding: 0px 5px; font-size: larger; font-weight: bold">=</span>
                    <label id="EndBalance" runat="server">
                        0.00</label>元 (<font color="blue">调整后余额</font>)
                </td>
            </tr>
            <tr>
                <th>备注：</th>
                <td colspan="2">
                    <textarea id="txtRemark" class="txtRemark" runat="server" style="width: 552px;
                        height: 100px;"></textarea>
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
