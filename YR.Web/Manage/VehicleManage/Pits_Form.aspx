<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pits_Form.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.Pits_Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>维修站信息表单</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/Validator/JValidator.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/artDialog.source.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/artDialog/iframeTools.source.js" type="text/javascript"></script>
    <link href="/Themes/Scripts/asyncbox/skins/default.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/asyncbox/asyncbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/ImageUpload.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化
        $(function () {
            $('#table2').hide();
            $('#table3').hide();
            $('#table4').hide();

            $("#btnUpMaxImage").click(function () {
                CommonData('up_max_image_id', '1', 'divMaxImage_msg', 3, 2, '<%=Request["key"] %>');
            });
            $("#btnUpMinImage").click(function () {
                CommonData('up_min_image_id', '1', 'divMinImage_msg', 3, 1, '<%=Request["key"] %>');
            });
        })

        function panel(obj) {
            if (obj == 1) {
                $('#table1').show();
                $('#table2').hide();
                $('#table3').hide();
                $('#table4').hide();
            } else if (obj == 2) {
                $('#table1').hide();
                $("#table2").show();
                $('#table3').hide();
                $('#table4').hide();
            } else if (obj == 3) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').show();
                $('#table4').hide();
            } else if (obj == 4) {
                $('#table1').hide();
                $("#table2").hide();
                $('#table3').hide();
                $('#table4').show();
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
                <td id="menutab" style="vertical-align: bottom;">
                    <div id="tab0" class="Tabsel" onclick="GetTabClick(this);panel(1)">
                        基本信息</div>
                    <div id="tab1" runat="server" class="Tabremovesel" onclick="GetTabClick(this);panel(2);">
                        维修人员</div>
                    <div id="tab2" runat="server" visible="false" class="Tabremovesel" onclick="GetTabClick(this);panel(3);">
                        缩略图</div>
                    <div id="tab3" runat="server" visible="false" class="Tabremovesel" onclick="GetTabClick(this);panel(4);">
                        详细图</div>
                </td>
            </tr>
        </table>
    </div>
    <div class="div-frm" style="height: 250px;">
        <table id="table1" border="0" cellpadding="0" cellspacing="0" class="frm">
            <tr>
                <th>
                    维修站名称:
                </th>
                <td>
                    <input id="Name" runat="server" type="text" class="txt" datacol="yes" err="停车场名称"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    维修站地址:
                </th>
                <td>
                    <input id="Address" runat="server" type="text" class="txt" datacol="yes" err="停车场地址"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    固定电话:
                </th>
                <td>
                    <input id="Telephone" runat="server" type="text" class="txt" datacol="yes" err="固定电话"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    移动电话:
                </th>
                <td>
                    <input id="Mobile" runat="server" type="text" class="txt" datacol="yes" err="移动电话"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    经度:
                </th>
                <td>
                    <input id="Longitude" runat="server" type="text" class="txt" datacol="yes" err="经度"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
                <th>
                    纬度:
                </th>
                <td>
                    <input id="Latitude" runat="server" type="text" class="txt" datacol="yes" err="纬度"
                        checkexpession="NotNull" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <th>
                    维修站描述:
                </th>
                <td colspan="3">
                    <textarea id="Description" class="txtRemark" runat="server" style="width: 552px;
                        height: 100px;"></textarea>
                </td>
            </tr>
        </table>
        <%--维修人员--%>
        <div id="table2">
            <div class="btnbartitle">
                <div>
                    组织机构
                </div>
            </div>
            <div class="div-body" style="height: 245px;">
                <ul class="strTree">
                    <%=strUserHtml.ToString()%>
                </ul>
            </div>
        </div>
        <%--车辆缩略图片--%>
        <div id="table3">
            <div class="div-body" style="height: 295px;">
                <table>
                    <tr>
                        <td>
                            缩略图
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
                                <% if (PitsImageTable != null)
                                   {
                                       string strFileIDs = string.Empty;
                                       string strFileNames = string.Empty;
                                %>
                                <% foreach (System.Data.DataRow dr in PitsImageTable.Rows)
                                   {
                                       if (dr["Type"].ToString() == "1")
                                       {
                                           strFileIDs += "," + dr["ID"];
                                           strFileNames += "," + dr["ImageName"];%>
                                <div id="contr_<%=dr["ID"] %>" class="imagekkk">
                                    <div class="imagekkkk">
                                        <a href="<%=dr["ImageUrl"] %>" target="_blank">
                                            <img src='<%=dr["ImageUrl"]%>' height="100px" width="100px" /></a> <a class="delimg"
                                                href='javascript:deluploadimg("<%=dr["ID"] %>","contr_<%=dr["ID"] %>","up_min_image_id")'>
                                                删除</a>
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
        <div id="table4">
            <div class="div-body" style="height: 295px;">
                <table>
                    <tr>
                        <td>
                            详细图
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
                                <% if (PitsImageTable != null)
                                   {
                                       string strFileIDs = string.Empty;
                                       string strFileNames = string.Empty;
                                %>
                                <% foreach (System.Data.DataRow dr in PitsImageTable.Rows)
                                   {
                                       if (dr["Type"].ToString() == "2")
                                       {
                                           strFileIDs += "," + dr["ID"];
                                           strFileNames += "," + dr["ImageName"];%>
                                <div id="contr_<%=dr[0] %>" class="imagekkk">
                                    <div class="imagekkkk">
                                        <a href="<%=dr["ImageUrl"] %>" target="_blank">
                                            <img src='<%=dr["ImageUrl"]%>' height="100px" width="100px" /></a> <a class="delimg"
                                                href='javascript:deluploadimg("<%=dr["ID"] %>","contr_<%=dr["ID"] %>","up_max_image_id")'>
                                                删除</a>
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
        <asp:LinkButton ID="Save" runat="server" class="l-btn" OnClick="Save_Click"><span class="l-btn-left">
            <img src="/Themes/Images/disk.png" alt="" />保 存</span></asp:LinkButton>
        <a class="l-btn" href="javascript:void(0)" onclick="OpenClose();"><span class="l-btn-left">
            <img src="/Themes/Images/cancel.png" alt="" />关 闭</span></a>
    </div>
    </form>
</body>
</html>
