<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MessageInfo.aspx.cs" Inherits="YR.Web.AppPage.MessageInfo" %>

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>消息详情</title>
    <script src="../api/h5/js/jquery.min.js"></script>
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
    <div id="divContent" runat="server" style="margin-top:15px;font-size:15px;word-wrap: break-word;word-break: normal;">
        
    </div>
    </form>
    <script type="text/javascript">
        $(function () {
            //if ($("img").width() > ($(window).width() - 20))
            //    $("img").removeAttr("style").css({ "width": ($(window).width() - 20) + "px", "height": "auto" });
            $("img").each(function () {
                if ($(this).width() > ($(window).width() - 20))
                    $(this).removeAttr("style").css({ "width": ($(window).width() - 20) + "px", "height": "auto" });
            });
        });
    </script>

</body>
</html>
