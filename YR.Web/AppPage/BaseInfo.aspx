<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BaseInfo.aspx.cs" Inherits="YR.Web.AppPage.BaseInfo" %>

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title></title>
    <script src="/Themes/Scripts/jquery-1.8.2.min.js"></script>
    <style type="text/css">
        body{margin:0px;padding:10px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divTitle" runat="server" style="display:none;font-weight:bold;font-size:16px;margin-top:10px; margin-bottom:10px;">
        
    </div>
    <div id="divDate" runat="server" style="display:none;color:#a0a0a0;font-size:14px;">
        
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
