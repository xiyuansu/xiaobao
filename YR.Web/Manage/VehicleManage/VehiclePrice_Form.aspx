<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehiclePrice_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehiclePrice_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>车型价格表单</title>
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
                <th>城市:</th>
                <td>
                    <select id="CityID" runat="server">
                    </select>
                </td>
                <th>
                    车型:
                </th>
                <td>
                    <select id="ModelID" runat="server">
                    </select>
                </td>
            </tr>
            <tr>
                <th style="vertical-align:top;">
                    分钟价格:
                </th>
                <td>
                    <input id="MinutePrice" runat="server" type="text" class="txt" datacol="yes" checkexpession="Double" err="分钟价格" style="width: 150px" />
                    单位:元
                </td>
                <th style="vertical-align:top;">
                    公里价格:
                </th>
                <td>
                    <input id="KmPrice" runat="server" type="text" class="txt" datacol="yes" checkexpession="Double" err="公里价格" style="width: 150px" />
                    单位:元
                </td>
            </tr>
            <tr>
                <th style="vertical-align:top;">
                    起步金额:
                </th>
                <td>
                    <input id="MinPrice" runat="server" type="text" class="txt" datacol="yes" checkexpession="Num" err="起步金额" style="width: 150px" />
                    单位:元
                </td>
                <th style="vertical-align:top;">
                    封顶金额:
                </th>
                <td>
                    <input id="MaxPrice" runat="server" type="text" class="txt" datacol="yes" checkexpession="Num" err="封顶金额" style="width: 150px" />
                    单位:元
                </td>
            </tr>
           </table>
        </div>
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click" OnClientClick="return CheckValid();"><span class="l-btn-left"> <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
    </div>
    </form>
</body>
</html>