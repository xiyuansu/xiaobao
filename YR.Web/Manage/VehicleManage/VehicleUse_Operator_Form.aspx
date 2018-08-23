<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleUse_Operator_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleUse_Operator_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>运维用车信息</title>
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
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input runat="server" type="hidden" id="UserID" />
        <div class="div-frm" style="height: 262px;">
            <%--用车信息--%>
            <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <th>客户人员:
                    </th>
                    <td>
                        <input id="RealName" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th>联系电话:
                    </th>
                    <td>
                        <input id="BindPhone" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>开车时间:
                    </th>
                    <td>
                        <input id="CreateTime" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th></th>
                    <td></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
