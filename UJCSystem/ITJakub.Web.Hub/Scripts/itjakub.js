//window.onload = () => { alert("hello from typescript!"); }
//$('#main-plugins-menu').find('li').click(function (event: Event) {
//    $('#main-plugins-menu').find('li').removeClass('active');
//    $(this).addClass('active');
//    $(this).parents('li').addClass('active');
//    //var submenu = $('#main-plugins-menu').find('li.active').parent('ul.has-sub');
//    //$(submenu).css('margin-left', mrg + 'px');
//    event.stopPropagation();
//});
//sets state to main plugins menu
$(document).ready(function () {
    $('#main-plugins-menu').find('li').removeClass('active');
    var href = window.location.pathname;
    console.log(href.toString());
    var liTargetingActualPage = $('#main-plugins-menu').find("a[href='" + href.toString() + "']").parent('li');
    $(liTargetingActualPage).addClass('active');
    $(liTargetingActualPage).parents('li').addClass('active');
});

//list item showing hidden context
$(document).ready(function () {
    $('ul.listing').find('li.list-item').find('.show-button').click(function (event) {
        $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
        $(this).siblings('.hide-button').show();
        $(this).hide();
    });
});

//list item hiding hidden context
$(document).ready(function () {
    $('ul.listing').find('li.list-item').find('.hide-button').click(function (event) {
        $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
        $(this).siblings('.show-button').show();
        $(this).hide();
    });
});
//# sourceMappingURL=itjakub.js.map
