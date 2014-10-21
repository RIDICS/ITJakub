/// <reference path="itjakub.plugins.bibliography.ts"/>
/// <reference path="itjakub.plugins.search.ts"/>

//sets state to main plugins menu
$(document).ready(function() {
    $('#main-plugins-menu').find('li').removeClass('active');
    var href = window.location.pathname;
    console.log(href.toString()); 
    var liTargetingActualPage = $('#main-plugins-menu').find("a[href='" + href.toString() + "']").parent('li');
    $(liTargetingActualPage).addClass('active');
    $(liTargetingActualPage).parents('li').addClass('active');
});



//TODO methods below should be on typescript component for creating listings
//list item showing hidden context
$(document).ready(function () {
    $('ul.listing').find('li.list-item').find('.show-button').click(function(event) {
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

$(document).ready(function () {
    var searchPlugin: SearchModule = new SearchModule();

    $('#fillBibList10').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 10; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds, '#bibliographyList');
    });

    $('#fillBibList3').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 3; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds, '#bibliographyList');
    });
    $('#fillBibList1000').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 1000; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds, '#bibliographyList');
    });


    $('#fillBibListTypeEdition').click(function () {
        searchPlugin.getBookWithType('Edition', '#bibliographyList');
    });
    $('#fillBibListTypeDictionary').click(function () {
        searchPlugin.getBookWithType('Dictionary', '#bibliographyList');
    });

});