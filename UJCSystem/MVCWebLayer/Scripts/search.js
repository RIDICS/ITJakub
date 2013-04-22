var arrowUpImg = new Image();
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
}