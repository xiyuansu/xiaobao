<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Article_List.aspx.cs" Inherits="YR.Web.Manage.InfomationManage.Article_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>文章管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })
        //新增
        function add() {
            var url = "/Manage/InformationManage/Article_Form.aspx";
            top.openDialog(url, 'Pits_Form', '文章信息 - 添加', 800, 600, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/InformationManage/Article_Form.aspx?key=" + key;
                top.openDialog(url, 'Pits_Form', '文章信息 - 编辑', 800, 600, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            var sysflag = $("#SysFlag_" + key).val();
            if (sysflag == "1")
            {
                showTipsMsg('系统预设文章无法删除', '5000', '5');
                return;
            }
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=文章管理&tableName=YR_Articles&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
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
                    <div>文章管理</div>
                </div>
                <div class="btnbarcontetn">
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                    <div style="float: left;">
                        <table class="tabCondition">
                            <tr>
                                <th>类型</th>
                                <td>
                                    <select id="selCatelog" runat="server" style="width:94px">
                                    </select>
                                </td>
                                <th>文章标题</th>
                                <td>
                                    <input type="text" id="txtTitle" runat="server" style="width: 176px;" />
                                </td>
                                <td>
                                    <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                                        <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>
                                        查 询
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                                        <span class="icon-botton"></span>
                                        重 置
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
                                <td style="width: 30px; text-align: center;">选择</td>
                                <td style="width: 80px; text-align: center;">标题</td>
                                <td style="width: 80px; text-align: center;">文章类别</td>
                                <td style="width: 80px; text-align: center;">顺序</td>
                                <td style="width: 80px; text-align: center;">系统预设</td>
                                <td style="width: 80px; text-align: center;">发布时间</td>
                                <td style="width: 80px; text-align: center;">操作员</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <input type="hidden" id="<%#"SysFlag_"+Eval("ID")%>" value="<%#Eval("SysFlag")%>" />
                                        <td style="text-align: center;">
                                            <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("ArticleName")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#GetCatelog(Eval("CategoryID").ToString())%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Sort")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("SysFlag").ToString()=="1"?"是":"否"%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("ReleaseTime")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Releaser")%>
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
                    </table>
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
