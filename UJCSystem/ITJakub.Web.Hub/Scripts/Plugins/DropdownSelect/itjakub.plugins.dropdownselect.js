/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var DropDownSelect = (function () {
    function DropDownSelect(dropDownSelectContainer, dataUrl, showStar) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
    }
    DropDownSelect.prototype.makeDropdown = function () {
        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");

        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);

        $(this.dropDownSelectContainer).append(dropDownDiv);
    };

    DropDownSelect.prototype.makeHeader = function (dropDownDiv) {
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
        textSpan.innerText = ""; //TODO read from parameter

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
    };

    DropDownSelect.prototype.makeBody = function (dropDownDiv) {
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
            if ($(this).val() == "") {
                $(this).parents(".dropdown-select-body").children(".concrete-item").show();
            } else {
                $(this).parents(".dropdown-select-body").children(".concrete-item").hide().filter(":contains(" + $(this).val() + ")").show();
            }
        });

        filterDiv.appendChild(filterInput);

        var filterClearSpan = document.createElement("span");
        $(filterClearSpan).addClass("dropdown-clear-filter glyphicon glyphicon glyphicon-remove-circle");

        $(filterClearSpan).click(function () {
            $(this).siblings(".dropdown-filter-input").val("").change();
        });

        filterDiv.appendChild(filterClearSpan);

        dropDownBodyDiv.appendChild(filterDiv);

        dropDownDiv.appendChild(dropDownBodyDiv);

        this.downloadData(dropDownBodyDiv);
    };

    DropDownSelect.prototype.downloadData = function (dropDownItemsDiv) {
        var _this = this;
        var loadDiv = document.createElement("div");
        $(loadDiv).addClass("loading");
        $(dropDownItemsDiv).append(loadDiv);

        $.ajax({
            type: "GET",
            traditional: true,
            data: {},
            url: this.dataUrl,
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var categories = response["categories"];
                var books = response["books"];

                $(dropDownItemsDiv).children("div.loading").remove();

                _this.makeTreeStructure(categories, books, dropDownItemsDiv);
            }
        });
    };

    DropDownSelect.prototype.makeTreeStructure = function (categories, books, dropDownItemsDiv) {
        var rootCategory = categories[""][0];

        var textSpan = $(dropDownItemsDiv).parent().children(".dropdown-select-header").children(".dropdown-select-text");
        $(textSpan).append(rootCategory["Description"]);

        var childCategories = categories[rootCategory["Id"]];
        var childBooks = books[rootCategory["Id"]];

        if (typeof (childCategories) !== "undefined") {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(dropDownItemsDiv, childCategory, categories, books);
            }
        }

        if (typeof (childBooks) !== "undefined") {
            for (var i = 0; i < childBooks.length; i++) {
                var childBook = childBooks[i];
                this.makeBookItem(dropDownItemsDiv, childBook);
            }
        }
    };

    DropDownSelect.prototype.makeCategoryItem = function (container, currentCategory, categories, books) {
        //TODO create divs with data and append to container
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-id, data-item-name, data-item-type, data-item-is-favorite

        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";

        $(checkbox).click(function () {
            //TODO add item to search criteria
        });

        itemDiv.appendChild(checkbox);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("concrete-item-more");

        $(moreSpan).click(function () {
            var childsDiv = $(this).closest(".concrete-item").children(".child-items");
            if (childsDiv.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");

                //if ($(childsDiv).children().length > 0) {
                childsDiv.slideDown();
                //}
            } else {
                $(this).children().removeClass("glyphicon-chevron-up");
                $(this).children().addClass("glyphicon-chevron-down");
                childsDiv.slideUp();
            }
        });

        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");

        moreSpan.appendChild(iconSpan);
        itemDiv.appendChild(moreSpan);

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
        nameSpan.innerText = currentCategory["Description"];
        itemDiv.appendChild(nameSpan);

        var childsDiv = document.createElement("div");
        $(childsDiv).addClass("child-items");
        itemDiv.appendChild(childsDiv);

        container.appendChild(itemDiv);

        var childCategories = categories[currentCategory["Id"]];
        var childBooks = books[currentCategory["Id"]];

        if (typeof (childCategories) !== "undefined") {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(childsDiv, childCategory, categories, books);
            }
        }

        if (typeof (childBooks) !== "undefined") {
            for (var i = 0; i < childBooks.length; i++) {
                var childBook = childBooks[i];
                this.makeBookItem(childsDiv, childBook);
            }
        }
    };

    DropDownSelect.prototype.makeBookItem = function (container, currentBook) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-id, data-item-name, data-item-type, data-item-is-favorite

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
        nameSpan.innerText = currentBook["Title"];

        itemDiv.appendChild(nameSpan);

        container.appendChild(itemDiv);
    };
    return DropDownSelect;
})();
//# sourceMappingURL=itjakub.plugins.dropdownselect.js.map
