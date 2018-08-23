<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Article_Form.aspx.cs" ValidateRequest="false" Inherits="YR.Web.Manage.InfomationManage.Article_Form" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文章信息表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/ckfinder/ckfinder.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        //获取表单值
        function CheckValid() {
            if (!CheckDataValid('#form1')) {
                return false;
            }
            if (!confirm('您确认要保存此操作吗？')) {
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>文章类别:
                </th>
                <td>
                    <select id="CategoryID" runat="server" style="width: 200px;"></select>
                </td>
            </tr>
            <tr>
                <th>文章标题:
                </th>
                <td>
                    <input id="ArticleName" runat="server" type="text" class="txt" datacol="yes" err="文章名称"
                        checkexpession="NotNull" style="width: 400px" />
                </td>
            </tr>
            <tr>
                <th>文章内容:
                </th>
                <td>
                    <textarea id="ArticleContent" class="ckeditor" runat="server" style="width: 552px; height: 100px;"></textarea>
                </td>
            </tr>
            <tr>
                <th>显示次序:
                </th>
                <td>
                    <input id="Sort" runat="server" type="text" class="txt" datacol="yes" err="显示次序"
                        checkexpession="NumOrNull" style="width: 120px" />
                </td>
            </tr>
        </table>
        <div class="frmbottom">
            <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click" OnClientClick="return CheckValid();"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
            <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
                <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
        </div>
    </form>
</body>
</html>
