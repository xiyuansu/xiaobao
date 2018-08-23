<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserReportLoss_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.UserReportLoss_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>用户挂失管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
        <script type="text/javascript">
            // 解挂
            function NoReportLoss() {
                var key = CheckboxValue();
                if (IsEditdata(key)) {
                    var url = "/Manage/UserManage/User_NoReportLoss.aspx?key=" + key;
                    top.openDialog(url, 'User_ReportLoss', '用户信息 - 解挂', 700, 350, 50, 50);
                }
            }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="btnbartitle">
        <div>
            用户挂失管理
        </div>
    </div>
    <div class="btnbarcontetn">
        
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 100px; text-align: center;">
                        会员手机号
                    </td>
                    <td style="width: 60px; text-align: center;">
                        会员姓名
                    </td>
                    <td style="width: 60px; text-align: center;">
                        会员昵称
                    </td>
                    <td style="width: 80px;text-align: center;">
                       状态
                    </td>
                    <td style="width: 180px;text-align: center;">
                        挂失时间
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
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("BindPhone")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <a href="javascript:void()">
                                <%#Eval("RealName")%></a>
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <%#Eval("NickName")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("State").ToString()=="0"?"已挂失":"已解挂"%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("ReportLossTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='6' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
    </form>
</body>
</html>
