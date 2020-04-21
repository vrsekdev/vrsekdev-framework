(function($) {
  'use strict';
  $(function() {
    var body = $('body');
    var sidebar = $('.sidebar');

    //Close other submenu in sidebar on opening any
    sidebar.on('show.bs.collapse', '.collapse', function() {
      sidebar.find('.collapse.show').collapse('hide');
    });

    //Change sidebar
    $('[data-toggle="minimize"]').on("click", function() {
      body.toggleClass('sidebar-icon-only');
    });

    //checkbox and radios
    $(".form-check label,.form-radio label").append('<i class="input-helper"></i>');
  });
})(jQuery);