<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DictForm.aspx.cs" Inherits="YR.Web.YRBase.SysConfig.DictForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>字典表添加</title>
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
        var districtData,cityData;
        $(function () {
            $("#select").hide();
            var cityValue = $("#drpDic").val();
            change(cityValue);
            $("#drpDic").change(function () {
                change($(this).val());
            });
            getProvince();
            $("#selProvince").change(function () {
                var selValue = $(this).val();
                $("#selCity option:gt(0)").remove();
                $("#selDistrict option:gt(0)").remove();
                getCity(selValue);
            });
            $("#selCity").change(function () {
                var selValue = $(this).val();
                $("#selDistrict option:gt(0)").remove();
                getDistrict(selValue);
                $.each(cityData, function (k, p) {
                    if (p.ID == selValue) {
                        $("#txtName").val(p.SHORTNAME);
                        $("#txtLongitude").val(p.LNG);
                        $("#txtLatitude").val(p.LAT);
                        $("#txtRemark,#txtCityCode").val(p.CITYCODE);
                        $("#txtZipCode").val(p.ZIPCODE);
                    }
                });
            }); 
            $("#selDistrict").change(function () {
                var selValue = $(this).val();
                if (selValue != -1) {
                    $.each(districtData, function (k, p) {
                        if (p.ID == selValue) {
                            $("#txtName").val(p.SHORTNAME);
                            $("#txtLongitude").val(p.LNG);
                            $("#txtLatitude").val(p.LAT);
                            $("#txtRemark,#txtCityCode").val(p.CITYCODE);
                            $("#txtZipCode").val(p.ZIPCODE);
                        }
                    });
                }
                //else {$("#txtName,#txtLongitude,#txtLatitude,#txtRemark,#txtZipCode,#txtCityCode").val(""); }
            }); 
        })

        function getProvince() {
            $.ajax({
                url: "/Ajax/AjaxRequst.ashx",
                type: "POST",
                dataType: "json",
                data: { action: "GetDistrict", levelType: 1, parentId:0 },
                success: function (data, textStatus) {
                    if (data.state == "success") {
                        if (data.content.ServiceArea.length>0) {
                            $.each(data.content.ServiceArea, function (k, p) {
                                var option = "<option value='" + p.ID + "'>" + p.NAME + "</option>";
                                $("#selProvince").append(option);
                            });
                        }
                    }
                }
            });
        }

        function getCity(parentId) {
            $.ajax({
                url: "/Ajax/AjaxRequst.ashx",
                type: "POST",
                dataType: "json",
                data: { action: "GetDistrict", levelType: 2, parentId: parentId },
                success: function (data, textStatus) {
                    if (data.state == "success") {
                        if (data.content.ServiceArea.length > 0) {
                            cityData = data.content.ServiceArea;
                            $.each(data.content.ServiceArea, function (k, p) {
                                var option = "<option value='" + p.ID + "'>" + p.NAME + "</option>";
                                $("#selCity").append(option);
                            });
                        }
                    }
                }
            });
        }

        function getDistrict(parentId) {
            $.ajax({
                url: "/Ajax/AjaxRequst.ashx",
                type: "POST",
                dataType: "json",
                data: { action: "GetDistrict", levelType: 3, parentId: parentId },
                success: function (data, textStatus) {
                    if (data.state == "success") {
                        if (data.content.ServiceArea.length > 0) {
                            districtData = data.content.ServiceArea;
                            $.each(data.content.ServiceArea, function (k, p) {
                                var option = "<option value='" + p.ID + "'>" + p.NAME + "</option>";
                                $("#selDistrict").append(option);
                            });
                        }
                    }
                }
            });
        }
       
        function change(v) {
            if (v == "03") {
                $("#select").show();
                $("#latlng").show();
                $("#tdTitle").html("城市区号：");
                $("#tdCity").html("城市名称：");
                //$("#txtName,#txtLongitude,#txtLatitude,#txtRemark").attr("disabled",true);
            } else {
                $("#select").hide();
                $("#latlng").hide();
                $("#tdTitle").html("备注说明：");
                $("#tdCity").html("条目名称：");
               //$("#txtName,#txtLongitude,#txtLatitude,#txtRemark").removeAttr("disabled");
            }
        }

        //获取表单值 http://localhost:16672/YRBase/SysConfig/DictForm.aspx
        function CheckValid() {
            var returnValue = true;
            var name = $.trim($("#txtName").val());
            var remark = $.trim($("#txtRemark").val());
            var sort = $.trim($("#txtSort").val());
            if ($("#drpDic").val() == "-1") {
                returnValue = false;
                alert("请选择字典类型!");
            }else{
                if ($("#drpDic").val()  == "03") {
                    if ($("#selCity").val() == -1 && name.length == 0) {
                        alert("请输入城市或地区！");
                        returnValue = false;
                    } else if (remark.length == 0) {
                        returnValue = false;
                        alert("请输入城市区号!");
                    }  else if (isNaN(sort)) {
                        returnValue = false;
                        alert("请输入有效排序顺序!");
                    } else if (!confirm('您确认要保存此操作吗？')) {
                        returnValue = false;
                    }
                } else {
                    if (name.length == 0) {
                        alert("请输入项目名称！");
                        returnValue = false;
                    } else if (isNaN(sort)) {
                        returnValue = false;
                        alert("请输入有效排序顺序!");
                    } else if (!confirm('您确认要保存此操作吗？')) {
                        returnValue = false;
                    }
                }
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
                    <td style="width: 20%; text-align: right; font-weight: 800">字典类型：</td>
                    <td style="text-align: left">
                        <asp:DropDownList ID="drpDic" runat="server">
                            <asp:ListItem Value="-1">-----请选择-----</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="select">
                    <td style="width: 20%; text-align: right; font-weight: 800">选择地区：</td>
                    <td style="width: 80%; text-align: left;">
                        <select name="selProvince" id="selProvince" style="width: 30%;">
                            <option value="-1">请选择省份</option>
                        </select>
                        <select name="selCity" id="selCity" style="width: 30%;">
                            <option value="-1">请选择城市</option>
                        </select>
                        <select name="selDistrict" id="selDistrict" style="width: 30%;">
                            <option value="-1">请选择区</option>
                        </select>
                        <asp:TextBox ID="txtZipCode" runat="server" style="display:none;"></asp:TextBox>
                        <asp:TextBox ID="txtCityCode" runat="server" style="display:none;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800" id="tdCity">条目名称：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="txtName" runat="server" Width="30%"></asp:TextBox>
                        <span id="latlng" style="display: none;">经度：<asp:TextBox ID="txtLongitude" runat="server" Width="30%"></asp:TextBox>
                            纬度：<asp:TextBox ID="txtLatitude" runat="server" Width="30%"></asp:TextBox>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800" id="tdTitle">备注说明：</td>
                    <td style="width: 80%; text-align: left;">
                        <asp:TextBox ID="txtRemark" runat="server" Width="53.5%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">是否启用：</td>
                    <td style="text-align: left">
                        <asp:RadioButton ID="radOK" Checked="true" GroupName="radGroup" runat="server" Text="启用" /><asp:RadioButton ID="radNo" GroupName="radGroup" runat="server" Text="未启用" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 20%; text-align: right; font-weight: 800">排序顺序：</td>
                    <td style="text-align: left">
                        <asp:TextBox ID="txtSort" runat="server" Width="10%"></asp:TextBox>
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
