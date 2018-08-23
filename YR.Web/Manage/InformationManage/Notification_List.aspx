<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notification_List.aspx.cs" Inherits="YR.Web.Manage.InformationManage.Notification_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>文章管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="../../Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="../../Themes/Scripts/webcalendar.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#txtMessageTitle,#txtReleaseTime').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })
        //新增
        function add() {
            var url = "/Manage/InformationManage/Notification_Form.aspx";
            top.openDialog(url, 'Notification_Form', '消息 - 添加', 1200, 700, 50, 50);
        }

        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/InformationManage/Notification_Form.aspx?key=" + key;
                top.openDialog(url, 'Pits_Form', '消息 - 编辑', 1200, 700, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            var selValue = key.split(',');
            if (IsDelData(selValue[0])) {
                var delparm = 'action=Virtualdelete&module=消息管理&tableName=YR_Messages&pkName=ID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        function ReviewInfo(ID) {;
            var url = "/Manage/InformationManage/Notification.aspx?id=" + ID;
            top.openDialog(url, 'Pits_Form', 'App信息提示 - 预览', 900, 600, 50, 50);
        }
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
                消息提醒
            </div>
        </div>
        <div class="btnbarcontetn">
            <div style="float: left;">
            </div>
            <div style="text-align: right">
                <uc1:LoadButton ID="LoadButton1" runat="server" />
            </div>
            <div>
                <table class="tabCondition">
                    <tr>
                        <th>信息标题</th>
                        <td>
                            <input type="text" id="txtMessageTitle" runat="server" placeholder="信息标题" tabindex="1" autocomplete="on" style="width:90px;" />
                        </td>
                        <th>发布时间</th>
                        <td>
                            <input type="text" id="txtReleaseTime" runat="server" placeholder="发布时间" onclick="SelectDate(this, 'yyyy-MM-dd hh:mm:ss')" tabindex="1" autocomplete="on" style="width:70px;" />
                        </td>
                        <th>启动时间</th>
                        <td>
                            <input id="txtStartCreateTime" runat="server" type="text" class="txt" onclick="SelectDate(this, 'yyyy-MM-dd hh:mm:ss')" checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                            <input id="txtEndCreateTime" runat="server" type="text" class="txt" onclick="SelectDate(this, 'yyyy-MM-dd hh:mm:ss')" checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                        </td>
                        <th>状态</th>
                        <td>
                            <asp:DropDownList ID="drpState" runat="server" Style="width: 70px;">
                                <asp:ListItem Selected="True" Value="-1">请选择</asp:ListItem>
                                <asp:ListItem Value="1">启用</asp:ListItem>
                                <asp:ListItem Value="0">未启用</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>查 询</asp:LinkButton>
                            <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click"><span class="icon-botton"></span>重 置</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="div-body">
            <table id="table1" class="grid" singleselect="true">
                <thead>
                    <tr>
                        <td style="width: 5%; text-align: center;">选择</td>
                        <td style="width: 40%; text-align: center;">消息标题</td>
                        <td style="width: 10%; text-align: center;">发布时间</td>
                        <td style="width: 10%; text-align: center;">发布者</td>
                        <td style="width: 10%; text-align: center;">开始时间</td>
                        <td style="width: 10%; text-align: center;">结束时间</td>
                        <td style="width: 5%; text-align: center">状态</td>
                        <td style="width: 10%; text-align: center">预览内容</td>
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
                                    <%#Eval("MessageTitle")%>
                                </td>
                                <td style="text-align: center;">
                                    <%#Eval("ReleaseTime")%>
                                </td>
                                <td style="text-align: center;">
                                    <%#Eval("User_Name")%>
                                </td>
                                <td style="text-align: center;">
                                    <%#Eval("BeginTime")%>
                                </td>
                                <td style="text-align: center;">
                                    <%#Eval("EndTime")%>
                                </td>
                                <td style="text-align: center;">
                                    <%#Eval("State")%>
                                </td>
                                <td style="text-align: center;">
                                    <a href="#" style="text-decoration: underline; color: Blue;" onclick="ReviewInfo('<%#Eval("ID")%>');">浏览内容 </a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <% if (rp_Item != null)
                               {
                                   if (rp_Item.Items.Count == 0)
                                   {
                                       Response.Write("<tr><td colspan='5' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
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
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
        </asp:UpdatePanel>    
    </form>
</body>
</html>