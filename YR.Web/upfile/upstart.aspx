<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upstart.aspx.cs" Inherits="YR.Web.upfile.upstart" %>

<%@ Register Assembly="MattBerseth.WebControls.AJAX" Namespace="MattBerseth.WebControls.AJAX.Progress"
    TagPrefix="mb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" href="_assets/css/progress.css" />
    <link rel="Stylesheet" href="_assets/css/upload.css" />
    <style type="text/css">
        BODY
        {
            font-family: Arial, Sans-Serif;
            font-size: 12px;
        }
    </style>
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="true" />
    <script type="text/javascript">
        var intervalID = 0;
        var progressBar;
        var fileUpload;
        var form;
        // 进度条      
        function pageLoad() {
            $addHandler($get('upload'), 'click', onUploadClick);
            progressBar = $find('progress');
        }
        // 注册表单       
        function register(form, fileUpload) {
            this.form = form;
            this.fileUpload = fileUpload;
        }
        function CheckFileName( filename ) {
            var re_text = /\gif|\jpg|\png|\jpeg/i;           //这名比较关键,定义上传的文件类型,允许上传的就写上.
            var newFileName = filename.split('.');        //这是将文件名以点分开,因为后缀肯定是以点什么结尾的.
            newFileName = newFileName[newFileName.length-1]; //这个是得到文件后缀
            if (newFileName.search(re_text) == -1) {
               var errormsg = "对不起,只能上传(*.jpg/*.gif/*.png)图片,请重新选择要上传的文件!";
               alert(errormsg);
               return false;
            }else{
                return true;
            }
        }
        //上传验证
        function onUploadClick() {
            var vaild = fileUpload.value.length > 0;
            if( CheckFileName(fileUpload.value) ){
                if (vaild) {
                    $get('upload').disabled = 'disabled';
                    updateMessage('info', '初始化上传...');
                    //提交上传
                    form.submit();
                    // 隐藏frame
                    Sys.UI.DomElement.addCssClass($get('uploadFrame'), 'hidden');
                    // 0开始显示进度条
                    progressBar.set_percentage(0);
                    progressBar.show();
                    // 上传过程
                    intervalID = window.setInterval(function () {
                        PageMethods.GetUploadStatus(function (result) {
                            if (result) {
                                //  更新进度条为新值
                                progressBar.set_percentage(result.percentComplete);
                                //更新信息
                                updateMessage('info', result.message);

                                if (result == 100) {
                                    dialog.returnValue = result.id;
                                    // 自动消失
                                    window.clearInterval(intervalID);
                                }
                            }
                        });
                    }, 500);
                }
                else {
                    onComplete('error', '您必需选择一个文件');
                }
            }else{
                onComplete('error', '对不起,只能上传(*.jpg/*.gif/*.png)图片,请重新选择要上传的文件!');

            }
            
        }

        function onComplete(type, msg, id, imgurl,name) {
            debugger;
            // 自动消失
            window.clearInterval(intervalID);
            // 显示消息
            //updateMessage(type, msg);
            msg += "&nbsp;&nbsp;<a href=\'/" + imgurl + "\' target=\'_blank\' style='font-size:14px; font-weight:bold; color:Blue;'>预览</a>";
            updateMessage(type, msg);

            // 隐藏进度条
            progressBar.hide();
            progressBar.set_percentage(0);
            // 重新启用按钮
            $get('upload').disabled = '';
            //  显示frame
            Sys.UI.DomElement.removeCssClass($get('uploadFrame'), 'hidden');
            
            if (type == "success") {
                btnRetrun(id, imgurl,name);
            }
        }
        function updateMessage(type, value) {
            var status = $get('status');
            status.innerHTML = value;
            // 移除样式
            status.className = '';
            Sys.UI.DomElement.addCssClass(status, type);
        }
        function btnRetrun(value, imgurl,name) {
            var hiddenid = getQueryString("hiddenid"); //隐藏域id
            if (hiddenid != "") {
                try {
                    if (value) {
                        if (value.length > 0 && value != "") {
                            var pdgv = parent.document.getElementById(hiddenid).value;
                            if(pdgv=="" || pdgv == "undefined"){
                                parent.document.getElementById(hiddenid).value = value;   
                            }else{
                                parent.document.getElementById(hiddenid).value += "," + value;
                            }                            
                        }
                    }
                } catch (e) {
                    alert(e);
                }
            }
            var msgid = getQueryString("msgid");
            if (msgid != "") {
                var div_id = 'contr_' + value;
                var msgstr = "<div id='" + div_id + "' class='imagekkk'><div class='imagekkkk'><a href='" + imgurl + "' target='_blank'><img src='" + imgurl + "' height='100px' width='100px' /></a><a class='delimg' href='javascript:deluploadimg(&quot;" + value + "&quot;,&quot;" + div_id + "&quot;,&quot;" + hiddenid + "&quot;);'>删除</a><div></div>";
                //var msgstr = "<div id='" + div_id + "' style='color:#00CC33'><span class='yes'>&nbsp;</span>上传成功!</div>";
                //var msgstr = "<div id='" + div_id + "'><span class='yes'>&nbsp;上传成功！</span></div>";
                try {
                    parent.document.getElementById(msgid).innerHTML += msgstr;
                    parent.document.getElementById(msgid).style.display = "block";

                    //移除验证样式
                    var obj = $("#"+hiddenid, window.parent.document);
                    var obj1 = obj.parent();
                    $(obj1).find("span").remove();
                    parent.asyncbox.close('showid');
                 
                } catch (e) {
                    
                }


            }
        }
        function getQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }
    </script>
    <div>
        <div class="upload">
            <h3>
                文件上传(最大允许上传10M.)</h3>
            <div>
                <iframe id="uploadFrame" frameborder="0" scrolling="no" src="Upload.aspx?category=<%=Request["category"]%>&type=<%=Request["type"]%>&relationid=<%=Request["relationid"]%>"></iframe>
                <mb:ProgressControl ID="progress" runat="server" CssClass="lightblue" Style="display: none"
                    Value="0" Mode="Manual" Speed=".4" Width="100%" />
                <div>
                    <div id="status" class="info">
                        请选择要上传的文件</div>
                    <div class="commands">
                        <input id="upload" type="button" value="上传" />
                    </div>
                </div>
                <asp:HiddenField runat="server" ID="test" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
