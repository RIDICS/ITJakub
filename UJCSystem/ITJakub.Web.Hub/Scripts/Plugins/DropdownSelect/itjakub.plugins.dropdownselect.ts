/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />


class DropDownSelect {

    dropDownSelectContainer: string;
    showStar: boolean;

    constructor(dropDownSelectContainer: string, showStar: boolean) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.showStar = showStar;
        this.makeDropdown();
    }

    makeDropdown() {

        $(this.dropDownSelectContainer).empty();
        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");

        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);

        $(this.dropDownSelectContainer).append(dropDownDiv);
    }

    private makeHeader(dropDownDiv: HTMLDivElement) {

        var dropDownHeadDiv = document.createElement("div");
        $(dropDownHeadDiv).addClass("dropdown-select-header");

        var checkBoxSpan = document.createElement("span");
        $(checkBoxSpan).addClass("dropdown-select-checkbox");

        var checkbox = document.createElement("input");
        checkbox.type = "checkbox";

        checkBoxSpan.appendChild(checkbox);

        dropDownHeadDiv.appendChild(checkBoxSpan);

        var textSpan = document.createElement("span");
        $(textSpan).addClass("dropdown-select-text");
        textSpan.innerText = "Slovnínky o staré češtině";

        dropDownHeadDiv.appendChild(textSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("dropdown-select-more");

        $(moreSpan).click(function () {
            var body = $(this).parents(".dropdown-select").children(".dropdown-select-body");
            if (body.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                body.slideDown();
            } else {
                $(this).children().removeClass("glyphicon-chevron-up");
                $(this).children().addClass("glyphicon-chevron-down");
                body.slideUp();
            }
        });

        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");

        moreSpan.appendChild(iconSpan);

        dropDownHeadDiv.appendChild(moreSpan);

        dropDownDiv.appendChild(dropDownHeadDiv);
    }

    private makeBody(dropDownDiv: HTMLDivElement) {
        var dropDownBodyDiv = document.createElement("div");
        $(dropDownBodyDiv).addClass("dropdown-select-body");

        var filterDiv = document.createElement("div");
        $(filterDiv).addClass("dropdown-filter");

        var filterInput = document.createElement("input");
        $(filterInput).addClass("dropdown-filter-input");
        filterInput.placeholder = "Filtrovat podle názvu..";

        $(filterInput).keyup(function () {
            $(this).change();
        });

        $(filterInput).change(function () {
            if ($(this).val() == '') {
                $(this).parents(".dropdown-select-body").children(".concrete-item").show();
            } else {
                $(this).parents(".dropdown-select-body").children(".concrete-item").hide().filter(':contains(' + $(this).val() + ')').show();
            }
        });

        filterDiv.appendChild(filterInput);

        var filterClearSpan = document.createElement("span");
        $(filterClearSpan).addClass("dropdown-clear-filter glyphicon glyphicon glyphicon-remove-circle");

        $(filterClearSpan).click(function () {
            $(this).siblings(".dropdown-filter-input").val('').change();
        });

        filterDiv.appendChild(filterClearSpan);

        dropDownBodyDiv.appendChild(filterDiv);

        //TODO load cascades of childrens

        this.makeItem(dropDownBodyDiv, "slovn9k asaa");

        dropDownDiv.appendChild(dropDownBodyDiv);
    }

    private makeItem(dropDownBodyDiv: HTMLDivElement, name : string) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item");

        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";

        $(checkbox).click(function () {
            //TODO add item to search criteria
        });

        itemDiv.appendChild(checkbox);

        if (this.showStar) {

            var saveStarSpan = document.createElement("span");
            $(saveStarSpan).addClass("save-item glyphicon glyphicon-star-empty");

            $(saveStarSpan).click(function () {
                $(this).siblings(".delete-item").show();
                $(this).hide();
                //TODO populate request on save to favorites
            });

            itemDiv.appendChild(saveStarSpan);

            var deleteStarSpan = document.createElement("span");
            $(deleteStarSpan).addClass("delete-item glyphicon glyphicon-star");

            $(deleteStarSpan).click(function () {
                $(this).siblings(".save-item").show();
                $(this).hide();
                //TODO populate request on delete from favorites
            });

            itemDiv.appendChild(deleteStarSpan);
        }

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        nameSpan.innerText = name;

        itemDiv.appendChild(nameSpan);

        dropDownBodyDiv.appendChild(itemDiv);
    }
}