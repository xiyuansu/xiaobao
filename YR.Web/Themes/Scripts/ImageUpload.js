/**
* hiddenid:隐藏域id,UPType:类型，msgid：提示消息ID ,method:回调方法，
* @auth:xzhang
* @date:2013-12-14
**/
var CommonData = function (hiddenid, UPType, msgid, category,type,relationid) {

    asyncbox.open({
        id: 'showid',
        title: "上传图片",
        url: "/upfile/upstart.aspx",
        args: 'hiddenid=' + hiddenid + "&UPType=" + UPType + "&msgid=" + msgid + "&category=" + category + "&type=" + type + "&relationid=" + relationid + "&identity=" + Math.random(1),
        width: 500,
        height: 130,
        modal: true,
        callback: function (btnRes, cntWin, returnValue) {
            //判断 btnRes 值
            if (btnRes == 'close') {
                //alert(returnValue);
            }
        }
    });

}

function deluploadimg(id, div_id, hiddenid) {
    $.ajax({
        url: "/upfile/file.ashx",
        data: "action=delfile&fileid=" + id,
        success: function (data) {
            var msg = data.split(',');
            if (msg[0] == "1") {
                alert("删除成功!");
                document.getElementById(div_id).innerHTML = "";
                document.getElementById(hiddenid).value = "";
                //                $(".menu li").not(".group").append("<span></span>");
                $("#" + div_id).remove();
            } else {
                alert("删除失败!");
            }
        },
        error: function () { alert("网路错误,稍后重试..."); }
    });
}
function deluploadimgnew(id,name, div_id, hiddenid,hiddenname) {
    $.ajax({
        url: "/upfile/file.ashx",
        data: "action=delfile&fileid=" + id,
        success: function (data) {
            var msg = data.split(',');
            if (msg[0] == "1") {
                alert("删除成功!");
                document.getElementById(div_id).innerHTML = "";
                var fileID = document.getElementById(hiddenid).value;
                var fileName = document.getElementById(hiddenname).value;

                document.getElementById(hiddenid).value = fileID.replace("," + id, '');
                document.getElementById(hiddenname).value = fileName.replace("," + name, '');
                $(".menu li").not(".group").append("<span></span>");
            } else {
                alert("删除失败!");
            }
        },
        error: function () { alert("网路错误,稍后重试..."); }
    });
}

/**
* 查看图片
* @auth:xzhang
* @date:2013-12-22
**/
var p_show_img = function (obj) {
    var ids = $("#" + obj).val();
    if (ids != "" && typeof (ids) != "undefined") {
        asyncbox.open({
            id: 'showimg',
            title: "查看文件",
            url: "/upfile/ShowImg.aspx",
            args: 'hiddenid=' + obj + '&obj=' + ids,
            width: 800,
            height: 350,
            modal: true,
            //flash：true,
            callback: function (btnRes, cntWin, returnValue) {
                //判断 btnRes 值
                if (btnRes == 'ok') {
                    alert(returnValue);
                }
            }
        });
    } else {
        alert("没有文件");
    }

}

/**
* 查看图片
* hiddenid：图片id
* type: 类型[0:无删除;1:有删除]
* @auth:xzhang
* @date:2013-12-29
**/
var ShowImgFile = function (hiddenid, type) {
    var ids = $("#" + hiddenid).val();
    if (typeof (type) == "undefined" || type == "") {
        type = 0;
    }
    if (ids != "" && typeof (ids) != "undefined") {
        asyncbox.open({
            id: 'showimg',
            title: "查看文件",
            url: "/upfile/ShowImg.aspx",
            args: 'hiddenid=' + hiddenid + '&type=' + type + '&obj=' + ids,
            width: 800,
            height: 350,
            modal: true,
            //flash：true,
            callback: function (btnRes, cntWin, returnValue) {
                //判断 btnRes 值
                if (btnRes == 'ok') {
                    alert(returnValue);
                }
            }
        });
    } else {
        alert("没有文件");
    }
}



/**
* 上传文件公共js方法
* @auth:xzhang
* @date:2013-12-22 17:15:45
* hiddenid:隐藏域id,imgurlid:提示图片id，msgid：提示消息ID
* hiddenid:隐藏域id,UPType:类型，msgid：提示消息ID ,method:回调方法，
**/
var ComUploadFile = function (hiddenid, UPType, msgid, method) {

    CommonData(hiddenid, UPType, msgid, method);

}


/**
* 上传资质文件js操作
* @auth:xzhang
* @date:2013-11-24
**/
$(function () {
    //法人身份证扫描件
    $("#Sup_btn").click(function () {
        CommonData("Sup_fileid", "", "Sup_msg");
    });

    //营业执照
    $("#txtUploadLegalCardyyzz").click(function () {
        CommonData("yyzz_LicenseFileId", "yyzz_LicenseImgId", "tip_yyzz_msg");
    });

    //组织机构代码
    $("#btn_zzjgdm_img").click(function () {
        CommonData("zzjgdm_fileid", "", "zzjgdm_msg");
    });

    //税务登记证
    $("#swdj_btn").click(function () {
        CommonData("swdj_fileid", "", "swdj_msg");
    });


    //=======注册部分==================
    //供应商基础信息  法人代表资质
    $("#txtUploadLegalCard").click(function () {
        CommonData("Sup_Legal_cardfile_id", "", "txtUploadLegalCard_msg");
    });

    //营业执照
    $("#txtUploadLegalCardyyzz").click(function () {
        CommonData("yyzz_LicenseFileId", "", "txtUploadLegalCardyyzz_msg");

    });

    //组织机构代码
    $("#btn_zzjgdm_LicenseFileId").click(function () {
        CommonData("zzjgdm_LicenseFileId", "", "btn_zzjgdm_LicenseFileId_msg");

    });


    //税务登记证
    $("#btn_swdj_LicenseFileId").click(function () {
        CommonData("swdj_LicenseFileId", "", "btn_swdj_LicenseFileId_msg");

    });

});