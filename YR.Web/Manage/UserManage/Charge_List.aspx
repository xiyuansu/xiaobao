<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Charge_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.Charge_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>充值规则列表</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>    
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })
        //新增
        function add() {
            var url = "/Manage/UserManage/Charge_Form.aspx";
            top.openDialog(url, 'User_Form', '充值规则设置 - 添加', 700, 350, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/Charge_Form.aspx?key=" + key;
                top.openDialog(url, 'User_Form', '充值规则设置 - 编辑', 700, 350, 50, 50);
            }
        }
        // 删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Delete&module=充值规则设置&tableName=YR_RechargeRules&pkName=ID&pkVal=' + key;
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
            充值规则列表
        </div>
    </div>
    <div class="btnbarcontetn">        
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        <div style="float: left;">
            <table class="tabCondition">
                <tr>
                    <th>名  称</th>
                    <td>
                        <input type="text" id="txtName" runat="server" placeholder="名称" tabindex="1" autocomplete="off" style=" width:158px"/>
                    </td>                  
                    <th>有效日期</th>
                    <td>
                        <input id="txtBeginTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始日期" tabindex="1" autocomplete="off" />-
                        <input id="txtEndTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="终止日期" tabindex="1" autocomplete="off" />
                    </td>
                    <th>创建时间</th>
                    <td>
                        <input id="txtCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="创建时间" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="6">
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
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 50px; text-align: center;">
                        名称
                    </td>
                    <td style="width: 50px; text-align: center;">
                        充值金额
                    </td>
                    <td style="width: 50px; text-align: center;">
                        赠送金额
                    </td> 
                    <td style="width: 50px;text-align: center;">
                        开始日期
                    </td>
                    <td style="width: 50px;text-align: center;">
                        终止日期
                    </td>
                    <td style="width: 50px;text-align: center;">
                        操作员
                    </td>
                    <td style="width: 50px;text-align: center;">
                        显示次序
                    </td>
                    <td style="width: 50px;text-align: center;">
                        创建时间
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="width: 20px; text-align: center;" class="ID">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="width: 50px; text-align: center;" class="Name">
                                <%#Eval("Name")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="LowestMoney">
                                <%#Eval("ChargeMoney")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="Presented">
                                <%#Eval("PresentMoney")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="BeginTime">
                                <%#Eval("BeginTime")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="EndTime">
                                <%#Eval("EndTime")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="EndTime">
                                <%#Eval("Operator")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="EndTime">
                                <%#Eval("Sort")%>
                            </td>
                            <td style="width: 50px; text-align: center;" class="CreateTime">
                                <%#Eval("CreateTime")%>
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
