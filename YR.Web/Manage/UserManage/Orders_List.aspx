<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Orders_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.Orders_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
            $("#lbtExport").remove();
        })
        function Export(){
            __doPostBack('lbtExport', '');
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        //锁 定
        function lock() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var parm = 'action=lock&user_ID=' + key;
                showConfirmMsg('注：您确认要【锁 定】当前选中用户吗？', function (r) {
                    if (r) {
                        getAjax('UserInfoValidate.ashx', parm, function (rs) {
                            if (parseInt(rs) > 0) {
                                showTipsMsg("锁定成功！", 2000, 4);
                            }
                            else {
                                showTipsMsg("<span style='color:red'>锁定失败，请稍后重试！</span>", 4000, 5);
                            }
                        });
                    }
                });
            }
        }

        function orderTrace()
        {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/Orders_Trace.html?orderid=" + key;
                top.openDialog(url, 'Orders_Trace', '用户订单-轨迹', 1100, 550, 50, 50);
            }
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
            订单管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div style="float: left;">
            <table class="tabCondition">
                <tr>
                    <th>订单号</th>
                    <td>
                        <input type="text" id="txtOrdersNo" runat="server" style="width: 158px;" placeholder="订单号" tabindex="1" autocomplete="off"/>
                    </td>
                    <th>用  户</th>
                    <td>
                        <input type="text" id="txtUserName" runat="server" placeholder="昵称/真实姓名/手机号码" tabindex="1" autocomplete="off" style=" width:158px"/>
                    </td>
                    <th>车辆名称</th>
                    <td>
                        <input type="text" id="txtVehicleName" runat="server" style="width: 158px;" placeholder="车辆名称" tabindex="1" autocomplete="off"/>
                    </td>
                    <th>订单状态</th>
                    <td>
                        <select id="selOrderState" class="Searchwhere2" runat="server" style="width: 162px;">
                            <option value="-1">全部</option>
                            <option value="0">已取消</option>
                            <option value="1">进行中</option>
                            <option value="5">未支付</option>
                            <option value="2">已完成</option>
                            <option value="3">异常订单</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>订单时间</th>
                    <td>
                        <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                    <th>完成时间</th>
                    <td>
                        <input id="txtStartFinishedTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndFinishedTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                    <%--<th>公里数</th>
                    <td>
                        <input id="txtStartMileage" runat="server" type="text" class="txt" style="width: 70px" placeholder="开始里数" tabindex="1" autocomplete="off" />-
                        <input id="txtEndMileage" runat="server" type="text" class="txt" style="width: 70px" placeholder="结束里数" tabindex="1" autocomplete="off" />
                    </td>
                    <th>分钟数</th>
                    <td>
                        <input id="txtStartMinutes" runat="server" type="text" class="txt" style="width: 70px" placeholder="开始分钟数" tabindex="1" autocomplete="off" />-
                        <input id="txtEndMinutes" runat="server" type="text" class="txt" style="width: 70px" placeholder="结束分钟数" tabindex="1" autocomplete="off" />
                    </td>--%>
                    <th>订单来源</th>
                    <td>
                        <select id="selOrderSource" class="Searchwhere2" runat="server" style="width: 162px;">
                            <option value="-1">全部</option>
                            <option value="01">系统订单</option>
                            <option value="02">第三方订单</option>
                        </select>
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="10">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                            <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>
                            查 询
                        </asp:LinkButton>
                        <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                            <span class="icon-botton"></span>重 置
                        </asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 40px; text-align: center;">选择</td>
                    <td style="width: 120px; text-align: center;">订单编号</td>
                    <td style="width: 80px; text-align: center;">会员手机号</td>
                    <td style="width: 60px; text-align: center;">会员姓名</td>
                    <td style="width: 100px;text-align: center;">车辆</td>
                    <td style="width: 40px;text-align: center;">公里价格</td>
                    <td style="width: 40px;text-align: center;">分钟价格</td>
                    <td style="width: 40px; text-align: center;">公里数</td>
                    <td style="width: 40px; text-align: center;">分钟数</td>
                    <td style="width: 40px; text-align: center;">总金额</td>
                    <td style="width: 40px; text-align: center;">结算金额</td>
                    <td style="width: 40px; text-align: center;">支付金额</td>
                    <td style="width: 60px; text-align: center;">订单状态</td>
                    <td style="width: 120px; text-align: center;">开始时间</td>
                    <td style="width: 120px; text-align: center;">结束时间</td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: center;" title="第三方订单号:<%#Eval("OutOrderNo")%>">
                                <%#Eval("OrderNum")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("BindPhone")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("RealName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("LicenseNumber")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("KMPrice")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("MinutePrice")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Mileage")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Minutes")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("TotalMoney")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("SettlementMoney")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("PayMoney")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Asiasofti.SmartVehicle.Common.EnumHelper.GetEnumShowName(typeof(Asiasofti.SmartVehicle.Common.Enum.OrderState), Convert.ToInt32(Eval("OrderState")))%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("CreateTime")%>
                            </td>                            
                            <td style="text-align: center;">
                                <%#Eval("FinishedTime")%>
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
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
        </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
