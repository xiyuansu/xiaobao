<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Vehicle_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Vehicle_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>车辆信息表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/ImageUpload.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Themes/Scripts/ckeditor/ckeditor.js"></script>
    <script type="text/javascript" src="/Themes/Scripts/ckfinder/ckfinder.js"></script>
    <script type="text/javascript">
        //初始化
        $(function () {
            $('#table2').hide();
            $('#table3').hide();
            $('#table4').hide();
            $('#table5').hide();
            $('#table6').hide();

            $("#btnUpMaxImage").click(function () {
                CommonData('up_max_image_id', '1', 'divMaxImage_msg', 1, 2, '<%=Request["key"] %>');
            });
            $("#btnUpMinImage").click(function () {
                CommonData('up_min_image_id', '1', 'divMinImage_msg', 1, 1, '<%=Request["key"] %>');
            });
        })

        //点击切换面板
        var IsFixedTableLoad = 1;
        function panel(obj) {
            if (obj == 1) {
                $('#table1').show();
                $('#table2').hide();
                $('#table3').hide();
                $('#table4').hide();
                $('#table5').hide();
                $('#table6').hide();
            } else if (obj == 2) {
                $('#table1').hide();
                $("#table2").show();
                $('#table3').hide();
                $('#table4').hide();
                $('#table5').hide();
                $('#table6').hide();
            } else if (obj == 3) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').show();
                $('#table4').hide();
                $('#table5').hide();
                $('#table6').hide();
            } else if (obj == 4) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').hide();
                $('#table4').show();
                $('#table5').hide();
                $('#table6').hide();
            } else if (obj == 5) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').hide();
                $('#table4').hide();
                $('#table5').show();
                $('#table6').hide();
            } else if (obj == 6) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').hide();
                $('#table4').hide();
                $('#table5').hide();
                $('#table6').show();
            }
        }
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
        <div class="frmtop">
            <table style="padding: 0px; margin: 0px; height: 100%;" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="menutab" style="vertical-align: bottom; display: none;">
                        <div id="tab0" class="Tabsel" onclick="GetTabClick(this);panel(1)">
                            基本信息
                        </div>
                        <div id="tab1" class="Tabremovesel" onclick="GetTabClick(this);panel(2);">
                            车辆介绍
                        </div>
                        <div id="tab2" class="Tabremovesel" onclick="GetTabClick(this);panel(3);">
                            价格体系
                        </div>
                        <div id="tab3" class="Tabremovesel" onclick="GetTabClick(this);panel(4);">
                            用车须知
                        </div>
                        <div id="tab4" runat="server" visible="false" class="Tabremovesel" onclick="GetTabClick(this);panel(5);">
                            缩略图
                        </div>
                        <div id="tab5" runat="server" visible="false" class="Tabremovesel" onclick="GetTabClick(this);panel(6);">
                            详细图
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="div-frm" style="height: 422px;">
            <%--基本信息--%>
            <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <th>车辆名称:
                    </th>
                    <td>
                        <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="车辆名称"
                            checkexpession="NotNull" style="width: 200px" />
                    </td>
                    <th>车牌号:
                    </th>
                    <td>
                        <input id="LicenseNumber" runat="server" type="text" class="txt" datacol="yes" err="车牌号" checkexpession="NotNull" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>车辆型号:
                    </th>
                    <td>
                        <select id="drpVModel" runat="server" style="width: 200px;" checkexpession="NotNull"></select>
                    </td>
                    <th>发动机号:
                    </th>
                    <td>
                        <input id="EngineNum" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>车辆颜色:
                    </th>
                    <td>
                        <input id="VehicleColor" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th>车辆排量:
                    </th>
                    <td>
                        <input id="Displacement" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>车架号:
                    </th>
                    <td>
                        <input id="VehicleNum" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th>车辆模块号:
                    </th>
                    <td>
                        <input id="VehicleGPSNum" runat="server" type="text" class="txt" datacol="yes" err="车辆模块号" checkexpession="NotNull" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>燃料方式:
                    </th>
                    <td>
                        <select id="FuelStyle" runat="server" style="width: 200px">
                            <option value="电">电</option>
                        </select>
                    </td>
                    <th>平台类型:
                    </th>
                    <td>
                        <select id="PlatformId" runat="server" style="width: 200px">
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>车辆品牌:
                    </th>
                    <td>
                        <input id="Brand" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th>公里数:
                    </th>
                    <td>
                        <input id="Mileage" runat="server" type="text" class="txt" style="width: 200px" value="0" disabled="disabled" />
                    </td>
                </tr>
                <tr>
                    <th>使用状态:
                    </th>
                    <td>
                        <select id="UseState" runat="server" style="width: 200px" disabled="disabled">
                            <option value="1">空闲</option>
                            <option value="2">使用中</option>
                        </select>
                    </td>
                    <th>车辆状态:
                    </th>
                    <td>
                        <select id="VehicleState" runat="server" style="width: 200px">
                            <option value="1">可用</option>
                            <option value="2">不可用</option>
                            <option value="3">维修</option>
                            <option value="4">丢失</option>
                            <option value="5">其它</option>
                            <option value="7">故障</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>满电续航里程:
                    </th>
                    <td>
                        <input id="ExpectRange" runat="server" type="text" class="txt" datacol="yes" err="满电续航里程"
                            checkexpession="NotNull" value="50" style="width: 200px" />
                    </td>
                    <th>最高时速:
                    </th>
                    <td>
                        <input id="ExpectHighestSpeed" runat="server" type="text" class="txt" datacol="yes" err="最高时速"
                            checkexpession="NotNull" value="30" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <th>电量:
                    </th>
                    <td>
                        <input id="Electricity" runat="server" type="text" class="txt" datacol="yes" err="电量"
                            style="width: 200px" value="0" disabled="disabled" />%
                    </td>
                    <th>续航里程:
                    </th>
                    <td>
                        <input id="Range" runat="server" type="text" class="txt" datacol="yes" err="续航里程"
                            style="width: 200px" value="0" disabled="disabled" />
                    </td>
                </tr>
                <tr>
                    <th>车辆手机号：
                    </th>
                    <td>
                        <input id="VehicleMobile" runat="server" type="text" class="txt" datacol="yes" style="width: 200px" />
                    </td>
                    <th>所在城市：
                    </th>
                    <td>
                        <select id="CityID" runat="server" style="width: 200px"></select>
                    </td>
                </tr>
            </table>
            <%--车辆介绍--%>
            <table id="table2" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <td>
                        <textarea id="VehicleRemark" runat="server" cols="20" rows="2" class="ckeditor"></textarea>
                    </td>
                </tr>
            </table>
            <%--价格体系--%>
            <table id="table3" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <td>
                        <textarea id="PriceSystem" runat="server" cols="20" rows="2" class="ckeditor"></textarea>
                    </td>
                </tr>
            </table>
            <%--用车须知--%>
            <table id="table4" border="0" cellpadding="0" cellspacing="0" class="frm">
                <tr>
                    <td>
                        <textarea id="TransportInformation" runat="server" cols="20" rows="2" class="ckeditor"></textarea>
                    </td>
                </tr>
            </table>
            <%--车辆缩略图片--%>
            <div id="table5">
                <div class="div-body" style="height: 295px;">
                    <table>
                        <tr>
                            <td>缩略图
                            </td>
                            <td>
                                <img src="/Themes/images/btnUpload.png" id="btnUpMinImage" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div id="divMinImage_msg" class="imagekk">
                                    <% if (VehicleImageTable != null)
                                       {
                                           string strFileIDs = string.Empty;
                                           string strFileNames = string.Empty;
                                    %>
                                    <% foreach (System.Data.DataRow dr in VehicleImageTable.Rows)
                                       {
                                           if (dr["Type"].ToString() == "1")
                                           {
                                               strFileIDs += "," + dr["ID"];
                                               strFileNames += "," + dr["ImageName"];%>
                                    <div id="contr_<%=dr["ID"] %>" class="imagekkk">
                                        <div class="imagekkkk">
                                            <a href="<%=dr["ImageUrl"] %>" target="_blank">
                                                <img src='<%=dr["ImageUrl"]%>' height="100px" width="100px" /></a> <a class="delimg"
                                                    href='javascript:deluploadimg("<%=dr["ID"] %>","contr_<%=dr["ID"] %>","up_min_image_id")'>删除</a>
                                        </div>
                                    </div>
                                    <%}
                                   } %>
                                    <input type="hidden" id="up_min_image_id" name="up_min_image_id" value="<%= strFileIDs%>" />
                                    <input type="hidden" id="up_min_image_name" name="up_min_image_name" value="<%= strFileNames%>" />
                                    <%} %>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <%--车辆详细照片--%>
            <div id="table6">
                <div class="div-body" style="height: 295px;">
                    <table>
                        <tr>
                            <td>详细图
                            </td>
                            <td>
                                <img src="/Themes/images/btnUpload.png" id="btnUpMaxImage" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div id="divMaxImage_msg" class="imagekk">
                                    <% if (VehicleImageTable != null)
                                       {
                                           string strFileIDs = string.Empty;
                                           string strFileNames = string.Empty;
                                    %>
                                    <% foreach (System.Data.DataRow dr in VehicleImageTable.Rows)
                                       {
                                           if (dr["Type"].ToString() == "2")
                                           {
                                               strFileIDs += "," + dr["ID"];
                                               strFileNames += "," + dr["ImageName"];%>
                                    <div id="contr_<%=dr[0] %>" class="imagekkk">
                                        <div class="imagekkkk">
                                            <a href="<%=dr["ImageUrl"] %>" target="_blank">
                                                <img src='<%=dr["ImageUrl"]%>' height="100px" width="100px" /></a> <a class="delimg"
                                                    href='javascript:deluploadimg("<%=dr["ID"] %>","contr_<%=dr["ID"] %>","up_max_image_id")'>删除</a>
                                        </div>
                                    </div>
                                    <%}
                                   } %>
                                    <input type="hidden" id="up_max_image_id" name="up_max_image_id" value="<%= strFileIDs%>" />
                                    <input type="hidden" id="up_max_image_name" name="up_max_image_name" value="<%= strFileNames%>" />
                                    <%} %>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div class="frmbottom">
            <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click" OnClientClick="return CheckValid();"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
            <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
                <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
        </div>
    </form>
</body>
</html>
