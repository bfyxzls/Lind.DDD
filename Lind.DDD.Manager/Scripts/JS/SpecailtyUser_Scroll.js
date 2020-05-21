$(function () {
    $("#specialList_Id").hover(function () {
        $(this).attr("name", "hovered");
    }, function () {
        $(this).removeAttr("name");
    });
    //setInterval("specialty_Scroll()", 5000);
    setInterval('AutoScroll("#specialList_Id")', 5000)
})

//向上翻的效果
function AutoScroll(obj) {
    if ($("#specialList_Id").attr("name") != "hovered") {
        var firstTop = $(obj).find(".zyb_list_box:first").height();
        $(obj).find("#specialList").animate({
            marginTop: "-" + firstTop + "px"
        }, 800, function () {
            $("#specialList_Id .zyb_list_box").eq(4).fadeIn(500);
            $("#specialList_Id .zyb_list_box:gt(5)").attr("style", "display:block");
            $(this).css({ marginTop: "0px" }).find(".zyb_list_box:first").appendTo(this);
            $("#specialList_Id .zyb_list_box:gt(4)").attr("style", "display:none");
        });
    }
}

//向下翻的效果
function specialty_Scroll() {
    if ($("#specialList_Id").attr("name") != "hovered") {
        $("#specialList_Id .zyb_list_box:last").hide().insertBefore($("#specialList_Id .zyb_list_box:first").slideDown(1000));
        $("#specialList_Id .zyb_list_box").eq(5).attr("style", "display:none");
    }
}
