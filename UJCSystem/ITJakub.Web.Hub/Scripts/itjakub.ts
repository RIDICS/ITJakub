//window.onload = () => { alert("hello from typescript!"); }

$('#main-plugins-menu').find('li').click(function (event: Event) {
    $('#main-plugins-menu').find('li').removeClass('active');
    $(this).addClass('active');
    $(this).parents('li').addClass('active');
    //var submenu = $('#main-plugins-menu').find('li.active').parent('ul.has-sub');
    //$(submenu).css('margin-left', mrg + 'px');
    event.stopPropagation();
});