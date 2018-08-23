<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User_Form.aspx.cs" Inherits="YR.Web.Manage.UserManage.User_Form" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户信息表单</title>
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
            $('#table2').hide();
            $('#table3').hide();
        })
        function panel(obj) {
            if (obj == 1) {
                $('#table1').show();
                $('#table2').hide();
                $('#table3').hide();
            } else if (obj == 2) {
                $('#table1').hide();
                $("#table2").show();
                $('#table3').hide();
            } else if (obj == 3) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').show();
            }
        }
        //附加信息表单赋值
        function Setform() {
            var pk_id = GetQuery('key');
            if (IsNullOrEmpty(pk_id)) {
                var strArray = new Array();
                var strArray1 = new Array();
                var item_value = $("#AppendProperty_value").val(); //后台返回值
                strArray = item_value.split(';');
                for (var i = 0; i < strArray.length; i++) {
                    var item_value1 = strArray[i];
                    strArray1 = item_value1.split('|');
                    $("#" + strArray1[0]).val(strArray1[1]);
                }
            }
        }
        //获取表单值
        function CheckValid() {
            if (!CheckDataValid('#form1')) {
                return false;
            }
            var item_value = '';
            $("#AppendProperty_value").empty;
            $("#table2 tr").each(function (r) {
                $(this).find('td').each(function (i) {
                    var pk_id = $(this).find('input,select').attr('id');
                    if ($(this).find('input,select').val() != "" && $(this).find('input,select').val() != "==请选择==" && $(this).find('input,select').val() != undefined) {
                        item_value += pk_id + "|" + $(this).find('input,select').val() + ";";
                    }
                });
            });
            $("#AppendProperty_value").val(item_value);
            $("#checkbox_value").val(CheckboxValue())
            if (!confirm('您确认要保存此操作吗？')) {
                return false;
            }
        }
        //验证所属部门必填
        var ChekOrgVale = "";
        function ChekOrgClick() {
            var pk_id = GetQuery('key');
            if (IsNullOrEmpty(pk_id)) {
                ChekOrgVale = 1;
            }
            $("#table3 [type = checkbox]").click(function () {
                ChekOrgVale = "";
                if ($(this).val() != "") {
                    if ($(this).attr("checked") == "checked") {
                        ChekOrgVale = 1;
                    };
                }
            })
        }
        function Step1() {
            $("#Step2Container").hide();
        }
        function Step2() {
            $("#CurruntPicContainer").hide();
        }
        function Step3() {
            $("#Step2Container").hide();
        }
        function dis() {
            document.getElementById("btnUpload").click();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <%-- 所有打勾复选框值--%>
    <input id="checkbox_value" type="hidden" runat="server" />
    <%--获取附加信息值--%>
    <input id="AppendProperty_value" type="hidden" runat="server" />
    <div class="frmtop">
        <table style="padding: 0px; margin: 0px; height: 100%;" cellpadding="0" cellspacing="0">
            <tr>
                <td id="menutab" style="vertical-align: bottom;">
                    <div id="tab0" class="Tabsel" onclick="GetTabClick(this);panel(1)">
                        基本信息</div>
                    <div id="tab1" class="Tabremovesel" onclick="GetTabClick(this);panel(2);">
                        个人头像</div>
                    <div id="tab2" class="Tabremovesel" onclick="GetTabClick(this);panel(3);">
                        身份证照片</div>
                </td>
            </tr>
        </table>
    </div>
    <div class="div-frm" style="height: 275px;">
        <%--基本信息--%>
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    会员名:
                </th>
                <td>
                    <input id="UserName" runat="server" type="text" class="txt" datacol="yes" err="会员名"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    会员昵称:
                </th>
                <td>
                    <input id="NickName" runat="server" type="text" class="txt" datacol="yes" err="会员昵称"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    真实姓名:
                </th>
                <td>
                    <input id="RealName" runat="server" type="text" class="txt" datacol="yes" err="真实姓名"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    身份证号:
                </th>
                <td>
                    <input id="IDCardNum" runat="server" type="text" class="txt" datacol="yes" err="身份证号"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    绑定手机:
                </th>
                <td>
                    <input id="BindPhone" runat="server" type="text" class="txt" style="width: 200px" />
                </td>
                <th>
                    实名认证:
                </th>
                <td>
                    <select id="RealNameCertification" runat="server" style="width: 200px" >
                        <option value="1">未认证</option>
                        <option value="2">提交申请</option>
                        <option value="3">审核失败</option>
                        <option value="4">已认证</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th>
                    账户余额:
                </th>
                <td>
                    <input id="Balance" disabled runat="server" type="text" value="0.0" class="txt" style="width: 200px" />
                </td>
                <th>
                    押金金额:
                </th>
                <td>
                    <input id="Deposit" disabled runat="server" type="text" value="0.0" class="txt" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    注册时间:
                </th>
                <td>
                    <input id="RegistrionTime" disabled runat="server" type="text" class="txt" style="width: 200px" />
                </td>
                <th>
                    最后登录时间:
                </th>
                <td>
                    <input id="LastloginTime" disabled runat="server" type="text" class="txt" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    状态:
                </th>
                <td>
                    <select id="UserState" class="select" runat="server" style="width: 206px">
                        <option value="1">启用</option>
                        <option value="0">禁用</option>
                    </select>
                </td>
                <th>
                    用户类型:
                </th>
                <td>
                    <select id="UserType" class="select" runat="server" style="width: 206px">
                    </select>
                </td>
            </tr>
        </table>
        <%--个人头像--%>
        <table id="table2" border="0" cellpadding="0" cellspacing="0" class="frm">
        <tr>
            <td>
                <asp:Image ID="imgHeadPortrait" runat="server" />
            </td>
        </tr>
        </table>
        <%--身份证照片--%>
        <div id="table3">
            <div class="btnbartitle">
                <div>
                    身份证照片
                </div>
            </div>
            <div class="div-body" style="height: 245px;">
                <asp:Image ID="imgIDCard" runat="server" />
            </div>
        </div>
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
