<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sms_Form.aspx.cs" Inherits="YR.Web.Manage.InfomationManage.Sms_Form" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>短信信息表单</title>
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
</head>
<body>
    <form id="form1" runat="server">
    
    <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    停车场名称:
                </th>
                <td>
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="停车场名称"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    停车场地址:
                </th>
                <td>
                    <input id="Address" runat="server" type="text" class="txt" datacol="yes" err="停车场地址"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    固定电话:
                </th>
                <td>
                    <input id="Telephone" runat="server" type="text" class="txt" datacol="yes" err="固定电话"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    移动电话:
                </th>
                <td>
                    <input id="Mobile" runat="server" type="text" class="txt" datacol="yes" err="移动电话"
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
                    停车场描述:
                </th>
                <td colspan="3">
                    <textarea id="Description" class="txtRemark" runat="server" style="width: 552px;
                        height: 100px;"></textarea>
                </td>
            </tr>
        </table>
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
    </div>
    </form>
</body>
</html>
