/// <reference path="Plugins/Bibliography/itjakub.plugins.bibliography.ts"/>
/// <reference path="Plugins/Search/itjakub.plugins.search.ts"/>
/// <reference path="Plugins/DropdownSelect/itjakub.plugins.dropdownselect.ts"/>


//sets state to main plugins menu
$(document).ready(() => {
    $('#main-plugins-menu').find('li').removeClass('active');
    var href = window.location.pathname;
    var liTargetingActualPage = $('#main-plugins-menu').find("a[href='" + href.toString() + "']").parent('li');
    $(liTargetingActualPage).addClass('active');
    $(liTargetingActualPage).parents('li').addClass('active');
});

function getQueryStringParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function getBaseUrl() {
    var baseUrl = $("#baseUrl").data("path");
    return baseUrl;
}

// jQuery case-insensitive contains
jQuery.expr[':'].containsCI = (a, i, m) => (jQuery(a).text().toLowerCase()
    .indexOf(m[3].toLowerCase()) >= 0);