<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.User_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>用户管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();
        })

        //新增
        function add() {
            var url = "/Manage/UserManage/User_Form.aspx";
            top.openDialog(url, 'User_Form', '用户信息 - 添加', 700, 350, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/User_Form.aspx?key=" + key;
                top.openDialog(url, 'User_Form', '用户信息 - 编辑', 700, 350, 50, 50);
            }
        }
        // 删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        // 审核
        function audit() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var url = "/Manage/UserManage/User_Audit.aspx?key=" + key;
                top.openDialog(url, 'User_Audit', '用户实名认证 - 审核', 700, 350, 50, 50);
            }
        }
        // 挂失
        function ReportLoss() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/User_ReportLoss.aspx?key=" + key;
                top.openDialog(url, 'User_ReportLoss', '用户信息 - 挂失', 700, 350, 50, 50);
            }
        }
        // 更新主帐户余额
        function UpdateBalance() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/User_UpdateBalance.aspx?key=" + key;
                top.openDialog(url, 'User_UpdateBalance', '用户信息 - 更新余额', 700, 350, 50, 50);
            }
        }

        // 更新副帐户余额
        function UpdateBalance2() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/User_UpdateBalance2.aspx?key=" + key;
                top.openDialog(url, 'User_UpdateBalance', '用户信息 - 更新余额', 700, 350, 50, 50);
            }
        }

        //异常还车
        function Abnormal() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var parm = 'action=abnormal&user_ID=' + key;
                showConfirmMsg('注：您确认要将当前用户订单【异常处理】吗？', function (r) {
                    if (r) {
                        getAjax('UserInfoValidate.ashx', parm, function (rs) {
                            if (parseInt(rs) > 0) {
                                showTipsMsg("处理成功！", 2000, 4);
                            }
                            else {
                                showTipsMsg("<span style='color:red'>没有发现预约订单！</span>", 4000, 5);
                            }
                        });
                    }
                });
            }
        }
        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
        }

        function Export() {
            __doPostBack('lbtExport', '');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="lbtExport" runat="server" class="button green" OnClick="lbtExport_Click">导出</asp:LinkButton>
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <div class="btnbartitle">
                    <div>
                        用户管理
                    </div>
                </div>
                <div class="btnbarcontetn">

                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>关键字</th>
                                <td>
                                    <input type="text" id="txt_Search" runat="server" placeholder="登录名/邮箱/手机号码" tabindex="1" style="outline: none;" />
                                </td>
                                <th>实名认证</th>
                                <td>
                                    <select id="selRealNameCertification" class="Searchwhere1" runat="server">
                                        <option value="-1">全部</option>
                                        <option value="1">未认证</option>
                                        <option value="2">提交申请</option>
                                        <option value="3">审核失败</option>
                                        <option value="4">已认证</option>
                                    </select></td>
                                <th>用户类型</th>
                                <td>
                                    <select id="selUserType" class="Searchwhere1" runat="server">
                                    </select></td>
                                <th>账户余额</th>
                                <td>
                                    <input type="text" id="start_money" runat="server" style="width: 100px" placeholder="开始金额" tabindex="1" autocomplete="off" />-<input type="text" id="end_money" runat="server" style="width: 100px" placeholder="结束金额" tabindex="1" autocomplete="off" />
                                </td>
                            </tr>
                            <tr style="text-align: center">
                                <td colspan="8">
                                    <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton><asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click"><span class="icon-botton">
        </span>重 置</asp:LinkButton></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="div-body">
                    <table id="table1" class="grid" singleselect="true">
                        <thead>
                            <tr>
                                <td style="width: 20px; text-align: center;">选择
                                </td>
                                <td style="width: 100px; text-align: center;">绑定手机
                                </td>
                                <td style="width: 60px; text-align: center;">真实姓名
                                </td>
                                <td style="width: 60px; text-align: center;">昵称
                                </td>
                                <td style="width: 80px; text-align: center;">是否实名认证
                                </td>
                                <td style="width: 80px; text-align: center;">账户余额
                                </td>
                                <td style="width: 80px; text-align: center;">副账户余额
                                </td>
                                <td style="width: 80px; text-align: center;">押金金额
                                </td>
                                <td style="width: 80px; text-align: center;">用户类型
                                </td>
                                <td style="width: 50px; text-align: center;">状态
                                </td>
                                <td style="width: 100px; text-align: center;">创建时间
                                </td>
                            </tr>
                        </thead>
                        <tbody>

                            <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 20px; text-align: center;">
                                            <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                        </td>
                                        <td style="width: 100px; text-align: center;">
                                            <%#Eval("BindPhone")%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Eval("RealName")%>
                                        </td>
                                        <td style="width: 60px; text-align: center;">
                                            <%#Eval("NickName")%>
                                        </td>
                                        <td style="width: 50px; text-align: center;">
                                            <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserRealNameCertification), Convert.ToInt32(Eval("RealNameCertification")))%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Eval("Balance")%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Eval("Balance2")%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Eval("Deposit")%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserType), Convert.ToInt32(Eval("UserType")))%>
                                        </td>
                                        <td style="width: 80px; text-align: center;">
                                            <%#Eval("UserState").ToString()=="0"?"禁用":"启用"%>
                                        </td>
                                        <td style="width: 50px; text-align: center;">
                                            <%#Eval("RegistrionTime")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <% if (rp_Item != null)
                                       {
                                           if (rp_Item.Items.Count == 0)
                                           {
                                               Response.Write("<tr><td colspan='10' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                                           }
                                       } %>
                                </FooterTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <br />
                    <uc1:PageControl ID="PageControl1" runat="server" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
                <asp:PostBackTrigger ControlID="PageControl1" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
