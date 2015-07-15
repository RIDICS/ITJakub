var DropDownSelect2 = (function () {
    function DropDownSelect2(dropDownSelectContainer, dataUrl, showStar) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
    }
    DropDownSelect2.prototype.makeDropDown = function () {
        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");
        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);
        $(this.dropDownSelectContainer).append(dropDownDiv);
    };
    DropDownSelect2.prototype.makeHeader = function (dropDownDiv) {
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
        textSpan.innerText = ""; //TODO read from parameter when root is not unique or is not description
        dropDownHeadDiv.appendChild(textSpan);
        var loadSpan = document.createElement("span");
        $(loadSpan).addClass("dropdown-select-text-loading");
        dropDownHeadDiv.appendChild(loadSpan);
        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("dropdown-select-more");
        $(moreSpan).click(function () {
            var body = $(this).parents(".dropdown-select").children(".dropdown-select-body");
            if (body.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                body.slideDown();
            }
            else {
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
    DropDownSelect2.prototype.makeBody = function (dropDownDiv) {
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
                $(this).parents(".dropdown-select-body").find(".concrete-item").show();
            }
            else {
                $(this).parents(".dropdown-select-body").find(".concrete-item").hide();
                $(this).parents(".dropdown-select-body").find(".concrete-item-name").filter(":containsCI(" + $(this).val() + ")").parents(".concrete-item").show();
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
    DropDownSelect2.prototype.downloadData = function (dropDownItemsDiv) {
        var _this = this;
        this.books = {};
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
                _this.processDownloadedData(response);
            }
        });
    };
    DropDownSelect2.prototype.processDownloadedData = function (result) {
        //self.type = this.getType(response);
        //var categories = this.getCategories(response);
        //var items = this.getLeafItems(response);
        //$(dropDownItemsDiv).children("div.loading").remove();
        //this.makeTreeStructure(categories, items, dropDownItemsDiv);
        this.books = {};
        this.categories = {};
        this.rootCategories = [];
        for (var i = 0; i < result.Categories.length; i++) {
            var resultCategory = result.Categories[i];
            var category = new DropDownCategory();
            category.id = resultCategory.Id;
            category.name = resultCategory.Description;
            category.parentCategoryId = resultCategory.ParentCategoryId;
            category.bookIds = [];
            category.subcategoryIds = [];
            this.categories[category.id] = category;
            if (!category.parentCategoryId)
                this.rootCategories.push(category);
        }
        for (var j = 0; j < result.Books.length; j++) {
            var resultBook = result.Books[j];
            var book = new DropDownBook();
            book.id = resultBook.Id;
            book.name = resultBook.Title;
            book.categoryIds = resultBook.CategoryIds;
            this.books[book.id] = book;
            for (var k = 0; k < book.categoryIds.length; k++) {
                var categoryId = book.categoryIds[k];
                this.categories[categoryId].bookIds.push(book.id);
            }
        }
    };
    DropDownSelect2.prototype.getState = function () {
        return new State();
    };
    return DropDownSelect2;
})();
var DropDownBook = (function () {
    function DropDownBook() {
    }
    return DropDownBook;
})();
var DropDownCategory = (function () {
    function DropDownCategory() {
    }
    return DropDownCategory;
})();
//# sourceMappingURL=itjakub.plugins.dropdownselect2.js.map