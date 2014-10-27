var SortBar = (function () {
    function SortBar() {
        this.comparatorResolver = new ComparatorResolver();
        this.actualSortOrder = 1;
        this.actualSortOptionValue = "bookid"; //TODO get default sort option from config
    }
    SortBar.prototype.makeSortBar = function (booksContainer, sortBarContainer) {
        var _this = this;
        var sortBarDiv = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select = document.createElement('select');
        $(select).change(function () {
            var selectedOptionValue = $(sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected").val();
            _this.actualSortOptionValue = selectedOptionValue;
            var comparator = _this.comparatorResolver.getComparatorForOptionValue(selectedOptionValue);
            _this.sort(comparator, _this.actualSortOrder, booksContainer);
        });
        this.addOption(select, "Název", "name");
        this.addOption(select, "Id", "bookid");
        this.addOption(select, "Datace", "century"); //TODO add options to json config
        this.addOption(select, "Typ", "booktype");
        sortBarDiv.appendChild(select);

        var sortOrderButton = document.createElement('button');
        sortOrderButton.type = 'button';
        $(sortOrderButton).addClass('btn btn-sm sort-button');
        var spanSortAsc = document.createElement('span');
        $(spanSortAsc).addClass('glyphicon glyphicon-arrow-up');
        sortOrderButton.appendChild(spanSortAsc);
        $(sortOrderButton).click(function (event) {
            _this.changeSortOrder();
            var comparator = _this.comparatorResolver.getComparatorForOptionValue(_this.actualSortOptionValue);
            _this.sort(comparator, _this.actualSortOrder, booksContainer);
            $(event.currentTarget).children('span').toggleClass('glyphicon-arrow-up glyphicon-arrow-down');
        });
        sortBarDiv.appendChild(sortOrderButton);

        return sortBarDiv;
    };

    SortBar.prototype.sort = function (comparator, order, booksContainer) {
        var elems = $(booksContainer).children('ul.bib-listing').children('li').detach();
        var sortFunction = function (a, b) {
            return order * comparator(a, b);
        };
        elems.sort(sortFunction);
        $(booksContainer).children('ul.bib-listing').append(elems);
    };

    SortBar.prototype.changeSortOrder = function () {
        this.actualSortOrder = -this.actualSortOrder;
    };

    SortBar.prototype.addOption = function (selectbox, text, value) {
        var option = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
    };
    return SortBar;
})();

var ComparatorResolver = (function () {
    function ComparatorResolver() {
        this.comparators = new Array();
        this.comparators['Default'] = function (value) {
            return function (a, b) {
                var aval = $(a).data(value);
                var bval = $(b).data(value);
                if (aval == bval) {
                    aval = $(a).data("bookid");
                    bval = $(b).data("bookid");
                }
                return aval > bval ? 1 : -1;
            };
        };
    }
    ComparatorResolver.prototype.getComparatorForOptionValue = function (optionValue) {
        if (typeof this.comparators[optionValue] !== 'undefined') {
            return this.comparators[optionValue](optionValue);
        }
        return this.comparators['Default'](optionValue);
    };
    return ComparatorResolver;
})();
//# sourceMappingURL=itjakub.plugins.bibliography.sorting.js.map
