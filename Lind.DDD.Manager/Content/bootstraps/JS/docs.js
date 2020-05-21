// jscs:disable requireCamelCaseOrUpperCaseIdentifiers

'use strict';

$(function () {
    $.ajax({
        url: 'https://api.github.com/repos/vsn4ik/bootstrap-submenu',
        success: function (data) {
            // XSS check
            if (typeof data.stargazers_count != 'number') {
                return;
            }

            var $group = $('<div class="input-group"><span class="input-group-btn"></span></div>');

            $group.append('<span class="input-group-addon">' + data.stargazers_count + '&nbsp;<span class="octicon octicon-star"></span></span>');

            $('#gh-view-link').wrap($group);
        }
    });

    $('#scroll_top').on('click', function () {
        this.disabled = true;

        // 'html' for Mozilla Firefox, 'body' for other browsers
        $('body, html').animate({
            scrollTop: 0
        }, 800, $.proxy(function () {
            this.disabled = false;
        }, this));

        this.blur();
    });

    // Dropdown fix
    $('.dropdown > a[tabindex]').on('keydown', function (event) {
        // 13: Return

        if (event.keyCode == 13) {
            $(this).dropdown('toggle');
        }
    });

    // 妤把快忱抉找志把忘投忘快技 戒忘抗把抑找我快 扭把我 抗抖我抗快 扶忘 扶快忘抗找我志扶抑抄 改抖快技快扶找 扼扭我扼抗忘
    $('.dropdown-menu > .disabled, .dropdown-header').on('click.bs.dropdown.data-api', function (event) {
        event.stopPropagation();
    });

    $('[data-submenu]').submenupicker();

   // hljs.initHighlighting();
});
