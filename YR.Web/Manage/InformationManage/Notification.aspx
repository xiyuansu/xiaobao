<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notification.aspx.cs" Inherits="YR.Web.Manage.InformationManage.Notification" %>

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>消息详情</title>
    <script src="/Themes/Scripts/jquery-1.8.2.min.js"></script>
    <style type="text/css">
        body
        {
            margin:0px;
            padding:10px;
            font-size:12px;
            background-color:#f3f3f3;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divTitle" runat="server" style="font-weight:bold;font-size:16px;margin-top:10px; margin-bottom:10px;">
        
    </div>
    <div id="divDate" runat="server" style="color:#a0a0a0;font-size:14px;">
        
    </div>
    <div id="divContent" runat="server" style="margin-top:15px;">
        
    </div>
    </form>
    <script type="text/javascript">
        $(function () {
            $("img").removeAttr("style").css({ "width": ($(window).width() - 20) + "px", "height": "auto" });
        });
    </script>

</body>
</html>
