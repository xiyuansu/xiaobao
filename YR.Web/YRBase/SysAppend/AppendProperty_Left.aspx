﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppendProperty_Left.aspx.cs"
    Inherits="YR.Web.YRBase.SysAppend.AppendProperty_Left" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>附加属性Left所属功能</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/TreeView/treeview.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/TreeView/treeview.pack.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化
        $(function () {
            divresize(31);
            treeAttrCss();
        })
        //点击事件
        function Property_Function(Function) {
            var path = 'AppendProperty_List.aspx?Function=' + escape(Function);
            window.parent.frames["target_right"].location = path;
            Loading(true);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="btnbartitle">
        <div>
            所属功能
        </div>
    </div>
    <div class="div-body">
        <ul class="strTree">
            <li>
                <div class="node">
                    所属功能</div>
                <ul>
                    <%=strHtml.ToString() %>
                </ul>
            </li>
        </ul>
    </div>
    </form>
</body>
</html>
