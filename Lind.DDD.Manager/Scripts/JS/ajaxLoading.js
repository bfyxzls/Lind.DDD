/******重写Ajax操作,通用Loading操作*******/
$.ajaxLoading = function (options, aimDivObj) {
    var img = $("<img id=\"progressImgage\"  src=\"/Scripts/js/img/LoadingPng.gif\" />"); //Loading小图标
    var errorMsg = $("<div>数据加载失败...</div>");
    var mask = $("<div id=\"maskOfProgressImage\"></div>").addClass("mask").addClass("hide"); //Div遮罩
    var PositionStyle = "fixed";
    //是否将Loading固定在aimDiv中操作,否则默认为全屏Loading遮罩
    if (aimDivObj != null && aimDivObj != "" && aimDivObj != undefined) {
        $(aimDivObj).css("position", "relative").append(img).append(mask);
        $(aimDivObj).css("position", "relative").append(errorMsg).append(mask);
        PositionStyle = "absolute";
    }
    else {
        $("body").append(img).append(mask);
        $("body").append(errorMsg).append(mask);

    }

    errorMsg.css({
        "display": "none",
        "position": PositionStyle,
        "top": "50%",
        "left": "50%",
        "background": "#eee",
        "padding": "20px",
        "border": "1px solid #333",
        "margin-top": function () { return -1 * img.height() / 2; },
        "margin-left": function () { return -1 * img.width() / 2; }
    });

    img.css({
        "z-index": "2000",
        "display": "none"
    })
    mask.css({
        "position": PositionStyle,
        "top": "0",
        "right": "0",
        "bottom": "0",
        "left": "0",
        "z-index": "1000",
        "background-color": "#000000",
        "display": "none"
    });
    var complete = options.complete;
    options.complete = function (httpRequest, status) { // status 可能为：null、'success'、 'notmodified'、 'error'、 'timeout'、 'abort'或'parsererror'等
        img.hide();
        mask.hide();
        errorMsg.hide();
        if (complete) {
            complete(httpRequest, status);
        }
    };
    options.error = function () {
       // errorMsg.show();
    }

    options.async = true;
    img.show().css({
        "position": PositionStyle,
        "top": "50%",
        "left": "50%",
        "margin-top": function () { return -1 * img.height() / 2; },
        "margin-left": function () { return -1 * img.width() / 2; }
    });
    mask.show().css("opacity", "0.1");
    
    $.ajax(options); //执行ajax请求
};
