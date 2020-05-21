// 弹出框
function boxyFun(title, info, options) {
    options = $.extend({
        title: title || '艺吧提示',            //标题
        closeText: "x",                  //关闭文字
        modal: false,            //背景是否变暗  （设置成false才能移动的）
        fixed: true,            //窗口是否固定
        cache: true,            //是否被遮挡
        draggable: true,        //这个设定窗口是否可以拖动，要定义title才有效，设定了modal就无效
        center: true,            //弹出对话框是否居中
        x: 50,
        y: 50,                  //设定窗口位置值为％多少，设定此后center会被覆盖
        afterDrop: function () { },    //关闭对话框后执行的｛IE下面关闭不了,原因不明｝
        afterShow: function () { },    //打开对话框后执行的
        afterHide: function () { }    //隐藏对话框后执行的 }, options || {});
    }, options || {});
    new Boxy($("#" + info).html(), options);
    return false;
}

// boxy对话框扩展
var Boxy_Extensions = {
    options: {
        title: '艺吧提示',
        closeText: 'x'
    },
    //弹出后N秒后隐藏
    alertDelayFun: function (info, timer, options) {
        options = $.extend(this.options, options || {});
        new Boxy("<div style='padding-left:50px;padding-right:50px;text-align:center;font-size:14px;'>" + info + "</div>", $.extend({ behaviours: function () {
            setTimeout('$(".boxy-wrapper").hide();', timer);
        }
        }, options));
    },
    //弹出后，自动跳转
    alertHrefFun: function (info, href, options) {
        options = $.extend(this.options, options || {});
        new Boxy("<div style='padding-left:50px;padding-right:50px;text-align:center;font-size:14px;'>" + info + "</div>", $.extend({ behaviours: function () {
            location.href = href;
        }
        }, options));
    }
}

//<a href="#" onclick="Boxy.get(this).hide();">关闭对话框</a>
//操作提示
function boxy_HrefFun(info, href, options) {
    options = $.extend({
        title: '提示',
        closeText: "x",
        w: 100,
        h: 100
    }, options || {});
    //    new Boxy("<div style='padding-left:50px;padding-right:50px;text-align:center;font-size:14px;'>" + info + "</div>",
    //    $.extend({ behaviours: function () {
    //        location.href = href;
    //    }
    //}, options));

    Boxy.alert(info, function () { location.href = href; }, options)

}
function boxy_LoadFun(href, options) {
    options = $.extend({
        behaviours: function (r) { location.href = href; },
        title: '试听',
        closeText: "x"
    }, options || {});
    Boxy.load(href, options)
}

/*
<a href="a.html" onclick="return $.qbox(this);" >
<a href="a.html" onclick="return qBox.iFLoad(this);" >
<a href="a.html" onclick="return qBox.iFrame({src:'b.html'})" > Boxy属性不变 新增 w、h、src 属性
qBox.Close();关闭当前窗口
qBox.iFSrc({}); 改变当前窗口的属性及指向 Boxy属性不变 新增 w、h、src 属性
*/

//Boxy插件的扩展[Iframe扩展]
jQuery.fn.qbox = function (options) {
    var node = this.get(0).nodeName.toLowerCase();
    var self = this;
    if (node == 'a') {
        $(this).attr('onclick', '').unbind('click').click(function () { return false; });
        options = $.extend(options || {}, { src: this.get(0).getAttribute('href'), beforeUnload: function () { $(self).unbind('click').click(function () { return $(this).qbox(options); }); } });
    }
    qBox.iFLoad(options);
    return false;
}
var qBox = function () { };
jQuery.extend(qBox, {
    aDgs: [],
    iFrame: function (op) {

        op = jQuery.extend({ title: '艺吧提示', closeText: "x", w: 650, h: 320, src: 'about:blank', modal: false, fixed: false, unloadOnHide: true, success: function () { } }, op), fm = parseInt(Math.random() * (1000 * 987)); //
        var dialog = new Boxy("<b id=\"ld" + fm + "\">正在加载，请稍后....</b><iframe id=\"_" + fm + "\" style=\"width:0;height:0;display:none;margin:0;padding:0;\" src=" + op.src + " frameborder=\"0\" scrolling=\"no\"></iframe>", op);

        jQuery("#_" + fm).load(function () {
            dialog.resize(op.w, op.h, function () { });
            jQuery("#ld" + fm).remove();
            jQuery("#_" + fm).css({ 'padding': '15px', 'display': '' });
            op.success.call(op);
        });
        qBox.aDgs.push(dialog);
        return false;
    },
    Close: function () {
        qBox.aDgs[qBox.aDgs.length - 1].hide();
        return false
    },
    iFSrc: function (op) {
        op = jQuery.extend({ w: 650, h: 320, src: 'about:blank' }, op);
        qBox.aDgs[qBox.aDgs.length - 1].getContent().attr("src", "about:blank");
        qBox.aDgs[qBox.aDgs.length - 1].setTitle(op.t);
        qBox.aDgs[qBox.aDgs.length - 1].tween(op.w, op.h, function () { qBox.aDgs[qBox.aDgs.length - 1].getContent().attr("src", op.src).css({ width: op.w, height: op.h }); });
        return false;
    },
    iFLoad: function (options) {
        var sr = jQuery(this).attr("href");
        var op = jQuery.extend({ src: sr }, options);
        qBox.iFrame(op);
        return false;
    }

}); 