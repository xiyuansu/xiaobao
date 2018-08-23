<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleMap.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleMap" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />  
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车辆监控</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../../Themes/Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="http://webapi.amap.com/maps?v=1.3&key=<%=ViewState["MapKey"]%>&plugin=AMap.ToolBar,AMap.Scale,AMap.Geolocation,AMap.MapType"></script>
    <link href="../../Themes/Scripts/artDialog/skins/blue.css" rel="stylesheet" />
    <script src="../../Themes/Scripts/artDialog/jquery.artDialog.js"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin:0px;
            padding:0px;
            font-size:12px;
        }

        .main
        {
            width:100%;
            height:100%;
        }

        #map
        {
            height:100%;
            padding-left:251px;
        }

        .text
        {
            height: 16px;
            padding: 4px 3px;
            border: 1px solid #bbb;
            font-size: 14px;
            vertical-align: middle;
        }

        .btn {
            height: 30px;
            display: inline-block;
            background-color:#0066cc;
            color:#ffffff;
            border-width: 0px;
            padding: 10px;
            margin-top: 10px;
            margin-bottom: 10px;
            margin-left:5px;
            border-radius:5px;
            float:none;
            vertical-align:middle;
        }

        .btn:hover
        {
            background-color:#003399;
        }

    </style>

    <script type="text/javascript">
        var map;
        var vehicleID = "<%=ViewState["VehicleID"]%>";
        var cityID = "<%=ViewState["CityID"]%>";
        $(function () {
            //$(".main").height($(window).height());
            initMap();
        });

        function initMap() {
            map = new AMap.Map("map", {
                resizeEnable: true,
                zoom: 11,
            });
            
            map.addControl(new AMap.ToolBar());
            map.addControl(new AMap.Scale());
            map.addControl(new AMap.Geolocation());
            map.addControl(new AMap.MapType());
            loadVehicleInfo();
        }

        function loadVehicleInfo()
        {
            //map.clearMap();
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetVehicleByID", VehicleID: vehicleID, Latitude: 0,Longitude:0},
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    Loading(false);
                    alert(textStatus);
                },
                success: function (resp) {
                    //Loading(false);
                    if (resp.state == "success") {
                            var vehicleInfo=resp.content.VehicleInfo;
                            var latlng = new AMap.LngLat(vehicleInfo.LONGITUDE, vehicleInfo.LATITUDE);
                            var myIcon = new AMap.Icon({
                                image: "/Themes/Images/marker_vehicle_icon_press.png",
                                imageSize: new AMap.Size(25, 32),
                                size: new AMap.Size(25, 32),
                                imageOffset: new AMap.Pixel(0, 0)
                            });
                            var marker = new AMap.Marker({
                                icon: myIcon,
                                position: latlng,
                                map: map
                            });
                            marker.setzIndex(9999);
                            map.setZoom(15);
                            map.panTo(latlng);
                        };
                    }
            });
        }

        var serviceArea_list = new Array();
        function loadServiceArea() {
            serviceArea_list.length = 0;
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetServiceAreaByID", city: cityID },
                error: function (XMLHttpRequest, textStatus, errorThrown) {

                },
                success: function (resp) {
                    if (resp.Code == "0") {
                        serviceArea_list.length = 0;
                        $.each(resp.Data, function (idx, item) {
                            var pts = item.COORDINATES.split(";");
                            item.points = new Array();
                            for (var i = 0; i < pts.length; i++) {
                                var latlng = new AMap.LngLat(pts[i].split(",")[0], pts[i].split(",")[1]);
                                item.points.push(latlng);
                            }
                            serviceArea_list.push(item);
                        });
                        drawServiceArea(serviceArea_list);
                    }
                }
            });
        }

        var serviceAreasOverlay = new Array();
        function drawServiceArea(arealist) {
            map.remove(serviceAreasOverlay);
            serviceAreasOverlay.length = 0;
            $.each(arealist, function (idx, item) {
                var serviceAreaOverlay = new AMap.Polygon({
                    map: map,
                    path: item.points,
                    strokeColor: "#ff0000",
                    strokeOpacity: 1,
                    strokeWeight: 2,
                    fillColor: "#ffffff",
                    fillOpacity: 0,
                    strokeStyle: "solid"
                });
            });
        }

        var Toast = function (config) {
            this.context = config.context == null ? $('body') : config.context;//上下文
            this.message = config.message;//显示内容
            this.time = config.time == null ? 5000 : config.time;//持续时间
            this.left = config.left;//距容器左边的距离
            this.top = config.top;//距容器上方的距离
            this.init();
        }
        var msgEntity;
        Toast.prototype = {
            //初始化显示的位置内容等
            init: function () {
                $("#toastMessage").remove();
                //设置消息体
                var msgDIV = new Array();
                msgDIV.push('<div id="toastMessage" style=\'border-radius:10px;\'>');
                msgDIV.push('<span>' + this.message + '</span>');
                msgDIV.push('</div>');
                msgEntity = $(msgDIV.join('')).appendTo(this.context);
                //设置消息样式
                var left = this.left == null ? this.context.width() / 2 - msgEntity.find('span').width() / 2 : this.left;
                var vertical = (document.body.clientHeight - msgEntity.height()) / 2;
                vertical = document.body.clientHeight - msgEntity.height() - 200;
                var top = this.top == null ? vertical : this.top;
                msgEntity.css({ position: 'absolute', top: top, 'z-index': '999', left: left, 'background-color': 'black', color: 'white', 'font-size': '18px', padding: '10px', margin: '10px' });
                msgEntity.hide();
            },
            //显示动画
            show: function () {
                msgEntity.fadeIn(this.time / 2);
                msgEntity.fadeOut(this.time / 2);
            }

        }

        function Loading(visible)
        {
            if (visible) {
                $.dialog({ id: 'dlg_loading', title:false,content: document.getElementById('dlg_loading'), lock: true,cancel:false });
            }
            else {
                $.dialog.get('dlg_loading').close();
            }
            
        }

    </script>

</head>
<body>
    <div class="main">
        <div id="map"></div> 
    </div>

    <div id="dlg_loading" style="display:none;">
        <img src="../../Themes/Images/loading.gif" />
        <span style="font-size:18px;font-weight:bold;color:#003399">数据正在加载中......</span>
    </div>

</body>
</html>
