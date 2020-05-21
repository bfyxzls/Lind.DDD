//----------播放在线声音、上传声音控制
function onSoundCompleteHandler() {
    //播放完成
    TsingDa_Voice.playComplete();
}
var TsingDa_Voice = {
    PATH: "",
    state: 0,
    obj: null,
    name: '',
    timeStamp: function () {
        return (new Date()).getTime();
    },
    createDOM: function (url) {
        if ($('#tsingda_voice_play').length === 0) {
            $(document.body).append('<div id="tsingda_voice_play" style="width:1px; height:1px; overflow:hidden;"></div>'); //width:1px; height:1px; overflow:hidden;
        }
		var flashurl = 'http://resource.yi8edu.com/tsingda/voice/Mp3Player.swf';
		var parms = 'autoPlay=1&t=' + Math.random() + '&sourceUrl='+encodeURIComponent(url);
		var html = '<object classid="clsid:clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0" width="282" height="40"><param name="movie" value="' + flashurl + '"><param name="quality" value="high"><param name="flashVars" value="' + parms + '" /><param name="allowscriptaccess" value="always"/><embed src="' + flashurl + '" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" width="282" height="40" flashVars="' + parms + '" allowscriptaccess="always"></embed></object>';
	    $('#tsingda_voice_play').html(html);
    },
    play: function (obj, name, url) {
        if ($(obj).prop("play") !== undefined) {
            this.playComplete();
            return false;
        }
        if (this.state !== 0) {
            this.playComplete();
        }
        this.state = 1;
        this.name = name;
        this.obj = obj;
        $(obj).html("<img style='padding-top:4px;' src='http://static.yi8edu.com/images/042.gif'/>").prop('play', '1');
        this.createDOM(url);
    },
    playComplete: function () {
        this.state = 0;
        $(this.obj).html(this.name).removeProp('play');
        $("#tsingda_voice_play").html('');
    }
};
$(function () {
    $(document.body).delegate(".wei_btn_green", "click", function () {
        var obj = $(this).find(".wei_nubmer[url]");
        if ($(obj).length === 1) {
            var url = $(obj).attr("url");
            var name = $(obj).html();
            TsingDa_Voice.play($(obj), name, url);
        }
    });
});