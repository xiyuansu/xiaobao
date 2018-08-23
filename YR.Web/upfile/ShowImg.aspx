<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowImg.aspx.cs" Inherits="YR.Web.upfile.ShowImg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/admin/Ajax/jquery/jquery.js"></script>
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            load();
        });

        function load() {
            var ids = getQueryString("obj");
            var type = getQueryString("type"); //是否显示删除图片按钮
            $.ajax({
                type: "POST",
                url: "/upfile/file.ashx",
                data: "action=getfile&fileid=" + ids,
                success: function (msg) {
                    var newid = '';
                    try {
                        var data = eval('(' + msg + ')');
                        var str = '<ul class="plistul">';
                        for (var i = 0; i < data.length; i++) {
                            newid += data[i]["id"] + ",";
                            var id = 'li_' + i;
                            str += '<li id="' + id + '" class="pList-item">';
                            if (type != 0) {
                                if (data[i]["IsDel"] == "0") {
                                    str += '<center><a style=\"cursor:pointer;\" onclick="del(&quot;' + data[i]["id"] + '&quot;,&quot;' + id + '&quot;,&quot;' + ids + '&quot;);"><span style=\"font-size:12px;\">删除</span></a></center>';
                                }
                            }
                            str += '<a onclick="showImgBig(&quot;' + data[i]["id"] + '&quot;);"><img border="0" src="/' + data[i]["FilePath"] + '"></a>';
                            str += '</li>';

                        }
                        str += '</ul>';
                        $("#div_imgShow").html(str);
                    } catch (e) {
                    }
                    //更新父控件hid值           
                    var hid = getQueryString("hiddenid");
                    try {
                        parent.document.getElementById(hid).value = newid;
                    } catch (e) {
                        try {
                            document.getElementById(hid).value = newid;
                        } catch (e) { }
                    }
                }
            });
        }
        function del(id, contr_li, ids) {
            $.ajax({
                url: "/upfile/file.ashx",
                data: "action=delfile&fileid=" + id,
                success: function (data) {
                    var msg = data.split(',');
                    if (msg[0] == "1") {
                        alert("删除成功!");
                        $("#" + contr_li).html("");
                        var newid = '';
                        var obj = getQueryString("obj").split(',');
                        for (var i = 0; i < obj.length; i++) {
                            if (obj[i] != id && obj[i] != "") {
                                newid += obj[i] + ',';
                            }
                        }
                        newid = newid.replace(/[ ]/g, "");
                        newid = newid.replace(/,$/gi, "");
                        var hid = getQueryString("hiddenid");
                        try {
                            parent.document.getElementById(hid).value = newid;
                        } catch (e) {
                            try {
                                document.getElementById(hid).value = newid;
                            } catch (e) { }
                        }

                    } else {

                        alert("删除失败!");
                    }
                },
                error: function () { alert("网路错误,稍后重试..."); }
            });
        }


        function showImgBig(id) {
            window.open('/upfile/BigShowImg.aspx?id=' + id);
        }
        function getQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }
    </script>
    <style type="text/css">
        #div_imgShow
        {
            text-align: center;
            padding: 0;
            margin: 0;
        }
        #div_imgShow img
        {
            margin: 10px;
        }
    </style>
    <style>
        body, div, dl, dt, dd, ul, ol, li, h1, h2, h3, h4, h5, h6, pre, code, form, fieldset, legend, input, textarea, p, blockquote, th, td
        {
            margin: 0;
            padding: 0;
        }
        .num_warp
        {
            margin: 10px;
        }
        
        .num_warp ul
        {
            margin: 0;
            padding-top: 23px;
        }
        .num_warp ul
        {
            list-style: none outside none;
            margin-top: 23px;
        }
        .pList-item
        {
            border: 3px solid #FFFFFF;
            display: inline;
            float: left;
            margin: 5px 3px;
            position: relative;
            width: 162px;
            z-index: 9;
        }
        .num_warp ul li
        {
            height: auto;
            margin: 0;
        }
        .num_warp ul li
        {
            float: left;
            margin: 0 14px 13px 0;
            height: auto;
            width: 200px;
        }
        .num_warp ul li img
        {
            height: auto;
            width: 200px;
        }
        .deltip
        {
            float: right;
            top: 0;
            margin-top: 0;
            border: solid 0px red;
            cursor: pointer;
        }
    </style>
</head>
<body>
    <div id="div_imgShow" class="num_warp">
    </div>
</body>
</html>
