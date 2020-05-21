var YiBaDialog = {
    Data: {
        list: []
    },
    //<div style="position: absolute; top: 0px; left: 0px; z-index: 10001; background-color: rgb(0, 0, 0); opacity: 0.3; width: 900px; height: 845px;"></div>
    //    DEFAULTS: {
    //        title: null,           // titlebar text. titlebar will not be visible if not set.
    //        closeable: true,           // display close link in titlebar?
    //        draggable: true,           // can this dialog be dragged?
    //        clone: false,          // clone content prior to insertion into dialog?
    //        actuator: null,           // element which opened this dialog
    //        center: true,           // center dialog in viewport?
    //        show: true,           // show dialog immediately?
    //        modal: false,          // make dialog modal?
    //        fixed: true,           // use fixed positioning, if supported? absolute positioning used otherwise
    //        closeText: '[关闭]',      // text to use for default close link
    //        unloadOnHide: false,          // should this dialog be removed from the DOM after being hidden?
    //        clickToFront: false,          // bring dialog to foreground on any click (not just titlebar)?
    //        behaviours: Boxy.EF,        // function used to apply behaviours to all content embedded in dialog.
    //        afterDrop: Boxy.EF,        // callback fired after dialog is dropped. executes in context of Boxy instance.
    //        afterShow: Boxy.EF,        // callback fired after dialog becomes visible. executes in context of Boxy instance.
    //        afterHide: Boxy.EF,        // callback fired after dialog is hidden. executed in context of Boxy instance.
    //        beforeUnload: Boxy.EF         // callback fired after dialog is unloaded. executed in context of Boxy instance.
    //    },
    _CreateID: function () {
        return "yibadialog_" + parseInt(Math.random() * 100000);
    },
    _CreateDOM: function (id) {
        var ary = [];
        if (id === null || id === undefined) {
            id = this._CreateID();
        }
        ary[ary.length] = '<div class="rw_show_box" id="' + id + '" style="background:white;">';
        ary[ary.length] = '<h2 class="rw_show_box_h2"><span class="title">提示</span><a onclick="YiBaDialog.CloseAll();">x</a></h2>';
        ary[ary.length] = '<div class="rw_show_box_t1 yibadialogcontent" style="background:white;">';
        ary[ary.length] = '</div>';
        ary[ary.length] = '</div>';
        return ary.join('');
    },
    centerPop: function (filed) {
        var windowWidth = document.documentElement.clientWidth;
        var windowHeight = document.documentElement.clientHeight;
        var popupHeight = $(filed).height();

        var popupWidth = $(filed).width();
        var bodyheight = $("body").height();
        var yScroll;
        if (self.pageYOffset) {
            yScroll = self.pageYOffset;
        } else if (document.documentElement && document.documentElement.scrollTop) {
            yScroll = document.documentElement.scrollTop;
        } else if (document.body) {
            yScroll = document.body.scrollTop;
        }
        $(filed).css({
            "position": "absolute",
            "top": (windowHeight - popupHeight) / 3 + yScroll,
            "left": windowWidth / 2 - popupWidth / 2
        });

        if (bodyheight < windowHeight) {
            $("#backgroundPopup").css({
                "height": windowHeight
            });
        } else {
            $("#backgroundPopup").css({
                "height": bodyheight
            });
        }
    },
    alert: function (title, obj) {
        var id = this._CreateID();
        this.Data.list[this.Data.list.length] = id;
        var html = this._CreateDOM(id);
        $(document.body).append(html);
        if ($(obj).is("*")) {
            $("#" + id).find(".yibadialogcontent").html($(obj).clone(true));
        } else {
            $("#" + id).find(".yibadialogcontent").html(obj);
        }
        $("#" + id).find(".title").html(title);
        this.centerPop($("#" + id));
    },
    alertURL: function (url) {
        var id = this._CreateID();
        this.Data.list[this.Data.list.length] = id;
        var obj = this._CreateDOM(id);
        $(document.body).append(obj);
        this.centerPop($("#" + id));
    },
    CloseAll: function () {
        for (var i = 0, len = this.Data.list.length; i < len; i++) {
            $("#" + this.Data.list[i]).remove();
            this.Close(this.Data.list[i]);
        }
        this.Data.list = [];
    },
    Close: function (id) {
        if ($("#" + id).is("div")) {
            $("#" + id).remove();
        }
    }
};