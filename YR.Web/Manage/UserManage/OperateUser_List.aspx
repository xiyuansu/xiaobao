﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperateUser_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.OperateUser_List" %>
<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>运维人员管理</title>
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
        //新增
        function add() {
            var url = "/Manage/UserManage/OperateUser_Form.aspx";
            top.openDialog(url, 'OperateUser_Form', '运维人员信息 - 添加', 710, 600, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/UserManage/OperateUser_Form.aspx?key=" + key;
                top.openDialog(url, 'OperateUser_Form', '运维人员信息 - 编辑', 710, 600, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=运维人员管理&tableName=YR_OPUser&pkName=UserID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
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
        <div>
            运维人员管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div>
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
                    <td >
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
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 80px; text-align: center;">
                        用户名称
                    </td>
                    <td style="width: 80px; text-align: center;">
                        用户昵称
                    </td>
                    <td style="width: 40px; text-align: center;">
                        接收短信
                    </td>
                    <td style="width: 100px; text-align: center;">
                        联系电话
                    </td>
                    <td style="width:100px; text-align: center;">
                        电子邮箱
                    </td>
                    <td style="width: 40px; text-align: center;">
                        性别
                    </td>
                    <td style="width: 100px; text-align: center;">
                        身份证号
                    </td>
                    <td style="width: 40px;text-align: center;">
                        用户状态
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
                                <input type="checkbox" value="<%#Eval("UserID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("UserName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("NickName")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("ReceiveSMS")==DBNull.Value?"否":(Eval("ReceiveSMS").ToString()== "1" ? "是" : "否")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Tel")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Email")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("UserSex")==DBNull.Value?"":(Eval("UserSex").ToString()== "1" ? "男" : "女")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("IDCardNum")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("UserState").ToString() == "1" ? "有效" : "<font color='red'>无效</font>"%>
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
