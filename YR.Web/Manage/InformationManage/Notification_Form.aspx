<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notification_Form.aspx.cs" Inherits="YR.Web.Manage.InformationManage.Notification_Form" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>文章信息表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="/Themes/Scripts/jquery.uploadify/uploadify.css" rel="stylesheet" />
    <script src="/Themes/Scripts/jquery.uploadify/jquery-1.3.2.min.js"></script>
    <script src="/Themes/Scripts/jquery.uploadify/swfobject.js"></script>
    <script src="/Themes/Scripts/jquery.uploadify/jquery.uploadify.v2.1.0.min.js"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/webcalendar.js"></script>
    <script type="text/javascript">
        var totalSize = 0
        $(document).ready(function () {
            $("#uploadify").uploadify({
                'uploader': '/Themes/Scripts/jquery.uploadify/uploadify.swf',
                'script': '/Ajax/AjaxAppManage.aspx?type=uploadimg',
                'cancelImg': '/Themes/Scripts/jquery.uploadify/cancel.png',
                'folder': '<%=GetUploadImagePath()%>',
                'queueID': 'fileQueue',
                'auto': false,
                'multi': false,
                'sizeLimit': 2048 * 1024,
                'fileDesc': '支持格式:jpg/gif/jpeg/png/bmp.', //如果配置了以下的'fileExt'属性，那么这个属性是必须的    
                'fileExt': '*.jpg;*.gif;*.jpeg;*.png;*.bmp',//允许的格式  
                'onComplete': function (event, ID, fileObj, response, data) {
                    var today = new Date();
                    var htmlImag = "<img ID=\"Image1\" src=\"" + response + "\" />";

                    $("#hidPath").val(response);
                    $("#uploadImg").html(htmlImag);

                },
                'onSelect': function (fileObj) {


                },
                'onError': function (event, queueId, fileObj, errorObj) {
                    if (errorObj.type == "File Size") {
                        alert("上传失败,图片限制最大为：2M");
                    }

                }
            });
        });
        //获取表单值
        function CheckValid() {
            var returnValue = true;
            if (!confirm('您确认要保存此操作吗？')) {
                returnValue = false;
            } else {
                if ($.trim($("#txtMessageTitle").val()).length == 0) {
                    $("#txtMessageTitle").focus();
                    returnValue = false;
                    alert("请输入消息标题！");
                } else if ($.trim($("#txtBegin").val()).length == 0 || $.trim($("#txtEnd").val()).length == 0) {
                    $("#txtBegin").focus();
                    returnValue = false;
                    alert("请输入启用时间！");
                } else if (!checkTime()) {
                    returnValue = false;
                    alert("启用开始时间要小于结束时间！");
                }
                else if ($.trim($("#txtSummary").val()).length == 0) {
                    returnValue = false;
                    alert("请上填写信息简介！");
                }             
            }
            return returnValue;
        }

        function checkTime() {
            var start = new Date($("#txtBegin").val().replace("-", "/").replace("-", "/"));
            var end = new Date($("#txtBegin").val().replace("-", "/").replace("-", "/"));
            if (end < start) {
                return false;
            } else {
                return true;
            }
        }
    </script>


</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; margin-left: 30px; margin-right: 30px">
            <table style="margin-top: 20px; width: 98%; border: 1px solid black">
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">消息标题：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="txtMessageTitle" runat="server" Width="53.5%"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">启用区间：</td>
                    <td style="width: 80%; text-align: left;">

                        <asp:TextBox ID="txtBegin" Style="width: 26%;" runat="server" onclick="SelectDate(this,'yyyy-MM-dd hh:mm:ss')"></asp:TextBox>-<asp:TextBox ID="txtEnd" runat="server" Style="width: 26%;" onclick="SelectDate(this,'yyyy-MM-dd hh:mm:ss')"></asp:TextBox>

                    </td>

                </tr>

                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">信息简介：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="txtSummary" runat="server" Width="53.5%" TextMode="MultiLine" Height="30px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">上传图片：</td>
                    <td style="text-align: left">
                        <input type="file" name="uploadify" id="uploadify" />
                        <a href="javascript:$('#uploadify').uploadifyUpload()">上传</a>| <a href="javascript:$('#uploadify').uploadifyClearQueue()">取消上传</a>
                        <div id="fileQueue"></div>
                        <div id="uploadImg" runat="server" style="width: 600px; height: 100px; overflow-y: scroll; border: 1px solid black">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">消息内容：</td>
                    <td>
                        <CKEditor:CKEditorControl ID="ckeId" BasePath="../../Themes/Scripts/ckeditor%20net/ckeditor/" runat="server"></CKEditor:CKEditorControl>

                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">是否启用：</td>
                    <td style="text-align: left">
                        <asp:RadioButton ID="radOK" Checked="true" GroupName="radGroup" runat="server" Text="启用" /><asp:RadioButton ID="radNo" GroupName="radGroup" runat="server" Text="未启用" /><asp:HiddenField ID="hidPath" runat="server" />
                    </td>
                </tr>

            </table>
            <div class="frmbottom">
                <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClientClick="return CheckValid();" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
                <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
                    <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
            </div>
        </div>
    </form>
</body>
</html>