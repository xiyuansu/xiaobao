<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceArea_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.ServiceArea_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>服务范围表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>  
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/ImageUpload.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化
        $(function () {

        })
        //获取表单值
        function CheckValid() {
            //showTipsMsg('请选择城市', '5000', '5');
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
    <div class="div-frm" >
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>名称:</th>
                <td>
                    <input id="ThisName" runat="server" type="text" class="txt" datacol="yes" err="名称" checkexpession="NotNull" style="width: 200px" />
                </td>
                <th></th>
                <td></td>
            </tr>
            <tr>
                <th>城市</th>
                <td><select id="CityID" runat="server"></select></td>
                <th>类型</th>
                <td>
                    <select id="AreaType" runat="server">
                        <option value="2">停车网点</option>
		                <option value="1">服务范围</option>
                        <option value="3">禁停区域</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>经度坐标:</th>
                <td><input id="Longitude" runat="server" type="text" class="txt" datacol="yes" err="经度" style="width: 200px" readonly="readonly"/></td>
                <th>纬度坐标:</th>
                <td><input id="Latitude" runat="server" type="text" class="txt" datacol="yes" err="纬度" style="width: 200px" readonly="readonly"/></td>
            </tr>
            <tr>
                <th>地址:</th>
                <td><input id="Address" runat="server" type="text" class="txt" datacol="yes" err="地址" style="width: 200px" /></td>
                <th>状态:</th>
                <td>
                    <select id="Status" runat="server">
		                <option value="0">禁用</option>
                        <option value="1">启用</option>
                    </select>
                </td>
            </tr>
        </table>
    </div>
    <div class="frmbottom">
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click"  OnClientClick="return CheckValid();">
            <span class="l-btn-left"><img src="/Themes/Images/disk.png" alt="" />保 存</span>
        </asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();">
            <span class="l-btn-left"><img src="/Themes/Images/cancel.png" alt="" />关 闭</span>
        </a>
    </div>
    </form>
</body>
</html>
