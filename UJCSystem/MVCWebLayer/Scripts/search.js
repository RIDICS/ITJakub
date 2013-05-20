$(document).ready(function () {
    $('.advanced-search-wrapper').advancedSearch();
    $("#search-results-alphabetical a.search-results-alphabetical-result").initLoadingAlphTermDetail();
    $("#results-type a.search-results-type-result").initLoadingTypeTermDetail();
});


(function ($) {
    /* todo */
    $.fn.extend({
        advancedSearch: function (options) {

            var defaults = {};

            var options = $.extend(defaults, options);
            var advancedSearchVisible = true;

            function changeASVisibility(asElement) {
                if (advancedSearchVisible) {
                    asElement.find('.advanced-search').slideUp();
                    advancedSearchVisible = false;
                } else {
                    asElement.find('.advanced-search').slideDown();
                    advancedSearchVisible = true;
                }
            }

            var asCheckboxes = null;

            function createFindsHtml() {
                if (asCheckboxes == null) {
                    return "<span class=\"muted\">Aktivní prohledávání ve všech dostupných dílech</span>";
                }
                var html = "";
                $.each(asCheckboxes, function (key, value) {
                    html += "<div class=\"muted\">";
                    html += "<strong>";
                    html += value.label + " ";
                    html += "</strong>";
                    if (value.children == null) {
                        html += "<span>všechny</span>";
                    } else {
                        var a = Array.prototype.slice.call(value.children);
                        html += a.join(", ");
                    }
                    html += "</div>";
                });
                return html;
            }

            function defineCheckboxes(asElement) {
                asCheckboxes = null;
                asElement.find(".advanced-search .span6 > ul > li > label > input[type=checkbox]").each(function () {
                    var children = null;
                    $(this).parent().parent().find("ul li input[type=checkbox]:checked").each(function () {
                        if (children == null) {
                            children = new Array();
                        }
                        children.push($(this).parent().find("span").html());
                    });
                    if (!(children == null && !$(this).is(':checked'))) {
                        if (asCheckboxes == null) {
                            asCheckboxes = {};
                        }
                        asCheckboxes[$(this).val()] = {};
                        asCheckboxes[$(this).val()]["label"] = $(this).parent().find("span").html();
                        asCheckboxes[$(this).val()]["children"] = children;
                    }
                });
                asElement.find(".searched-books").html(createFindsHtml());
            }

            return this.each(function () {
                var asElement = $(this);

                asElement.find('.advanced-search').hide();
                /*advancedSearchVisible = false;
                $('.show-advanced-search').click(function () {
                    changeASVisibility(asElement);
                });

                asElement.find('.advanced-search input[type=checkbox]').click(function () {
                    defineCheckboxes(asElement);
                });*/
            });
        }
    });
})(jQuery);


(function ($) {

    $.fn.extend({
        initLoadingAlphTermDetail: function (options) {

            var loadingHTML = "<div class=\"progress progress-striped active search-progress\"><div class=\"bar\" style=\"width: 100%;\"></div></div>";
            
            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                element.click(function () {
                    element.parent().parent().find("li").removeClass("active");
                    element.parent().addClass("active");

                    $('#alphabetical-result-detail').html(loadingHTML);
                    $.get(element.attr("data-url"), function (data) {
                        $('#alphabetical-result-detail').html(data);
                        element.blur();
                    });
                    return false;
                });
            });
        }
    });
})(jQuery);


(function ($) {

    $.fn.extend({
        initLoadingTypeTermDetail: function (options) {

            var loadingHTML = "<div class=\"progress progress-striped active search-progress\"><div class=\"bar\" style=\"width: 100%;\"></div></div>";
            
            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                element.click(function () {
                    element.parent().parent().find("li").removeClass("active");
                    element.parent().addClass("active");

                    $('#alphabetical-result-detail').html(loadingHTML);
                    $.get(element.attr("data-url"), function (data) {
                        $('#type-result-detail').html(data);
                        element.blur();
                    });
                    return false;
                });
            });
        }
    });
})(jQuery);

    
/* var arrowUpImg = new Image();
arrowUpImg.src = "/Images/arrow-up.png";

var arrowDownImg = new Image();
arrowDownImg.src = "/Images/arrow.png";

var autocompleteStrings = [
         		 "Alexandreida. Zlomek budějovicko-muzejní",
                "Alexandreida. Zlomek budějovický",
    "Alexandreida. Zlomek budějovický druhý",
    "Alexandreida. Zlomek jindřichohradecký"
         		];

$(document).ready(function () {
    closeAdvancedSearch();

    $("#advanced-search .show").click(function () {
        showAdvancedSearch();
        return false;
    });

    $("#advanced-search-active .cancel-button").click(function () {
        closeAdvancedSearch();
    });

    $("#advanced-search-active input[type='checkbox']").click(function () {
        checkChildrenNodes(this);
        uncheckParentNodes(this);
    });

    $(".has-children .arrow").click(function () {
        showHideChildrenNodes(this);
    });

    $(".has-children").hover(function () {
        showArrow(this);
    }, function () {
        hideArrows(this);
    });

    $("#search-submit").click(function () {
        window.location = "results.html";
    });

    $(".autocomplete").autocomplete({
        source: autocompleteStrings,
        select: function (event, ui) {
            createNewCheckbox(ui.item.value);
        }
    });
});

function showAdvancedSearch() {
	$("#advanced-search").hide();
	$("#advanced-search-active").show();
}

function closeAdvancedSearch() {
	$("#advanced-search").show();
	$("#advanced-search-active").hide();
}

function showHideChildrenNodes(element) {
	var firstList = $(element).parent().find("ul").first();
	
	if($(element).parent().hasClass("opened")) {
		$(element).parent().removeClass("opened");
		// $(element).parent().find("input[type='checkbox']").first().removeAttr("checked");
		$(firstList).addClass("not-shown");
	} else {
		$(element).parent().addClass("opened");
		if($(firstList).hasClass("not-shown")) {
			$(firstList).removeClass("not-shown");
		}
		// $(element).parent().find("input[type='checkbox']").first().attr("checked", "checked");
	}
}

function showArrow(element) {
    if ($(element).hasClass("opened")) {
		$(element).find(".arrow").first().attr("src", arrowUpImg.src);
		$(element).find(".arrow").first().removeClass("hidden");
		$(element).addClass("hovered");
    } else {
		$(element).find(".arrow").first().attr("src", arrowDownImg.src);
		$(element).find(".arrow").first().removeClass("hidden");
		$(element).addClass("hovered");
	}
}

function hideArrows(element) {
	$(element).find(".arrow").first().addClass("hidden");
	$(element).removeClass("hovered");
}

function createNewCheckbox(value) {
	var html = "<li><input type='checkbox' checked='checked' name='SearchPart' value='aa' id='aa' /><label for='aa'>";
	html += value;
	html += "</label></li>";
	
	$(".autocomplete").parent().parent().prepend(html);
}

function loadTermDetail(element, searchTerm) {
    $("#alphabetical-results li a").removeClass("selected");
    $(element).addClass("selected");

    $('#result-detail-panel').load('/Search/Detail?searchTerm=' + searchTerm);
}

function bookDetail(element, searchTerm) {
    $("#type-results li a").removeClass("selected");
    $(element).addClass("selected");

    $('#result-detail-panel').load('/Search/DetailByType?book=' + encodeURIComponent(searchTerm));
}

function showTypeResults() {
    $(".type-button").addClass("active");
    $(".alphabetical-button").removeClass("active");

    $("#alphabetical-results").hide();
    $("#type-results").show();
}

function showAlphabeticalResults() {
    $(".type-button").removeClass("active");
    $(".alphabetical-button").addClass("active");

    $("#alphabetical-results").show();
    $("#type-results").hide();
    $("#alphabetical-results li:first a").click();
}

function checkChildrenNodes(element) {
    if ($(element).parent().hasClass('has-children') && $(element).is(":checked")) {
        $(element).parent().find("input[type='checkbox']").attr("checked", "checked");
    }
}

function uncheckParentNodes(element) {
    if (!$(element).is(":checked")) {
        $(element).parent().parents(".has-children").each(function () {
            $(this).find("input[type='checkbox']").first().removeAttr("checked");
        });
    }
}*/