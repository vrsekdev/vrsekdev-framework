window.vrsekdev = {};

window.vrsekdev.frest = {
    addLiviconEvo: (element, config) => {
        $(element).addLiviconEvo(config);
    },
    stopLiviconEvo: (element) => {    
        $(element).stopLiviconEvo();
    },
    playLiviconEvo: (element) => {    
        $(element).playLiviconEvo();
    },
    initTooltip: (element) => {
        $(element).tooltip({
            container: "body"
          });
    },
    focusElement: (element) => {
        setTimeout(() => element.focus(), 0);
    },
    mainMenuExpand: () => {
        console.log("expanding");
        var $listItem = $('.main-menu li.menu-collapsed-open');
        $subList = $listItem.children('ul');

        $subList.hide().slideDown(200, function () {
          $(this).css('display', '');
        });

        $listItem.addClass('open').removeClass('menu-collapsed-open');
    },
    mainMenuCollapse: () => {
        setTimeout(function () {
            var $listItem = $('.main-menu li.open');
            $subList = $listItem.children('ul');
            $listItem.addClass('menu-collapsed-open');

            $subList.show().slideUp(200, function () {
                $(this).css('display', '');
            });

            $listItem.removeClass('open');
        }, 1);
    },
    mainMenuToggleCollapse: () => {
        // Toggle menu
        $.app.menu.toggle();

        setTimeout(function () {
            $(window).trigger("resize")
        }, 200);
    }
}