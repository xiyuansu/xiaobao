﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppendProperty_Index.aspx.cs"
    Inherits="YR.Web.YRBase.SysAppend.AppendProperty_Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>附加属性</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("html").css("overflow", "hidden");
            $("body").css("overflow", "hidden");
            iframeresize();
            Loading(true);
            $("#target_right").attr("src", "AppendProperty_List.aspx");
            $("#target_left").attr("src", "AppendProperty_Left.aspx");
        })
        /**自应高度**/
        function iframeresize() {
            resizeU();
            $(window).resize(resizeU);
            function resizeU() {
                var iframeMain = $(window).height();
                $("#iframeMainContent").height(iframeMain - 59);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="btnbartitle">
        <div>
            附加属性设置
        </div>
    </div>
    <div class="btnbarcontetn" style="margin-bottom: 1px;">
        <div style="float: left;">
            <table style="padding: 0px; margin: 0px; height: 100%;" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="menutab" style="vertical-align: bottom;">
                        <div id="tab0" class="Tabsel" onclick="GetTabClick(this);Urlhref('AppendProperty_Index.aspx');">
                            附加资料</div>
                        <div id="tab1" class="Tabremovesel" onclick="GetTabClick(this);Urlhref('AppendProperty_Function.aspx');">
                            所属功能</div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="iframeMainContent">
        <div class="iframeleft">
            <iframe id="target_left" name="target_left" scrolling="auto" frameborder="0" scrolling="yes"
                width="100%" height="100%"></iframe>
        </div>
        <div class="iframeContent">
            <iframe id="target_right" name="target_right" scrolling="auto" frameborder="0" scrolling="yes"
                width="100%" height="100%"></iframe>
        </div>
    </div>
    </form>
</body>
</html>
