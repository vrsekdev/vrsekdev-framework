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
      console.log("collapsing main menu")

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
    },
    menuItemCollapse: (listItem) => {
        var $subList = $(listItem).children('ul');

        $(listItem).removeClass('open');
        $subList.show().slideUp($.app.nav.config.speed, function () {
          $(this).css('display', '');
          $(this).find('> li').removeClass('is-shown');

          $.app.nav.container.trigger('collapsed.app.menu');
        });
      },
      menuItemExpand: (listItem) => {
        var $subList = $(listItem).children('ul');
        var $children = $subList.children('li').addClass('is-hidden');

        $subList.hide().slideDown($.app.nav.config.speed, function () {
          $(this).css('display', '');
  
          $.app.nav.container.trigger('expanded.app.menu');
        });
  
        setTimeout(function () {
          $children.addClass('is-shown');
          $children.removeClass('is-hidden');
        }, 0);
      },
}