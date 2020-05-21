//通用地区，选择控制
//<reference path="/JS/jquery-1.8.2.js" />
var Area = {
    Data:
    {
        objProvince: null,
        objCity: null,
        objArea: null,
        Data: null
    },
    Load: function (v) {
        var _this = this;
        $.getJSON("/Common/Area", function (data) {
            _this.Data.Data = data;
            _this.InitData(v);
        });
    },
    InitData: function (v) {
        //初始化数据
        var _this = this;
        var area_data = this.Data.Data;
        var len = this.Data.Data.length;
        $(_this.Data.objProvince).html("<option value='0'>省份</option>");
        $(_this.Data.objCity).html("<option value='0'>城市</option>");
        $(_this.Data.objArea).html("<option value='0'>区/县</option>");
        //var ary = new Array(0, 0, 0);
        //ary = this.FindIndex(v, ary);
        var provinceid = 0, cityid = 0, areaid = 0;
        if (v > 100000) {
            areaid = v;
        }
        if (areaid > 0 || v > 1000) {
            cityid = parseInt(v.toString().substring(0, 4), 10);
        }
        if (cityid > 0 || v > 10) {
            provinceid = parseInt(v.toString().substring(0, 2), 10);
        }
        //绑定县级
        if (cityid > 0) {
            for (var i = 0; i < len; i++) {
                if (area_data[i].ParentID == cityid) {
                    if (areaid === area_data[i].CodeID) {
                        $(_this.Data.objArea).append("<option value='" + area_data[i].CodeID + "' selected='selected'>" + area_data[i].Name + "</option>");
                    } else {
                        $(_this.Data.objArea).append("<option value='" + area_data[i].CodeID + "'>" + area_data[i].Name + "</option>");
                    }
                }
            }
        }
        //绑定市级
        if (provinceid > 0) {
            for (var i = 0; i < len; i++) {
                if (area_data[i].ParentID == provinceid) {
                    if (cityid === area_data[i].CodeID) {
                        $(_this.Data.objCity).append("<option value='" + area_data[i].CodeID + "' selected='selected'>" + area_data[i].Name + "</option>");
                    } else {
                        $(_this.Data.objCity).append("<option value='" + area_data[i].CodeID + "'>" + area_data[i].Name + "</option>");
                    }
                }
            }
        }
        //绑定省级
        for (var i = 0; i < len; i++) {
            if (area_data[i].ParentID == 0) {
                if (provinceid === area_data[i].CodeID) {
                    $(_this.Data.objProvince).append("<option value='" + area_data[i].CodeID + "' selected='selected'>" + area_data[i].Name + "</option>");
                } else {
                    $(_this.Data.objProvince).append("<option value='" + area_data[i].CodeID + "'>" + area_data[i].Name + "</option>");
                }
            }
        }
        _this.BindEvent();
    },
    FindIndex: function (v, ary) {
        if (v === 0) {
            return ary;
        }
        var area_data = this.Data.Data;
        var len = this.Data.Data.length;
        var pid = 0;
        for (var i = 0; i < len; i++) {
            if (v === area_data[i].CodeID) {
                pid = area_data[i].ParentID;
                ary.unshift(v);
                break;
            }
        }
        if (pid === 0) {
            return ary;
        } else {
            return this.FindIndex(pid, ary);
        }
    },
    BindEvent: function () {
        var _this = this;
        var area_data = this.Data.Data;
        var len = this.Data.Data.length;
        //绑定事件
        if (this.Data.objProvince !== null) {
            $(this.Data.objProvince).click(function () {
                var val = parseInt($(this).val());
                var ary = [];
                ary[ary.length] = "<option value='0'>城市</option>";
                $(_this.Data.objArea).html("<option value='0'>区/县</option>");
                if (val === 0) {
                    return false;
                }
                for (var i = 0; i < area_data.length; i++) {
                    if (area_data[i].ParentID == val) {
                        ary[ary.length] = "<option value='" + area_data[i].CodeID + "'>" + area_data[i].Name + "</option>";
                    }
                }
                $(_this.Data.objCity).html(ary.join(''));
                return false;
            });
        }
        if (this.Data.objCity !== null && this.Data.objArea !== null) {
            $(_this.Data.objCity).click(function () {
                var val = parseInt($(this).val());
                var ary = [];
                ary[ary.length] = "<option value='0'>区/县</option>";
                if (val === 0) {
                    return false;
                }
                for (var i = 0; i < area_data.length; i++) {
                    if (area_data[i].ParentID == val) {
                        ary[ary.length] = "<option value='" + area_data[i].CodeID + "'>" + area_data[i].Name + "</option>";
                    }
                }
                $(_this.Data.objArea).html(ary.join(''));
                return false;
            });
        }
    },
    Init: function (objProvince, objCity, objArea, v) {
        //绑定显示地区等信息
        this.Data.objProvince = objProvince;
        this.Data.objCity = objCity;
        this.Data.objArea = objArea;
        this.Load(parseInt(v, 10));
    }
};