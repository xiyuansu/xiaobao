<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleHistory_List.aspx.cs" Inherits="YR.Web.Manage.UserManage.VehicleHistory_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>行车历史管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="/Themes/Styles/select2.min.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/select2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () { })
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        //锁 定
        function lock() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var parm = 'action=lock&user_ID=' + key;
                showConfirmMsg('注：您确认要【锁 定】当前选中用户吗？', function (r) {
                    if (r) {
                        getAjax('UserInfoValidate.ashx', parm, function (rs) {
                            if (parseInt(rs) > 0) {
                                showTipsMsg("锁定成功！", 2000, 4);
                            }
                            else {
                                showTipsMsg("<span style='color:red'>锁定失败，请稍后重试！</span>", 4000, 5);
                            }
                        });
                    }
                });
            }
        }
        var data = [{ id: 0, text: 'enhancement' }, { id: 1, text: 'bug' }, { id: 2, text: 'duplicate' }, { id: 3, text: 'invalid' }, { id: 4, text: 'wontfix' }];
 
        $(".js-example-data-array-selected").select2({
          data: data
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2"> 
        <ContentTemplate>
    <div class="btnbartitle">
        <div>
            车辆操控记录
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
        
        <div>
            <table class="tabCondition">
                <tr>
                    <th>车辆
                    </th>
                    <td>
                        <input type="text" id="txtVehicleName" runat="server" placeholder="车辆名称" tabindex="1" autocomplete="off" style="width: 70px;" />
                    </td>
                    <th>操作类型</th>
                    <td>
                        <select id="selDriveStyle" class="Searchwhere2" runat="server" style="width: 122px;">
                            <option value="-1">全部</option>
                            <option value="01">开车</option>
                            <option value="02">锁车</option>
                            <option value="03">寻车</option>
                            <option value="04">开座锁</option>
                        </select>
                    </td>
                    <th>操作结果</th>
                    <td>
                        <select id="selOprResult" class="Searchwhere2" runat="server" style="width: 122px;">
                            <option value="-1">全部</option>
                            <option value="1">成功</option>
                            <option value="0">失败</option>
                        </select>
                    </td>
                    <th>操控时间
                    </th>
                    <td>
                        <input id="txtStartDrivingTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndDrivingTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 70px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="8">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click">
                            <span class="icon-botton" style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"></span>查 询
                        </asp:LinkButton>
                        <asp:LinkButton ID="lbtInit" runat="server" class="button green" OnClick="lbtInit_Click">
                            <span class="icon-botton"></span>重 置
                        </asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width:3%; text-align: center;">选择</td>
                    <td style="width: 6%; text-align: center;"> 车辆名称</td>
                    <td style="width: 10%; text-align: center;">盒子号</td>
                    <td style="width: 5%; text-align: center;">速度</td>
                    <td style="width: 5%; text-align: center;">电量</td>
                    <td style="width: 10%;text-align: center;"> 经度</td>
                    <td style="width: 10%;text-align: center;"> 纬度</td>
                    <td style="width: 4%;text-align: center;">操控</td>
                    <td style="width: 4%;text-align: center;"> 结果</td>
                    <td style="width: 28%;text-align: center;">返回结果</td>
                    <td style="width: 10%; text-align: center;"> 行车时间</td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("LicenseNumber")%></a>
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("GPSNum")%></a>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Speed")%>
                            </td>
                            <td style="text-align: center;">
                                <%#GetPower(Eval("Power").ToString())%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Longitude")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("Latitude")%>
                            </td>
                            <td style="text-align: center;">
                                <%#GetDriveStyle(Eval("DriveStyle").ToString())%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("OprResult").ToString()=="1"?"成功":"失败"%>
                            </td>
                            <td style="text-align: left;">
                                <%#Eval("ReturnResult")%>
                            </td>
                            <td style="text-align: center;">
                                <%#Eval("RecordTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='9' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
                </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
