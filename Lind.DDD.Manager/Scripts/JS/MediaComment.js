$(function () {

    $("#inputMaxNum").text($("#Content").attr("textlength"));
    $("#inputNum").text($("#Content").attr("textlength"));
    var txtLength = $("#Content").attr("textlength");

    $("#Content").keyup(function () {
        if (txtLength - $("#Content").val().length < 0) {
            $("#inputNum").css("color", "#f69");
            $("#inputNum").text($("#Content").val().length);
            $("#postComment").attr("disabled", "disabled");
        }
        else {
            $("#errContent").html("");
            $("#inputNum").css("color", "#999");
            $("#inputNum").text($("#Content").val().length);
            $("#postComment").removeAttr("disabled");
        }
    });
})

function CommendOrReply(mediaReviewId, mediaReview_UserName, content) {
    var excuteContent=content.replace(/<[^>]+>/g, ""); 
    $("#mediaReviewId").val(mediaReviewId);
    $("#Content").val("//@" + mediaReview_UserName + "：" + excuteContent);
    locatePoint();
    $("#inputNum").text($("#Content").val().length);
    $("#errContent").html("");
}

//点击让Textarea文本域中的光标定位
function locatePoint() {
    var textArea = document.getElementById("Content");
    if (textArea.setSelectionRange) {        //IE的定位
        setTimeout(function () {
            textArea.setSelectionRange(0, 0); //将光标定位在textarea的开头，需要定位到其他位置的请自行修改 
            textArea.focus();
        }, 0);
    } else if (textArea.createTextRange) {    //FF,等的定位
        var txt = textArea.createTextRange();
        txt.moveEnd("character", 0 - txt.text.length);
        txt.select();
    }
}    
    