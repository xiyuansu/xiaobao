<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleMonitor.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleMonitor" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />  
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车辆监控系统</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="http://webapi.amap.com/maps?v=1.3&key=<%=ViewState["MapKey"]%>&plugin=AMap.ToolBar,AMap.Scale,AMap.Geolocation,AMap.MapType,AMap.Geocoder,AMap.MarkerClusterer"></script>
    <link href="/Themes/Scripts/artDialog/skins/blue.css" rel="stylesheet" />
    <script src="/Themes/Scripts/artDialog/jquery.artDialog.js"></script>
    <script src="/Themes/Scripts/FileSaver.js"></script>
    <script src="/Themes/Scripts/layer/layer.js"></script>
    <style type="text/css">
        body{margin:0;padding:0;font-size:12px}
        .main{width:100%;height:100%}
        #map{width:100%;height:100%}
        #summary_ul{width:auto;margin:0 auto;padding:10px;text-align:center;overflow:auto}
        #summary_ul li{list-style:none;float:left;width:25%;color:#fff}
        #summary_ul li img{width:30px;height:30px}
        .amap-maptypecontrol{top:70px}
        .button-group{position:absolute;top:80px;right:20px;font-size:12px;padding:10px;z-index:100}
        .button-group .ruler{height:28px;line-height:28px;background-color:#0D9BF2;color:#FFF;border:0;outline:0;padding-left:5px;padding-right:5px;border-radius:3px;margin-bottom:4px;cursor:pointer}
    </style>
</head>
<body>
    <div class="main">
        <div id="map"></div> 
    </div>
    <div id="summary_div" style="margin:0 auto;width:100%;position:absolute;top:0px;background-color:#808080;opacity:0.8;text-align:center;overflow:auto;">
        <ul id="summary_ul">
            <li id="summary_ul_all">
                <img src="../../Themes/Images/marker_vehicle_icon_press.png"/>
                <br />
                运营数量:<span id="num_total" style="color:red;">0</span></li>
            <li id="summary_ul_online">
                <img src="../../Themes/Images/marker_vehicle_online.png"/>
                <br />
                在线数量:<span id="num_online" style="color:red;">0</span></li>
            <li id="summary_ul_offline">
                <img src="../../Themes/Images/marker_vehicle_offline.png"/>
                <br />
                离线数量:<span id="num_offline" style="color:red;">0</span></li>
            <li id="summary_ul_lowpower">
                <img src="../../Themes/Images/marker_vehicle_lowpower.png"/>
                <br />
                低电数量:<span id="num_lowpower" style="color:red;">0</span></li>
        </ul>
    </div>

    <div id="dlg_vehicle_orders" style="display:none;">
        <table id="order_list" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 120px; text-align: center;">订单编号</td>
                    <td style="width: 80px; text-align: center;">会员手机号</td>
                    <td style="width: 60px; text-align: center;">会员姓名</td>
                    <td style="width: 100px;text-align: center;">车辆</td>
                    <td style="width: 46px;text-align: center;">公里价格</td>
                    <td style="width: 46px;text-align: center;">分钟价格</td>
                    <td style="width: 60px; text-align: center;">公里数</td>
                    <td style="width: 60px; text-align: center;">分钟数</td>
                    <td style="width: 80px; text-align: center;">总金额</td>
                    <td style="width: 80px; text-align: center;">结算金额</td>
                    <td style="width: 60px; text-align: center;">订单状态</td>
                    <td style="width: 60px; text-align: center;">支付状态</td>
                    <td style="width: 120px; text-align: center;">开始时间</td>
                    <td style="width: 120px; text-align: center;">结束时间</td>
                    <td style="width: 60px; text-align: center;">操作</td>
                </tr>
            </thead>
            <tbody>
                
            </tbody>
        </table>
    </div>

    <div id="map_menu_findcar" style="display:none;">
        <div>
            <label>车辆名称</label>
            <input type="text" id="txt_vehiclename" placeholder="车辆名称"/>
        </div>
    </div>

    <script type="text/javascript">
        var map,ruler;
        var contextMenu;
        var citycode = "0797";
        var vehiclesOverlay = new Array();
        var vehicle_online_list = new Array();
        var vehicle_offline_list = new Array();
        var vehicle_lowpower_list = new Array();
        $(function () {
            initMap();
            $("#summary_ul_all").click(function () {
                exportVehicleList("运营车辆");
            });
            $("#summary_ul_online").click(function () {
                exportVehicleList("在线车辆");
            });
            $("#summary_ul_offline").click(function () {
                exportVehicleList("离线车辆");
            });
            $("#summary_ul_lowpower").click(function () {
                exportVehicleList("低电车辆");
            });
            setInterval("loadVehicles()", 180000);
        });

        function initMap() {
            map = new AMap.Map("map", {
                resizeEnable: true,
                zoom: 12,
                center: [117.297766,34.799223]
            });
            map.setMapStyle('amap://styles/light');
            map.plugin(["AMap.RangingTool"], function () {
                ruler = new AMap.RangingTool(map);
            });
            loadServiceArea(citycode);
            loadParkingList(citycode);
            loadForbidList(citycode);
            map.addControl(new AMap.ToolBar({ offset: new AMap.Pixel(10, 70) }));
            /*map.on("zoomchange", function () {
                if (map.getZoom() <= 16 && map.getZoom() >= 10) {
                    map.getCity(function (result) {
                        if (citycode != result.citycode || serviceAreasOverlay.length == 0) {
                            citycode = result.citycode;
                            loadServiceArea(citycode);
                        }
                    });
                }
                else {
                    //if (serviceAreasOverlay.length == 0) {
                        //map.remove(serviceAreasOverlay);
                        //serviceAreaOverlay.length = 0;
                    //}
                    citycode = "";
                }
                map.getCity(function (result) {
                    if (citycode != result.citycode) {
                        citycode = result.citycode;
                        loadParkingList(citycode);
                    }
                });
            });*/
            contextMenu = new AMap.ContextMenu();
            contextMenu.addItem("查找车辆", function () {
                $.dialog({
                    title: "查找车辆", content: document.getElementById('map_menu_findcar'), lock: true,
                    okValue: "确定",
                    ok: function () {
                        var name = $("#txt_vehiclename").val();
                        if (name.length == 0) {
                            layer.msg('车辆名称未填写');
                            return false;
                        }
                        var vehicle = null;
                        $(vehicle_list).each(function (idx, item) {
                            if (item.LICENSENUMBER === name || item.NAME === name) {
                                vehicle = item;
                            }
                        });
                        if (vehicle != null) {
                            var point = new AMap.LngLat(vehicle.LONGITUDE, vehicle.LATITUDE);
                            map.setZoomAndCenter(20, point);
                            map.panTo(point);
                            showMapInfoWindow(vehicle);
                        }
                        else {
                            layer.msg('未找到指定车辆');
                        }
                    }
                });
            }, 0);
            contextMenu.addItem("刷新数据", function () {
                loadVehicles();
            }, 0);
            map.on('rightclick', function (e) {
                contextMenu.open(map, e.lnglat);
                contextMenuPositon = e.lnglat;
            });
           
            map.on("complete", function () {
                 /*if (typeof (Storage) !== "undefined") {
                    if (localStorage.map_bound != undefined) {
                        var southWest, northEast;
                        var pt1 = localStorage.map_bound.split(";")[0];
                        var pt2 = localStorage.map_bound.split(";")[1];
                        southWest = new AMap.LngLat(pt1.split(",")[0], pt1.split(",")[1]);
                        northEast = new AMap.LngLat(pt2.split(",")[0], pt2.split(",")[1]);
                        var bounds = new AMap.Bounds(southWest, northEast);
                        map.setBounds(bounds);
                    }
                }*/
                map.getCity(function (result) {
                    citycode = result.citycode;
                    //loadServiceArea(citycode);
                    loadParkingList(citycode);
                    loadForbidList(citycode);
                });
            });
            map.on("mousemove", function () {
                if (typeof (Storage) !== "undefined") {
                    localStorage.map_bound = map.getBounds();
                }
            });
            map.on("zoomend", function () {
                if (typeof (Storage) !== "undefined") {
                    localStorage.map_bound = map.getBounds();
                }
            });
            loadVehicles();
        }

        var vehicle_list = new Array();
        function loadVehicles() {
            var vstate = "1";
            vehicle_list.length = 0;
            //layer.load(2);
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetVehicles", vstate: vstate, page_index: 0, page_size: 9999 },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //layer.closeAll('loading');
                },
                success: function (resp) {
                    //layer.closeAll('loading');
                    if (resp.Code == "0") {
                        var num_online = 0, num_offline = 0, sum_count = 0;
                        $(eval(resp.Data)).each(function (idx, item) {
                            item.marker = null;
                            vehicle_list.push(item);
                            //layer.msg('加载数据成功');
                        });
                        drawVehicles();
                    } else {
                        layer.msg("加载车辆数据失败:" + resp.Message);
                    }
                }
            });
        }

        var serviceArea_list = new Array();
        function loadServiceArea(city) {
            serviceArea_list.length = 0;
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetServiceArea", city: citycode },
                error: function (XMLHttpRequest, textStatus, errorThrown) {},
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

        var parking_list = new Array();
        function loadParkingList(city) {
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetParkingList", city: citycode },
                error: function (XMLHttpRequest, textStatus, errorThrown) {},
                success: function (resp) {
                    if (resp.Code == "0") {
                        parking_list.length = 0;
                        $.each(resp.Data, function (idx, item) {
                            var pts = item.COORDINATES.split(";");
                            item.points = new Array();
                            for (var i = 0; i < pts.length; i++) {
                                var latlng = new AMap.LngLat(pts[i].split(",")[0], pts[i].split(",")[1]);
                                item.points.push(latlng);
                            }
                            parking_list.push(item);
                        });
                        drawParkings(parking_list);
                    }
                }
            });
        }

        var forbid_list = new Array();
        function loadForbidList(city) {
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetForbidList", city: citycode },
                error: function (XMLHttpRequest, textStatus, errorThrown) { },
                success: function (resp) {
                    if (resp.Code == "0") {
                        forbid_list.length = 0;
                        $.each(resp.Data, function (idx, item) {
                            var pts = item.COORDINATES.split(";");
                            item.points = new Array();
                            for (var i = 0; i < pts.length; i++) {
                                var latlng = new AMap.LngLat(pts[i].split(",")[0], pts[i].split(",")[1]);
                                item.points.push(latlng);
                            }
                            forbid_list.push(item);
                        });
                        drawForbid(forbid_list);
                    }
                }
            });
        }

        function showMapInfoWindow(data) {
            var lnglatXY = [116.396574, 39.992706];
            var geocoder = new AMap.Geocoder({
                radius: 1000,
                extensions: "all"
            });
            geocoder.getAddress(new AMap.LngLat(data.LONGITUDE, data.LATITUDE), function (status, result) {
                if (status === 'complete' && result.info === 'OK') {
                    data.ADDRESS = result.regeocode.formattedAddress;
                    var infoHtml = "<br/>名称：" + data.NAME;
                    infoHtml += "<br/>车牌号：" + data.LICENSENUMBER;
                    infoHtml += "<br/>盒子号：" + data.VEHICLEGPSNUM;
                    infoHtml += "<br/>地址：" + data.ADDRESS;
                    infoHtml += "<br/>电量：" + data.ELECTRICITY;
                    infoHtml += "<br/>总里程：" + data.MILEAGE + "公里";
                    infoHtml += "<br/>当天里程：" + data.TODAYMILEAGE + "公里";
                    if (data.DIFFMINUTES > 10)
                        infoHtml += "<br/>离线时间：" + data.LASTUPDATETIME;
                    infoHtml += "<br/>行驶轨迹：<a href=\"#\" onclick=\"showVehicleTrace('" + data.ID + "','')\">查看</a>";
                    infoHtml += "<br/>客户订单：<a href=\"#\" onclick=\"showVehicleOrders('" + data.ID + "')\">查看</a>";
                    var infoWindow = new AMap.InfoWindow({ content: infoHtml, offset: new AMap.Pixel(0, -15) });
                    infoWindow.open(map, new AMap.LngLat(data.LONGITUDE, data.LATITUDE));
                }
            });
        }

        function drawVehicles() {
            map.remove(vehiclesOverlay);
            vehiclesOverlay.length = 0;
            vehicle_online_list.length = 0;
            vehicle_offline_list.length = 0;
            vehicle_lowpower_list.length = 0;
            for (var i = 0; i < vehicle_list.length; i++) {
                var item = vehicle_list[i];
                var point = new AMap.LngLat(item.LONGITUDE, item.LATITUDE);
                var icon_path = "/Themes/Images/marker_vehicle_icon_press.png";
                if (item.DIFFMINUTES > 10) {
                    vehicle_offline_list.push(item);
                    icon_path = "/Themes/Images/marker_vehicle_offline.png";
                }
                else {
                    vehicle_online_list.push(item);
                    icon_path = "/Themes/Images/marker_vehicle_online.png";
                }
                if (parseFloat(item.ELECTRICITY) < 30) {
                    vehicle_lowpower_list.push(item);
                    icon_path = "/Themes/Images/marker_vehicle_lowpower.png";
                }

                var myIcon = new AMap.Icon({
                    image: icon_path,
                    imageSize: new AMap.Size(25, 32),
                    size: new AMap.Size(25, 32),
                    imageOffset: new AMap.Pixel(0, 0)
                });
                var marker = new AMap.Marker({
                    icon: myIcon,
                    position: point,
                    map: map
                });
                item.marker = marker;
                vehiclesOverlay.push(marker);
                (function (arg) {
                    marker.on("click", function () {
                        showMapInfoWindow(arg);
                    });
                })(item);
            }
            $("#num_total").text((vehicle_list.length));
            $("#num_online").text((vehicle_online_list.length));
            $("#num_offline").text(vehicle_offline_list.length);
            $("#num_lowpower").text(vehicle_lowpower_list.length);
        }

        var serviceAreasOverlay = new Array();
        function drawServiceArea(arealist) {
            if (arealist.length > 0) {
                map.remove(serviceAreasOverlay);
                serviceAreasOverlay.length = 0;
                $.each(arealist, function (idx, item) {
                    var serviceAreaOverlay = new AMap.Polygon({
                        map: map,
                        path: item.points,
                        strokeColor: "#ff0000",
                        strokeOpacity: 1,
                        strokeWeight: 2,
                        fillColor: "#ff0000",
                        fillOpacity: 0.1,
                        strokeStyle: "solid"
                    });
                    serviceAreaOverlay.on('rightclick', function (e) {
                        contextMenu.open(map, e.lnglat);
                        contextMenuPositon = e.lnglat;
                    });
                    serviceAreasOverlay.push(serviceAreaOverlay);
                });
            }
        }

        var parkingsOverlay = new Array();
        function drawParkings(parkings) {
            map.remove(parkingsOverlay);
            parkingsOverlay.length = 0;
            $.each(parkings, function (idx, item) {
                var parkingOverlay = new AMap.Polygon({
                    map: map,
                    path: item.points,
                    strokeColor: "#ff0000",
                    strokeOpacity: 1,
                    strokeWeight: 2,
                    fillColor: "#ff0000",
                    fillOpacity: 0.3,
                    strokeStyle: "dashed"
                });
                parkingOverlay.on('rightclick', function (e) {
                    contextMenu.open(map, e.lnglat);
                    contextMenuPositon = e.lnglat;
                });
                parkingsOverlay.push(parkingOverlay);
            });
        }

        var forbidsOverlay = new Array();
        function drawForbid(parkings) {
            map.remove(forbidsOverlay);
            forbidsOverlay.length = 0;
            $.each(parkings, function (idx, item) {
                var forbidOverlay = new AMap.Polygon({
                    map: map,
                    path: item.points,
                    strokeColor: "#000000",
                    strokeOpacity: 1,
                    strokeWeight: 2,
                    fillColor: "#778899",
                    fillOpacity: 0.3,
                    strokeStyle: "dashed"
                });
                forbidOverlay.on('rightclick', function (e) {
                    contextMenu.open(map, e.lnglat);
                    contextMenuPositon = e.lnglat;
                });
                forbidsOverlay.push(forbidOverlay);
            });
        }

        function showVehicleTrace(vehicleid, orderid) {
            var url = "/Manage/VehicleManage/VehicleTrace.html?vid=" + vehicleid + "&oid=" + orderid;
            window.open(url);
        }

        function showVehicleOrders(vehicleid) {
            var now = new Date();
            var begin = new Date();
            var end = new Date();
            begin = now.getFullYear() + "-" + (now.getMonth() + 1) + "-" + now.getDate() + " 00:00:00";
            end = now.getFullYear() + "-" + (now.getMonth() + 1) + "-" + now.getDate() + " 23:59:59";
            $("#order_list>tbody").html("");
            //layer.load(2);
            $.ajax({
                url: "/api/app/VehicleController.ashx",
                type: "POST",
                dataType: "json",
                data: { method: "GetOrdersByVehicle", VehicleID: vehicleid, begin: begin, end: end, page_index: 0, page_size: 9999 },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //layer.closeAll("loading");
                },
                success: function (resp2) {
                    //layer.closeAll("loading");
                    if (resp2.Code == "0") {
                        if (resp2.Data == null) {
                            layer.msg("未找到订单数据,请选择其它车辆查看 " + resp2.Message);
                            return;
                        }
                        $(eval(resp2.Data)).each(function (idx2, item2) {
                            var orderstate_str = "";
                            var paystate_str = "";
                            switch (item2.ORDERSTATE) {
                                case 0:
                                    orderstate_str = "无效";
                                    break;
                                case 1:
                                    orderstate_str = "有效";
                                    break;
                                case 2:
                                    orderstate_str = "已完成";
                                    break;
                                case 3:
                                    orderstate_str = "异常订单";
                                    break;
                                default:
                                    break;
                            }
                            switch (item2.PAYSTATE) {
                                case 0:
                                    paystate_str = "未支付";
                                    break;
                                case 1:
                                    paystate_str = "已支付";
                                    break;
                                case 2:
                                    paystate_str = "余额不足";
                                    break;
                                default:
                                    break;
                            }
                            var row = "";
                            row += "<tr>";
                            row += "<td style=\"width: 170px; text-align: center;\">" + item2.ORDERNUM + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + item2.BINDPHONE + "</td>";
                            row += "<td style=\"width: 50px; text-align: center;\">" + item2.REALNAME + "</td>";
                            row += "<td style=\"width: 150px; text-align: center;\">" + item2.VEHICLENAME + "</td>";
                            row += "<td style=\"width: 40px; text-align: center;\">" + item2.KMPRICE + "</td>";
                            row += "<td style=\"width: 40px; text-align: center;\">" + item2.MINUTEPRICE + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + item2.MILEAGE + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + item2.MINUTES + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + item2.TOTALMONEY + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + item2.SETTLEMENTMONEY + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + orderstate_str + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\">" + paystate_str + "</td>";
                            row += "<td style=\"width: 100px; text-align: center;\">" + item2.CREATETIME + "</td>";
                            row += "<td style=\"width: 100px; text-align: center;\">" + item2.FINISHEDTIME + "</td>";
                            row += "<td style=\"width: 60px; text-align: center;\"><a href=\"#\" onclick=\"showVehicleTrace('" + item2.VEHICLEID + "','" + item2.ID + "','','')\">轨迹</a></td>";
                            row += "</tr>";
                            $(row).appendTo($("#order_list>tbody")).data("odata", item2);
                        });
                        $.dialog({ title: "客户订单", content: document.getElementById('dlg_vehicle_orders'), lock: true, padding: 0 });
                    }
                    else {
                        lay.message("加载订单数据失败:" + resp2.Message);
                    }
                }
            });
        }

        function exportVehicleList(type) {
            var list = new Array();
            if (type == "运营车辆") {
                if (vehicle_list.length > 0) {
                    list = vehicle_list;
                } else {
                    return false;
                }
            }

            if (type == "在线车辆") {
                if (vehicle_online_list.length > 0) {
                    list = vehicle_online_list;
                } else {
                    return false;
                }
            }

            if (type == "离线车辆") {
                if (vehicle_offline_list.length > 0) {
                    list = vehicle_offline_list;
                } else {
                    return false;
                }
            }

            if (type == "低电车辆") {
                if (vehicle_lowpower_list.length > 0) {
                    list = vehicle_lowpower_list;
                } else {
                    return false;
                }
            }
            var dt = new Date();
            var dtstr = dt.getFullYear() + "" + (dt.getMonth() + 1) + "" + dt.getDate() + "" + dt.getHours() + "" + dt.getMinutes() + "" + dt.getSeconds();
            var fname = type + "列表_" + dtstr + ".csv";
            var csv = "";
            var columns = "\uFEFF车辆名称,盒子号\n";
            if (type == "离线车辆") {
                columns = "\uFEFF车辆名称,盒子号,离线时间\n";
                $.each(list, function (idx, item) {
                    csv += item.NAME;
                    csv += ",'" + item.VEHICLEGPSNUM;
                    csv += ",'" + item.LASTUPDATETIME;
                    csv += "\n";
                });
            }
            else {
                columns = "\uFEFF车辆名称,盒子号\n";
                $.each(list, function (idx, item) {
                    csv += item.NAME;
                    csv += ",'" + item.VEHICLEGPSNUM;
                    csv += "\n";
                });
            }
            var blob = new Blob([columns + csv], { type: "text/plain;charset=utf-8" });
            saveAs(blob, fname);
        }
        function startRuler() {
            ruler.turnOn();
        }
    </script>
</body>
</html>
