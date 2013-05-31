$(document).ready(function () {
    $('.advanced-search-wrapper').advancedSearch();
    $("#search-results-alphabetical a.search-results-alphabetical-result").initLoadingAlphTermDetail();
    $("#results-type a.search-results-type-result").initLoadingTypeTermDetail();
});

function isBlankString(str) {
    return (!str || /^\s*$/.test(str));
}

var SelectedSources = function () {
    var selectedCategories = new Array();
    var selectedBooks = new Array();

    this.book = "book";
    this.category = "category";

    this.addSource = function (type, id, name) {
        if (type == this.category) {
            selectedCategories.push({
                id: id,
                name: name
            });
        }
        if (type == this.book) {
            selectedBooks.push({
                id: id,
                name: name
            });
        }
    };

    this.removeSource = function (type, id, name) {
        if (type == this.category) {
            for (var i = 0; i < selectedCategories.length; i++) {
                if (id == selectedCategories[i].id) {
                    selectedCategories.splice(i, 1);
                }
            }
        }
        if (type == this.book) {
            for (var i = 0; i < selectedBooks.length; i++) {
                if (id == selectedBooks[i].id) {
                    selectedBooks.splice(i, 1);
                }
            }
        }
    }

    this.getSelectedHtml = function () {
        var categoriesNames = new Array();
        for (var i = 0; i < selectedCategories.length; i++) {
            categoriesNames.push(selectedCategories[i].name);
        }
        var booksNames = new Array();
        for (var i = 0; i < selectedBooks.length; i++) {
            booksNames.push(selectedBooks[i].name);
        }

        if (categoriesNames.length == 0 && booksNames.length == 0) {
            return "<span class=\"muted\">Aktivní prohledávání ve všech dostupných dílech</span>";
        }

        var categoriesString = categoriesNames.join(", ");
        var booksString = booksNames.join(", ");

        if (categoriesNames.length == 0) {
            categoriesString = "-";
        }

        if (booksNames.length == 0) {
            booksString = "-";
        }

        return "<div class=\"muted\">" +
            "<strong>Kategorie:</strong> " + categoriesString + "<br>" +
            "<strong>Díla:</strong> " + booksString;
    };

    this.getSelectedUrlParam = function () {
        var categoriesIds = new Array();
        for (var i = 0; i < selectedCategories.length; i++) { 
            categoriesIds.push(selectedCategories[i].id); 
        }
        var booksIds = new Array();
        for (var i = 0; i < selectedBooks.length; i++) { 
            booksIds.push(selectedBooks[i].id); 
        }

        var categoriesString = categoriesIds.join("+");
        var booksString = booksIds.join("+");

        return "kategorie=" + categoriesString + "&books=" + booksString;
    };

    this.checkCheckboxes = function (selector) {
        var _this = this;
        selector.each(function () {
            _this.addSource($(this).attr("data-type"), $(this).attr("data-id"), $(this).attr("data-name"));
        });
        $(".advanced-search-wrapper .searched-books").html(this.getSelectedHtml());
    };

    this.uncheckCheckboxes = function (selector) {
        var _this = this;
        selector.each(function () {
            _this.removeSource($(this).attr("data-type"), $(this).attr("data-id"), $(this).attr("data-name"));
        });
        $(".advanced-search-wrapper .searched-books").html(this.getSelectedHtml());
    };

}

var selectedsources = new SelectedSources();

(function ($) {
    $.fn.extend({
        loadChildren: function (options) {

            var defaults = {
                categoryId: null,
                categoriesUrl: "/hledani/search-category-children"
            };

            var options = $.extend(defaults, options);

            function showLevel(iconElement) {
                iconElement.parent().find(" > ul.nav.show-categories").slideDown();
                iconElement.parent().find(" > div.category-select").slideDown();
                iconElement.attr("class", "icon-chevron-down");
                iconElement.unbind('click');
                iconElement.click(function () {
                    hideLevel(iconElement);
                });
            }

            function hideLevel(iconElement) {
                iconElement.parent().find(" > ul.nav.show-categories").slideUp();
                iconElement.parent().find(" > div.category-select").slideUp();
                iconElement.attr("class", "icon-chevron-right");
                iconElement.unbind('click');
                iconElement.click(function () {
                    showLevel(iconElement);
                });
            }

            return this.each(function () {
                var categoriesUrl = options.categoriesUrl;
                if (options.categoryId != null) {
                    categoriesUrl = categoriesUrl + "/" + options.categoryId;
                }
                
                var parentElement = $(this);

                if (parentElement.find(" > ul.nav").length > 0) {
                    showLevel(parentElement.find(" > i[class=icon-chevron-down]"));
                } else {
                    $.get(categoriesUrl, function (data) {
                        parentElement.append(data);

                        parentElement.find(".category-select input[type=text]").typeahead({
                            source: function () {
                                var children = new Array();
                                parentElement.find('> .nav input[type=checkbox]').each(function () {
                                    children.push($(this).attr('data-name'));
                                });
                                return children;
                            }
                        });
                        parentElement.find(".category-select form").submit(function () {
                            var selectedName = $(this).find('input[type=text]').val();
                            $(this).find('input[type=text]').val("");
                            var formEl = $(this);
                            $(this).parent().parent().find('input[type=checkbox]').each(function () {
                                if ($(this).attr("data-name") == selectedName) {
                                    $(this).prop("checked", true);
                                    selectedsources.checkCheckboxes($(this));
                                    formEl.parent().find('.selected-categories').append($(this).parent().parent());
                                }
                            });
                            return false;
                        });

                        parentElement.find("input[type=checkbox]").unbind("change");
                        parentElement.find("input[type=checkbox]").change(function () {
                            function uncheckParentIfAllChildrenUnchecked(chckbx) {
                                if (chckbx.parent().parent().parent().find("input[type=checkbox]").length > 0) {
                                    var allUnchecked = true;

                                    chckbx.parent().parent().parent().find("input[type=checkbox]").each(function () {
                                        if ($(this).is(":checked")) {
                                            allUnchecked = false;
                                        }
                                    });

                                    if (allUnchecked) {
                                        chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]").prop("checked", false);
                                        selectedsources.uncheckCheckboxes(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                        uncheckParentIfAllChildrenUnchecked(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                    }
                                }
                            }

                            if ($(this).is(":checked")) {
                                $(this).parent().parent().find("> label > input[type=checkbox]").each(function () {
                                        $(this).prop("checked", true);
                                        selectedsources.checkCheckboxes($(this));
                                });
                                $(this).parent().parent().find("ul.nav.show-categories > li > label > input[type=checkbox]").each(function () {
                                    if (!$(this).prop("checked")) {
                                        $(this).prop("checked", true);
                                        selectedsources.checkCheckboxes($(this));
                                    }
                                });
                            } else {
                                selectedsources.uncheckCheckboxes($(this));
                                if (!$(this).parent().parent().parent().is(".selected-categories")) {
                                    uncheckParentIfAllChildrenUnchecked($(this));
                                }
                                if ($(this).parent().parent().parent().is(".selected-categories")) {
                                    $(this).parent().parent().parent().parent().parent().find("> ul.nav").append($(this).parent().parent());
                                }
                            }
                        });

                        if (isBlankString(data)) {
                            parentElement.find(" > i[class=icon-chevron-right]").attr("class", "icon-chevron-down");
                            parentElement.find(" > i[class=icon-chevron-right]").unbind("click");
                        } else {
                            if (parentElement.find(" > i[class=icon-chevron-right]").length > 0) {
                                showLevel(parentElement.find(" > i[class=icon-chevron-right]"));
                            } else {
                                parentElement.find(" > ul.nav.show-categories").slideDown();
                                parentElement.find(" > div.category-select").slideDown();
                            }
                            parentElement.find(" > ul.nav i[class=icon-chevron-right]").click(function () {
                                $(this).parent().loadChildren({ categoriesUrl: options.categoriesUrl, categoryId: $(this).parent().attr("data-category-id") });
                            });
                        }
                    });
                }
            });
        }
    });
})(jQuery);


(function ($) {
    $.fn.extend({
        advancedSearch: function (options) {

            var defaults = {};

            var options = $.extend(defaults, options);

            var advancedSearchVisible = true;

            var categoriesUrl = null;

            function changeASVisibility(asElement) {
                if (advancedSearchVisible) {
                    asElement.find('.advanced-search').slideUp();
                    advancedSearchVisible = false;
                } else {
                    asElement.find('.advanced-search').slideDown();
                    advancedSearchVisible = true;
                }
            }

            return this.each(function () {
                var asElement = $(this);

                categoriesUrl = asElement.find(".categories").attr("data-categories-url");
                    
                asElement.find('.advanced-search').hide();
                asElement.find(".categories").loadChildren({ categoriesUrl: categoriesUrl });

                advancedSearchVisible = false;
                $('.show-advanced-search').click(function () {
                    changeASVisibility(asElement);
                });

                $(".advanced-search-wrapper form.search-form").submit(function () {
                    window.location.href = $(this).attr("action") + "?searchTerm=" + $(this).find("#search-term").val() + "&" + selectedsources.getSelectedUrlParam();
                    return false;
                });
            });
        }
    });
})(jQuery);


(function ($) {

    $.fn.extend({
        initLoadingAlphTermDetail: function (options) {

            var loadingHTML = "<div class=\"loading\"></div>";
            
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

            var loadingHTML = "<div class=\"loading\"></div>";
            
            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                element.click(function () {
                    element.parent().parent().find("li").removeClass("active");
                    element.parent().addClass("active");

                    $('#type-result-detail').html(loadingHTML);
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