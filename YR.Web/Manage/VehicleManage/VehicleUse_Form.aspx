<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleUse_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleUse_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>马厩信息表单</title>
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
            var state = $("#OrderState").val();
            if (state ==6)
            {
                $("#trOrder,#OrderState").css({ "color": "red" });
            }
        })
        //异常还车
        function Abnormal(type) {
            var key = $("#OrderNum").val();
            if (key != null) {
                var parm = 'action=abnormal&ordernum=' + key + "&type="+type;
                var r = confirm("您确认要将当前用户订单【异常处理】吗？");
                if (r == true) {
                    getAjax('../UserManage/UserInfoValidate.ashx', parm, function (rs) {
                        if (parseInt(rs) > 0) {
                            top.main.load();
                            OpenClose();
                        } else {
                            showTipsMsg("<span style='color:red'>异常还车失败！</span>", 4000, 5);
                        }
                    });
                }
            }
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input runat="server" type="hidden" id="UserID" />
        <input runat="server" type="hidden" id="hdBillingState" />
        
        <div class="div-frm">
            <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <th>占用人:</th>
                    <td>
                        <input id="RealName" runat="server" type="text" class="txt" datacol="yes"  style="width: 200px" disabled="disabled"/>
                    </td>
                    <th>占用人手机:</th>
                    <td>
                        <input id="BindPhone" runat="server" type="text" class="txt" datacol="yes"  style="width: 200px" disabled="disabled"/>
                    </td>
                </tr>
                <tr>
                    <th>车辆名称:</th>
                    <td>
                        <input id="Name" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                    <th>使用状态:</th>
                    <td>
                        <select id="UseState" runat="server" style="width:204px">
                            <option value="1">空闲</option>
                            <option value="2">使用中</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>车辆状态:</th>
                    <td>
                        <select id="VehicleState" runat="server" style="width:204px">
                            <option value="1">可用</option>
	                        <option value="2">不可用</option>
                            <option value="3">维修</option>
                            <option value="4">丢失</option>
                            <option value="5">其它</option>
                        </select>
                    </td>
                    <th>开车状态:</th>
                    <td>
                        <select id="LockState" runat="server" style="width:204px">
                            <option value="unlock">开锁</option>
	                        <option value="lock">锁定</option>
                        </select>
                    </td>
                </tr>
                <tr id="trOrder">
                    <th>订单状态:</th>
                    <td>
                        <select id="OrderState" runat="server" style="width:204px">
                            <option value="0">已取消</option>
                            <option value="1">进行中</option>
                            <option value="5">未支付</option>
                            <option value="2">已完成</option>
                            <option value="3">异常订单</option>
                        </select>
                    </td>
                    <th>订单编号:</th>
                    <td>
                        <input id="OrderNum" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                </tr>
                <tr>
                    <th>订单创建时间:</th>
                    <td>
                        <input id="CreateTime" runat="server" type="text" class="txt" datacol="yes"  style="width: 200px" disabled="disabled"/>
                    </td>
                    <th>开车时间:</th>
                    <td>
                        <input id="StartTime" runat="server" type="text" class="txt" datacol="yes"  style="width: 200px" disabled="disabled"/>
                    </td>
                </tr>
                <tr>
                    <th>公里单价:</th>
                    <td>
                        <input id="KMPrice" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                    <th>分钟单价:</th>
                    <td>
                        <input id="MinutePrice" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                </tr>
                <tr>
                    <th>行驶里程:</th>
                    <td>
                        <input id="Mileage" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                    <th>用车时长:</th>
                    <td>
                        <input id="Minutes" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" disabled="disabled"/>
                    </td>
                </tr>
                
            </table>
        </div>
        <div class="frmbottom">
            <a title="异常还车" onclick="Abnormal(1);" class="button green"><span class="icon-botton" style="background: url('/Themes/images/16/2012080412301.png') no-repeat scroll 0px 4px;"></span>异常还车(收费)</a>
            <a title="异常还车" onclick="Abnormal(2);" class="button green"><span class="icon-botton" style="background: url('/Themes/images/16/2012080412301.png') no-repeat scroll 0px 4px;"></span>异常还车(免费)</a>
            <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left"><img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
        </div>
    </form>
</body>
</html>