<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sys_SettingForm.aspx.cs" Inherits="YR.Web.YRBase.SysConfig.Sys_SettingForm" %>

<!DOCTYPE HTML>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>系统参数配置修改</title>
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
    <script>

        $(document).ready(function () {
            $("#uploadify").uploadify({
                'uploader': '/Themes/Scripts/jquery.uploadify/uploadify.swf',
                'script': '/Ajax/AjaxAppManage.aspx?type=uploadsysset',
                'cancelImg': '/Themes/Scripts/jquery.uploadify/cancel.png',
                'folder': '/Resource/Image/other/' + $("#hidId").val(),
                'queueID': 'fileQueue',
                'auto': false,
                'multi': false,
                'sizeLimit': 4096 * 1024,
                'fileDesc': '支格式:jpg/gif/jpeg/png/bmp.', //如果配置了以下的'fileExt'属性，那么这个属性是必须的    
                'fileExt': '*.jpg;*.gif;*.jpeg;*.png;*.bmp',//允许的格式  
                'onComplete': function (event, ID, fileObj, response, data) {
                    var today = new Date();
                    var htmlImag = "<img  src=\"/Resource/Image/other/" + $("#hidId").val() + response + "\" />";
                    ////alert("/Resource/Image/other/" + $("#hidId").val() + response);
                    $("#hidPath").val("/Resource/Image/other/" + $("#hidId").val() + response);
                    $("#uploadImg").html(htmlImag);

                },
                'onSelect': function (fileObj) {


                },
                'onError': function (event, queueId, fileObj, errorObj) {
                    if (errorObj.type == "File Size") {
                        alert("上传失败,图片限制最大为：4M");
                    }

                }
            });
        });

        //获取表单值
        function CheckValid() {

            var returnValue = true;
            var settingContent = $.trim($("#txtSettingContent").val());

            if ($("#PanelTxt").length == 1) {
                if (settingContent.length == 0) {
                    alert("请输入参数值！");
                    returnValue = false;
                }
            } else if ($("#PanelImg").length == 1) {
                if (uploadImg.html() == "") {
                    alert("请上传图片！");
                    returnValue = false;
                }
            } else if ($("#drpDic").val() == "-1") {
                alert("请选择参数类别！");
                returnValue = false;
            }
            return returnValue;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; margin-left: 30px; margin-right: 30px">
            <table style="margin-top: 20px; width: 98%; border: 1px solid black; line-height: 50px;">
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">参数名：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="txtName" runat="server" Width="53.5%" ReadOnly="true"></asp:TextBox></td>
                </tr>
                <tr runat="server" id="trTxt">
                    <td style="width: 20%; text-align: right; font-weight: 800">参数值：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:Panel ID="PanelTxt" runat="server">
                            <asp:TextBox ID="txtSettingContent" runat="server" Width="53.5%"></asp:TextBox>
                        </asp:Panel>
                        <asp:Panel ID="PanelImg" runat="server">
                            <div id="imgDiv" runat="server">
                                <p>
                                    <input id="uploadify" name="uploadify" type="file" />
                                    <a href="javascript:$('#uploadify').uploadifyUpload()">上传</a>| <a href="javascript:$('#uploadify').uploadifyClearQueue()">取消上传</a>
                                </p>
                                <div id="fileQueue">
                                </div>
                                <div id="uploadImg" runat="server" style="width: 600px; height: 300px; overflow-y: scroll; border: 1px solid black">
                                </div>
                                <asp:HiddenField ID="hidPath" runat="server" />
                                <asp:HiddenField ID="hidId" runat="server" />
                            </div>
                        </asp:Panel>

                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">说明：</td>
                    <td style="text-align: left">
                        <asp:TextBox ID="txtDescription" runat="server" Width="53%" Height="69px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">参数类别：</td>
                    <td style="text-align: left">
                        <asp:DropDownList ID="drpDic" runat="server">
                            <asp:ListItem Value="-1">-----请选择-----</asp:ListItem>
                        </asp:DropDownList>
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
