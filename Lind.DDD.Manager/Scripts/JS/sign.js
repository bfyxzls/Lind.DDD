//签到任务
var Sign = {
    Data: {
        SignMonthDetailUrl: '/UserCenter/SignMonthDetail', //签到明细查询地址
        SignUrl: '/UserCenter/Sign/', //签到地址
        CurentShowDate: null, //当前显示的月份
        objSignButton: null, //签到对象
        objSignContainer: null, //签到显示的对象
        PreMonthSelector: '.PreMonth',
        NextMonthSelector: '.NextMonth',
        MonthDataDetail: {}, //每月明细
        timer: null,
        ajax_status: false
    },
    CreateDOM: function () {
        var hoverID = "divSignDetail_" + parseInt(Math.random() * 1000);
        this.Data.objSignContainer = "#" + hoverID;
        var str_yearmonth = new Date().getFullYear() + "年" + (new Date().getMonth() + 1) + "月";
        this.Data.CurentShowDate = new Date();
        var ary = ['<div class="qd_complet" id="' + hoverID + '">', '<div class="qd_complet_tx">', '<a href="javascript:void(0);" class="qd_complet_b_left PreMonth"></a>', '<div class="qd_complet_timer">' + str_yearmonth + '</div>', '<a href="javascript:void(0);" class="qd_complet_b_right NextMonth"></a>', '<div class="qd_complet_en">', '<h2></h2>', '<a href="javascript:void(0);" class="timer_close">×</a>', '</div>', '</div>', '<div class="timer_sheet"></div>', '</div>'];
        $(this.Data.objSignButton).closest("div").prepend(ary.join(''));
        //－－－生成当前月的详细信息
        this.ShowMonthDetail(new Date().getFullYear(), new Date().getMonth() + 1);

    },
    ShowMonthDetail: function (Year, Month) {
        var _this = this;
        var html = _this.GetMonthDetailHTML(Year, Month);
        if (html !== undefined && html !== null) {
            _this.ShowMonthDetailHTML(Year, Month, html);
            return false;
        }
        var data = { Year: Year, Month: Month };
        //---------------------------------------
        //显示指定月信息
        _this.Data.ajax_status = true;
        $.ajax({
            url: _this.Data.SignMonthDetailUrl,
            data: data,
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                if (data !== 0) {
                    _this.CreateMonthDOM(Year, Month, data);
                } else {
                    alert("年月参数错误");
                }
            }, error: function () {
                alert("加载数据失败");
            }
        });
    },
    CreateMonthDOM: function (Year, Month, data) {
        //生成指定月的明细
        var _this = this;
        var ary = ['<table cellpadding="0" cellspacing="0" width="100%" border="1" style="border-collapse: collapse;">', '<thead><tr> <td class="timer_td1"> 日</td><td> 一</td><td> 二</td><td>三 </td> <td>四</td><td>五</td><td>六</td></tr></thead>', '<tbody>'];

        var dtFirstDay = new Date(Year, Month - 1, 1);
        var weekDay = ["星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];
        var weeknum = dtFirstDay.getDay();
        if (weeknum > 0) {
            ary[ary.length] = new Array(weeknum + 1).join('<td class="td_bg_none"></td>');
        }
        var dayNum = new Date(Year, Month, 0).getDate();
        var curDate = new Date();
        var curDayNum = -1;
        if (curDate.getFullYear() === dtFirstDay.getFullYear() && curDate.getMonth() == dtFirstDay.getMonth()) {
            curDayNum = curDate.getDate();
        }
        var aryflag = _this.ToMonthAry(data);
        var strClass = "";
        for (var i = 1; i <= dayNum; ++i) {
            if (weeknum == 0) {
                ary[ary.length] = '<tr>';
            }
            //是否为当天
            if (aryflag[i] === true) {//签到
                ary[ary.length] = '<td>' + i + '</td>';
            } else {//没有签到
                if (curDayNum === i) {
                    strClass = "td_bg_hover";
                } else { strClass = "td_bg_none"; }
                ary[ary.length] = '<td class="' + strClass + '">' + i + '</td>';
            }
            if (weeknum == 6) {
                ary[ary.length] = '</tr>';
                weeknum = 0;
            } else {
                ++weeknum;
            }
        }
        if (weeknum > 0) {
            ary[ary.length] = new Array(8 - weeknum).join('<td class="td_bg_none"></td>');
            ary[ary.length] = '</tr>';
        }
        ary[ary.length] = '</tbody>';
        ary[ary.length] = '</table>';
        ary[ary.length] = '<p style="text-align:center; font-family:微软雅黑; font-size:20px; color:#3aa9ce; line-height:40px; border:1px solid #badcf6; border-top:none;"><span style="display:none;">恭喜您,签到成功</span>&nbsp;</p>';
        _this.Data.ajax_status = false;
        var html = ary.join('');
        _this.SetMonthDetailHTML(Year, Month, html);
        _this.ShowMonthDetailHTML(Year, Month, html);
    },
    GetMonthDetailHTML: function (Year, Month) {
        return this.Data.MonthDataDetail[Year + '.' + Month];
    },
    SetMonthDetailHTML: function (Year, Month, html) {
        this.Data.MonthDataDetail[Year + '.' + Month] = html;
    },
    ShowMonthDetailHTML: function (Year, Month, Html) {
        var _this = this;
        $(_this.Data.objSignContainer).find(".timer_sheet").html(Html);
        $(_this.Data.objSignContainer).find(".qd_complet_timer").html(Year + "年" + Month + "月");
        _this.Data.CurentShowDate = new Date(Year, Month - 1, 1);
    },
    ToMonthAry: function (data) {
        var ary_list = new Array(32);
        for (var i = 0, len = data.length; i < len; ++i) {
            ary_list[data[i]] = true;
        }
        return ary_list;
    },
    BindSignEvent: function () {
        //绑定签到事件
        var _this = this;
        //查看
        $(_this.Data.objSignButton).click(function () {
            //第一次点击签到
            if (_this.Data.objSignContainer === null) {
                _this.SignInit();
            } else {
                clearTimeout(_this.Data.timer);
                $(_this.Data.objSignContainer).fadeIn('fast');
            }
        });
    },
    SignAction: function (obj) {
        //签到行为
        var _this = this;
        //判断是否需要签到
        var issign = $(obj).is(".active");
        if (issign === true) {
            if ($(obj).data("ajax") === "1") {
                return false;
            }
            $(obj).data("ajax", "1");
            $.getJSON(_this.Data.SignUrl, function (data) {
                $(obj).removeData("ajax");
                if (data.code === 1 && data.year > 0 && data.month > 0) {//签到成功
                    $(obj).prop("class", "");
                    $(_this.Data.objSignContainer).find("span").show();
                    _this.SetMonthDetailHTML(data.year, data.month, $(_this.Data.objSignContainer).find(".timer_sheet").html());
                } else {//签到失败
                    alert("签到失败");
                }
            });
        } else {
            _this.SignInit();
        }


    },
    BindDetailEvent: function () {
        //绑定明细事件
        var _this = this;
        //点头关闭
        $(_this.Data.objSignContainer).find(".timer_close").click(function () {
            $(_this.Data.objSignContainer).fadeOut('fast');
        });
        //自动关闭
        $(_this.Data.objSignContainer).mouseout(function () {
            clearTimeout(_this.Data.timer);
            _this.Data.timer = setTimeout(function () { $(_this.Data.objSignContainer).find(".timer_close").trigger("click") }, 800);
        }).mouseover(function () {
            clearTimeout(_this.Data.timer);
        });
        //--上个月、下个月
        $(_this.Data.objSignContainer).delegate(_this.Data.PreMonthSelector, 'click', function () {
            var dtTmp = _this.Data.CurentShowDate;
            var dt = new Date(dtTmp.getFullYear(), dtTmp.getMonth() - 1, 1);
            _this.ShowMonthDetail(dt.getFullYear(), dt.getMonth() + 1);
        });
        $(_this.Data.objSignContainer).delegate(_this.Data.NextMonthSelector, 'click', function () {
            var dtTmp = _this.Data.CurentShowDate;
            var dt = new Date(dtTmp.getFullYear(), dtTmp.getMonth() + 1, 1);
            _this.ShowMonthDetail(dt.getFullYear(), dt.getMonth() + 1);
        });
        //用户签到
        $(_this.Data.objSignContainer).delegate('.active,.td_bg_hover', 'hover', function () {
            if ($(this).is(".td_bg_hover")) {
                $(this).prop("class", "active");
            } else {
                $(this).prop("class", "td_bg_hover");
            }
        });
        $(_this.Data.objSignContainer).delegate('.active,.td_bg_hover', 'click', function () {
            _this.SignAction($(this));
        });
    },
    SignInit: function () {
        ///<summary>
        ///生成签到明细
        ///</summary>
        var _this = this;
        _this.CreateDOM();
        _this.BindDetailEvent();
    },
    Init: function (objBtn) {
        if ($(objBtn).is("a")) {
            this.Data.objSignButton = objBtn;
            this.BindSignEvent();
        }
    }
};
