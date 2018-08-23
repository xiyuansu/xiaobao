<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserFinancial_List.aspx.cs"
    Inherits="YR.Web.Manage.UserManage.UserFinancial_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>财务信息查询</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })

        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/UserFinancial_Form.aspx?key=" + key;
                top.openDialog(url, 'UserFinancial_Form', '财务信息 - 编辑', 700, 350, 50, 50);
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
            财务信息查询
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
                        用  户
                    </th>
                    <td>
                        <input type="text" id="txt_Search" runat="server" placeholder="昵称/真实姓名/手机号码"
                            tabindex="1" autocomplete="off">
                    </td>
                    <th>
                        变动类型
                    </th>
                    <td>
                        <select id="selChangesType" class="Searchwhere2" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">充值</option>
                            <option value="2">消费</option>
                            <option value="3">平台增加</option>
                            <option value="4">平台减少</option>
                            <option value="5">押金缴纳</option>
                            <option value="99">其它</option>
                        </select>
                    </td>
                    <th>
                        操作者类型
                    </th>
                    <td><select id="selOperatorType" class="Searchwhere2" runat="server" style=" width:134px">
                            <option value="-1">全部</option>
                            <option value="2">管理员</option>
                            <option value="1">用户</option>
                        </select>
                    </td>
                    <th>
                        变动时间
                    </th>
                    <td>
                        <input id="txtStartChangesTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="开始时间" tabindex="1" autocomplete="off"/>-
                        <input id="txtEndChangesTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="结束时间" tabindex="1" autocomplete="off"/>
                    </td>
                    <th>
                        操作渠道
                    </th>
                    <td>
                        <select id="selOperatorWay" class="Searchwhere2" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">支付宝</option>
                            <option value="2">微信支付</option>
                            <option value="3">平台</option>
                            <option value="99">其他</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>操作者</th>
                    <td><input type="text" id="txtOperation" runat="server" style=" width:130px" placeholder="操作者名称" tabindex="1" autocomplete="off"/>
                    </td>
                    <th>
                        状态
                    </th>
                    <td>
                        <select id="selFinancialState" class="Searchwhere2" runat="server">
                            <option value="-1">全部</option>
                            <option value="1">新提交</option>
                            <option value="2">生效</option>
                            <option value="3">无效</option>
                        </select>
                    </td>
                    <th>订单号</th>
                    <td>
                        <input type="text" id="txtOrdersNo" runat="server" placeholder="订单号" tabindex="1" autocomplete="off"/>
                    </td>
                    <th>
                        变动金额
                    </th>
                    <td>
                        <input id="txtStartMoney" runat="server" type="text" class="txt" style="width: 100px" placeholder="开始金额" tabindex="1" autocomplete="off"/>-
                        <input id="txtEndMoney" runat="server" type="text" class="txt" style="width: 100px" placeholder="结束金额" tabindex="1" autocomplete="off"/>
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="10">
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
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 40px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 80px; text-align: center;">
                        会员手机号
                    </td>
                    <td style="width: 60px; text-align: center;">
                        会员姓名
                    </td>
                    <td style="width: 120px; text-align: center;">
                        订单编号
                    </td>
                    <td style="width:100px; text-align: center;">
                        流水号
                    </td>
                    <td style="width:100px; text-align: center;">
                        交易号
                    </td>
                    <td style="width: 80px; text-align: center;">
                        变动金额
                    </td>
                    <td style="width: 120px; text-align: center;">
                        变动时间
                    </td>
                    <td style="width: 100px; text-align: center;">
                        变动类型
                    </td>
                    <td style="width: 80px; text-align: center;">
                        操作者
                    </td>
                    <td style="width: 80px; text-align: center;">
                        操作者类型
                    </td>
                    <td style="width: 80px; text-align: center;">
                        操作渠道
                    </td>
                    <td style="width: 100px; text-align: center;">
                        当前余额
                    </td>
                    <td style="width: 100px; text-align: center;">
                        状态
                    </td>
                    <td style="width: 100px; text-align: center;">
                        备注说明
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
                            <td style="width: 80px; text-align: center;">
                                <%#Eval("BindPhone")%>
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("RealName")%>
                            </td>
                            <td style="width: 120px; text-align: center;">
                                    <%#Eval("OrderNum")%>
                            </td>
                            <td style="text-align: center;white-space:nowrap;">
                                <%#Eval("OrderPayID")%>
                            </td>
                            <td style="text-align: center;white-space:nowrap;">
                                <%#Eval("TradeNo")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <%#Eval("ChangesAmount")%>
                            </td>
                            <td style="width: 120px; text-align: center;">
                                <%#Eval("ChangesTime")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialChangesType), Convert.ToInt32(Eval("ChangesType")))%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <%#Eval("OperatorName")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialOperatorType), Convert.ToInt32(Eval("OperatorType")))%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialOperatorWay), Convert.ToInt32(Eval("OperatorWay")))%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Eval("CurrentBalance")%>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.UserFinancialState), Convert.ToInt32(Eval("State")))%>
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
                                   Response.Write("<tr><td colspan='10' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
        <br />
        <uc1:PageControl ID="PageControl1" runat="server" PageSize="10" />
    </div>
        </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
