<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="YR.Web.upfile.upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <style type="text/css">
        BODY
        {
            margin: 0;
            padding: 0;
            background-color: #F0F8FF;
        }
    </style>
</head>
<body>
    <form id="form" runat="server" enctype="multipart/form-data">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <script type="text/javascript">
        function pageLoad(sender, args) {
            window.parent.register(
                $get('<%= this.form.ClientID %>'),
                $get('<%= this.fileUpload.ClientID %>')
            );
        }
    </script>
    <div>
        <asp:FileUpload ID="fileUpload" runat="server" Width="100%" />
    </div>
    </form>
</body>
</html>
