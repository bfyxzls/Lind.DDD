var Props = {
    Data: {
        UserProps: null,
        FreeZoneProps: null,
        TempObj: null
    },
    artDialog: function (msg) {
        Boxy.alert(msg, null, { title: "提示信息" });
    },
    GetProps: function (flag, mediaid) {
        ///获取用户道具信息
        var _this = this;
        $.ajax({
            url: '/Common/Props/',
            data: 'mediaid=' + mediaid + '&flag=' + flag,
            type: 'POST',
            success: function (data) {
                if (flag === 1) {//作品显示页
                    _this.PlayShowProps(data);
                } else if (flag === 2) {//自由区
                    _this._CreateZoneFreePropsDOM(data);
                }
                else if (flag == 3) {//话题页
                    _this._CreateTopicProps(data);
                }
            },
            error: function (data) {
                _this.artDialog("获取用户道具信息失败");
            }
        });
    },
    //-------------------------------------作品展示页---------------------------------------
    PlayShowProps: function (data) {
        //作品页
        var ary = [];
        for (var i = 0, len = data.length; i < len; i++) {
            var strAmount = "";
            if (data[i].IsFree == 0) {
                strAmount = '<span class="color_red">[已收到<font class="num" num="' + data[i].UserAmount + '">' + data[i].UserAmount + '</font>' + data[i].Unit + ']</span>';
            } else {
                strAmount = "";
            }
            ary[ary.length] = '<li><a href="javascript:void(0);" propsid="' + data[i].PropsID + '"><img src="' + data[i].SmallImg + '"/>' + data[i].Name + '</a><span>(人气+' + data[i].PopularityNum + '点)</span>' + strAmount + '</li>';
        }
        $("#ulPoropsList").html(ary.join(''));
    },
    BindEvent: function () {
        var _this = this;
        $("#ulPoropsList").delegate("a[propsid]", "click", function () {
            var obj = $("#ulPoropsList");
            var propsid = parseInt($(this).attr("propsid"));
            var mediaid = parseInt($(obj).attr("mediaid"));
            _this.UseProps(mediaid, propsid, $(this));
        });
    },
    UseProps: function (MediaID, PropsID, obj) {
        //使用道具
        if ($(document.body).attr("ajax") === "1") { return false; }
        $(document.body).attr("ajax", "1");
        var data = { MediaID: MediaID, PropsID: PropsID };
        var _this = this;
        $.ajax({
            url: '/Common/UseProps/',
            data: data,
            type: 'POST',
            success: function (data) {
                if (data.code === 1) {
                    $(document.body).removeAttr("ajax");
                    _this.artDialog("使用成功");
                    //设置当前作品的人气
                    if (data.num !== undefined && data.num > 0 && $("#divPopu").is("div")) {
                        $("#divPopu").find(".pPopularityNum").html(data.num);
                    }
                    //增加当前装备的次数数量
                    if ($(obj).siblings(".color_red").find(".num").is("[num]")) {
                        var spanNum = $(obj).siblings(".color_red").find(".num");
                        var num = parseInt($(spanNum).attr("num"));
                        ++num;
                        if (num == 0) {
                            $(spanNum).remove();
                        } else {
                            $(spanNum).attr("num", num).html(num);
                        }
                    }
                } else if (data.code === -100) {
                    qBox.iFLoad({ src: '/User/WindowLogin.shtml?ReturnUrl=' + encodeURIComponent(location.href), title: '登录', w: 500, h: 275, success: function () { $(document.body).removeAttr("ajax"); } });
                    return false;
                } else {
                    $(document.body).removeAttr("ajax");
                    var msg = data.msg;
                    if (msg === undefined) { msg = "联系客服"; }
                    _this.artDialog(msg);
                }
            }, error: function () {
                $(document.body).removeAttr("ajax");
                _this.artDialog("使用道具失败");
            }
        });
    },
    InitPlayMedia: function (mediaid) {
        this.GetProps(1, mediaid);
        this.BindEvent();
    },
    //-------------------------------------自由区(通用)---------------------------------------
    _CreateZoneFreePropsDOM: function (data) {
        //创建自由区道具DOM
        var ary = [];
        if ($(Props.Data.TempObj).is("a")) {
            ary[ary.length] = '<div class="n_new_alist_hecon" style=" left:39px; ">';
        } else {
            ary[ary.length] = '<div class="n_new_alist_hecon">';
        }
        for (var i = 0, len = data.length; i < len; i++) {
            ary[ary.length] = '<div class="n_new_alist_he"><img src="' + data[i].MiddleImg + '"><a href="javascript:void(0);" style="padding:0;" propsid="' + data[i].PropsID + '">' + data[i].Name + '</a></div>';
        }
        ary[ary.length] = '</div>';
        var strhtml = ary.join('');
        Props.Data.FreeZoneProps = strhtml;
        $(Props.Data.TempObj).after(strhtml);
    },
    _FirstShowZoneFreeProps: function (obj) {
        //显示自由区道具
        if (this.Data.FreeZoneProps != null) {
            $(obj).after(this.Data.FreeZoneProps);
        } else {
            this.Data.TempObj = obj;
            this.GetProps(2, 0);
        }
    },
    BindFreeZoneEvent: function () {
        var _this = this;
        var timer = null;
        $("#medialist").delegate("a[mediaid]", "mouseenter", function () {
            //显示道具
            if (timer != null) {
                clearTimeout(timer);
                timer = null;
            }
            if ($(this).siblings(".n_new_alist_hecon").is("div")) {
                $(this).siblings(".n_new_alist_hecon").show();
            } else {
                _this._FirstShowZoneFreeProps($(this));
            }
        });
        $("#medialist").delegate(".n_zpright", "mouseleave", function () {
            //隐藏道具
            if ($(this).find(".n_new_alist_hecon").is("div")) {
                $(this).find(".n_new_alist_hecon").hide();
            }
        });
        $("#medialist").delegate("a[propsid]", "click", function () {
            //使用道具
            var mediaid = parseInt($(this).parents(".n_new_alist_hecon").siblings("a[mediaid]").attr("mediaid"));
            var propsid = parseInt($(this).attr("propsid"));
            _this.UseProps(mediaid, propsid, $(this));
        });
    },
    InitFreeZone: function () {
        this.BindFreeZoneEvent();
    },
    //-------------------------------------话题详细页---------------------------------------
    _CreateTopicProps: function (data) {
        var isFree = ($(this.Data.TempObj).find('a').is('.a1') ? 1 : 0); //免费
        var ary = [];
        for (var i = 0, len = data.length; i < len; i++) {
            if (isFree !== data[i].IsFree) {
                continue;
            }
            ary[ary.length] = '<div class="n_new_alist_he"><img src="' + data[i].MiddleImg + '"><a href="javascript:void(0);" propsid="' + data[i].PropsID + '">' + data[i].Name + '</a></div>';
        }
        $(Props.Data.TempObj).find(".n_new_alist_hecon").html(ary.join(''));
    },
    InitTopic: function () {
        $(".n_new_alist .n_new_alist_a[props]").mouseenter(function () {
            if ($(this).find('.n_new_alist_hecon').is("div")) {
                $(this).find('.n_new_alist_hecon').fadeIn();
            } else {
                Props.Data.TempObj = $(this);
                $(this).append('<div class="n_new_alist_hecon"></div>');
                Props.GetProps(3, 0);
            }
        }).mouseleave(function () {
            $(this).find('.n_new_alist_hecon').delay(100).fadeOut();
        });
        $(".n_new_alist").delegate("a[propsid]", "click", function () {
            //使用道具
            var mediaid = parseInt($(this).parents("div[mediaid]").attr("mediaid"));
            var propsid = parseInt($(this).attr("propsid"));
            Props.UseProps(mediaid, propsid, $(this));
        });

    }
};