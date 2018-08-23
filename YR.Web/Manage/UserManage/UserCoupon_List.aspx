<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserCoupon_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.UserCoupon_List" %>
<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>优惠券列表</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#txtName,#txtTel').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })
        
        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
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
            优惠券列表
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
                        用户名称
                    </th>
                    <td>
                        <input type="text" id="txtName" runat="server" placeholder="用户名称" tabindex="1" autocomplete="on" style="width:90px;"/>
                    </td>
                    <th>
                        用户电话
                    </th>
                    <td>
                        <input type="text" id="txtTel" runat="server" placeholder="用户电话" tabindex="1" autocomplete="on" style="width:90px;"/>
                    </td>
                    <td colspan="10">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                            <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>查 询
                        </asp:LinkButton>
                        <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                            <span class="icon-botton">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>重 置
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
                    <td style="width: 30px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 100px; text-align: center;">
                        用户名称
                    </td>
                    <td style="width: 100px; text-align: center;">
                        联系电话
                    </td>
                    <td style="width: 40px; text-align: center;">
                        面值
                    </td>
                    <td style="width:60px; text-align: center;">
                        有效期起始时间
                    </td>
                    <td style="width: 160px; text-align: center;">
                        有效期结果时间
                    </td>
                    <td style="width: 80px; text-align: center;">
                        是否使用
                    </td>
                    <td style="width: 100px;text-align: center;">
                        创建时间
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("RealName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("BindPhone")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Money")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("BeginTime")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("EndTime")%>
                            </td>
                            <td style="text-align: center;">
                                <%#String.IsNullOrEmpty(Eval("OrderID").ToString())?"未使用":"已使用"%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("CreateTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='12' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
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
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
