<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Recharge_List.aspx.cs" Inherits="YR.Web.Manage.AgentManage.Recharge_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>提现申请管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })

        //处理
        function handle() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var url = "/Manage/AgentManage/Recharge_Handle.aspx?key=" + key;
                top.openDialog(url, 'Pits_Form', '众筹申请信息 - 审核', 700, 350, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2"> 
        <ContentTemplate>
    <div class="btnbartitle">
        <div>
            提现申请管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div style="float: left;">
            <table class="tabCondition">
                <tr>
                    <th>
                        提现会员
                    </th>
                    <td>
                        <input type="text" id="txt_Search" runat="server" placeholder="昵称/真实姓名/手机号码"
                            tabindex="1" autocomplete="off">
                    </td>
                    <th>
                        提现时间
                    </th>
                    <td>
                        <input id="txtStartChangesTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndChangesTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        处理时间
                    </th>
                    <td>
                        <input id="txtStartHandleTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndHandleTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        提现金额
                    </th>
                    <td>
                        <input id="txtStartMoney" runat="server" type="text" class="txt" style="width: 100px" placeholder="开始金额" tabindex="1" autocomplete="off" />-
                        <input id="txtEndMoney" runat="server" type="text" class="txt" style="width: 100px" placeholder="结束金额" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr>
                    <th>
                        状态
                    </th>
                    <td colspan="7">
                        <select id="selFinancialState" class="Searchwhere2" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">新提交</option>
                            <option value="2">已处理</option>
                            <option value="3">无效</option>
                        </select>
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="8">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton><asp:LinkButton ID="lbtInit" runat="server" class="button green"
            OnClick="lbtInit_Click"><span class="icon-botton">
        </span>重 置</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid">
            <thead>
                <tr>
                    <td style="width: 40px; text-align: center;">
                        <label id="checkAllOff" onclick="CheckAllLine()" title="全选">
                            &nbsp;</label>
                    </td>
                    <td style="width: 60px; text-align: center;">
                        姓名
                    </td>
                    <td style="width: 80px; text-align: center;">
                        手机号
                    </td>
                    <td style="width: 60px;text-align: center;">
                        提现金额
                    </td>
                    <td style="width: 100px;text-align: center;">
                        提现时间
                    </td>
                    <td style="width: 80px;text-align: center;">
                        处理时间
                    </td>
                    <td style="width: 100px; text-align: center;">
                        状态
                    </td>
                    <td style="width: 60px; text-align: center;">
                        户名
                    </td>
                    <td style="width: 60px; text-align: center;">
                        卡号
                    </td>
                    <td style="width: 60px; text-align: center;">
                        所属银行
                    </td>
                    <td style="width: 60px; text-align: center;">
                        开户行
                    </td>
                    <td style="width: 100px; text-align: center;">
                        审核备注
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="width: 40px; text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("RealName")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("BindPhone")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("WithdrawalMoney")%>
                            </td>
                            <td style="width: 120px; text-align: center;">
                                <%#Eval("ApplyTime")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Eval("DoTime")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserRaiseWithdrawalApplyState), Convert.ToInt32(Eval("ApplyState")))%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("AccountName")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("Account")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("BankName")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("OpenBank")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Eval("Remark")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='7' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
        </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
