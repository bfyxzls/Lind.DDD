function $$(id) {
    return $("#" + id);
}
var YiBaDate = {
    Data: {
        ddlYear: null,
        ddlMonth: null,
        ddlDay: null,
        MonthDay: [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    },
    ///－－－－－－－－－下拉框start－－－－－－－－－
    LoadDropDownList: function (def) {
        var defYear = null, defMonth = null, defDay = null;
        if (this.IsValidDate(def)) {
            var result = def.match(/^(\d{4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
            var defDate = new Date(result[1], result[3] - 1, result[4]);
            defYear = defDate.getFullYear();
            defMonth = defDate.getMonth() + 1;
            defDay = defDate.getDate();
        }
        this.InitYear(defYear);
        this.InitMonth(defMonth);
        this.InitDay(defDay);

        this.BindDropDownListEvent();
    },
    IsRunYear: function (year) {
        return (0 == year % 4 && (year % 100 != 0 || year % 400 == 0));
    },
    IsValidDate: function (str) {
        //判断是否是有效的长日期格式 - "YYYY-MM-DD" || "YYYY/MM/DD"
        if (str === null) { return false; }
        var result = str.match(/^(\d{4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
        if (result == null) return false;
        var d = new Date(result[1], result[3] - 1, result[4]);
        return (d.getFullYear() == result[1] && (d.getMonth() + 1) == result[3] && d.getDate() == result[4]);
    },
    InitYear: function (def) {
        //－－－－初始化年－－－－
        var ddlYear = this.Data.ddlYear;
        var cur_year = new Date().getFullYear();
        $$(ddlYear).html(this.CreateOptionList(1920, cur_year, def, '年'));
    },
    InitMonth: function (def) {
        $$(this.Data.ddlMonth).html(this.CreateOptionList(1, 12, def, '月'));
    },
    InitDay: function (def) {
        //初始化日
        var year = $(this.Data.ddlYear).val();
        var month = $(this.Data.ddlMonth).val();
        var ddlDay = this.Data.ddlDay;
        var ary_option = [];
        var cur_month_days = this.Data.MonthDay[month - 1];
        if (this.IsRunYear(year)) { ++cur_month_days; }

        $$(ddlDay).html(this.CreateOptionList(1, cur_month_days, def, '日'));
    },
    CreateOptionList: function (start, end, def, flag) {
        //创建Option
        var ary_option = [];
        for (var i = start; i <= end; ++i) {
            if (i === def) {
                ary_option[ary_option.length] = '<option value="' + i + '" selected="selected">' + i + flag + '</option>';
            } else {
                ary_option[ary_option.length] = '<option value="' + i + '">' + i + flag + '</option>';
            }
        }
        return ary_option.join('');
    },
    BindDropDownListEvent: function () {
        var _this = this;
        $$(_this.Data.ddlYear).change(function () {
            _this.InitDay();
        });
        $$(_this.Data.ddlMonth).change(function () {
            _this.InitDay();
        });
    },
    InitDropDownList: function (ddlyear, ddlmonth, ddlday, def) {
        if ($$(ddlyear).is("select") && $$(ddlmonth).is("select") && $$(ddlday).is("select")) {
            this.Data.ddlYear = ddlyear;
            this.Data.ddlMonth = ddlmonth;
            this.Data.ddlDay = ddlday;
            this.LoadDropDownList(def);
        }
    }
    ///－－－－－－－－－下拉框end－－－－－－－－－
};