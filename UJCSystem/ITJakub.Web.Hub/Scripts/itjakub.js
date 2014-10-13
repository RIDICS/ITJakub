//window.onload = () => { alert("hello from typescript!"); }
$('#main-plugins-menu').find('li').click(function (event) {
    $('#main-plugins-menu').find('li').removeClass('active');
    $(this).addClass('active');
    $(this).parents('li').addClass('active');
    event.stopPropagation();
});
//# sourceMappingURL=itjakub.js.map
