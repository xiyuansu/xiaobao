<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehiclePitLog_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehiclePitLog_Form" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>维修记录表单</title>
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
                    维修站:
                </th>
                <td>
                    <select id="PitID" runat="server">
                        <option value="1">维修站1</option>
                    </select>
                </td>
                <th>
                    维修车辆:
                </th>
                <td>
                    <select id="VehicleID" runat="server">
                        <option value="12223">爱玛电动车</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>
                    维修项目:
                </th>
                <td>
                    <input id="MaintainItems" runat="server" type="text" class="txt" datacol="yes" err="维修项目"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    维修人:
                </th>
                <td>
                    <input id="MaintainPeople" runat="server" type="text" class="txt" datacol="yes" err="维修人"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    维修人电话:
                </th>
                <td>
                    <input id="LinkPhone" runat="server" type="text" class="txt" datacol="yes" err="维修人电话"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    维修时间:
                </th>
                <td>
                    <input id="MaintainTime" runat="server" type="text" class="txt" datacol="yes" err="维修时间"
                        checkexpession="NotNull" style="width: 200px" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd HH:mm:ss' })" />
                </td>
            </tr>
            <tr>
                <th>
                    备注:
                </th>
                <td colspan="3">
                    <textarea id="Remark" class="txtRemark" runat="server" style="width: 552px;
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
