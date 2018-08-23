//关闭提示按钮
var showBtn = {
  dom:$('.cover'),
  init:function(){
    this.bindEvent();
  },
  bindEvent:function(){
      

    this.dom.click(function(){
      $(this).hide();
    })
  },
  show:function(){
    this.dom.show();
  }
}

//下载按钮
var downBtn = {
    dom: $('.dl_main_right'),
    init: function () {
        this.bindEvent();
    },
    bindEvent: function () {
        var isAndroid = false;
        var isIOS = false;
        //判断是安卓手机还是苹果
        var u = navigator.userAgent;
        if (u.indexOf('Android') > -1 || u.indexOf('Linux') > -1) {
            //安卓手机
            //location.href = "http://api.xiaobaochuxing.com/app/xiaobaochuxing280.apk";
            isAndroid = true;
            var ua = navigator.userAgent.toLowerCase();//获取判断用的对象
            if (ua.match(/MicroMessenger/i) == "micromessenger") {
                //在微信中打开
                this.dom.click(function () {
                    showBtn.show();
                });
                return;
            }
        }
        else if (u.indexOf('iPhone') > -1) {
            isIOS = true;
            //苹果手机
            var ua = navigator.userAgent.toLowerCase();//获取判断用的对象
            if (ua.match(/MicroMessenger/i) == "micromessenger") {
                //在微信中打开
                this.dom.click(function () {
                    showBtn.show();
                });
                return;
            }
        }
        this.dom.click(function () {
            if (isAndroid) {
                location.href = "http://api.xiaobaochuxing.com/app/xiaobaochuxing280.apk";
            }
            if (isIOS) {
                alert("正在审核中");
                //location.href="http://a.app.qq.com/o/simple.jsp?pkgname=";
            }
        })
    }
}

//判断打开方式
window.onload = function(){

  showBtn.init();
  downBtn.init();
  if (browser.versions.mobile) {//判断是否是移动设备打开。
      var ua = navigator.userAgent.toLowerCase();//获取判断用的对象
      if (ua.match(/MicroMessenger/i) == "micromessenger") {
        //在微信中打开
      }
     
    } else {
      //否则就是PC浏览器打开
  }
}