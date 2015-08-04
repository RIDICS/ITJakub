var SortBar = (function () {
    function SortBar(sortChangeCallback) {
        this.actualSortOrder = 1;
        this.actualSortOptionValue = 1 /* Title */;
        this.sortChangeCallback = sortChangeCallback;
    }
    SortBar.prototype.makeSortBar = function (booksContainer, sortBarContainer) {
        var _this = this;
        var sortBarDiv = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select = document.createElement('select');
        $(select).change(function () {
            var selectedOptionValue = $(sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected").val();
            _this.changeSortCriteria(parseInt(selectedOptionValue));
        });
        this.addOption(select, "Název", 1 /* Title */.toString());
        this.addOption(select, "Datace", 3 /* Dating */.toString());
        this.addOption(select, "Autor", 0 /* Author */.toString());
        this.addOption(select, "Editor", 2 /* Editor */.toString());
        sortBarDiv.appendChild(select);
        var sortOrderButton = document.createElement('button');
        sortOrderButton.type = 'button';
        $(sortOrderButton).addClass('btn btn-sm sort-button');
        var spanSortAsc = document.createElement('span');
        $(spanSortAsc).addClass('glyphicon glyphicon-arrow-up');
        sortOrderButton.appendChild(spanSortAsc);
        $(sortOrderButton).click(function (event) {
            _this.changeSortOrder();
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
        this.sortingChanged();
    };
    SortBar.prototype.changeSortCriteria = function (sortOptionValue) {
        this.actualSortOptionValue = sortOptionValue;
        this.sortingChanged();
    };
    SortBar.prototype.sortingChanged = function () {
        this.sortChangeCallback();
    };
    SortBar.prototype.addOption = function (selectbox, text, value) {
        var option = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
    };
    SortBar.prototype.isSortedAsc = function () {
        return this.actualSortOrder > 0;
    };
    SortBar.prototype.getSortCriteria = function () {
        return this.actualSortOptionValue;
    };
    return SortBar;
})();
/*
 *
 *
 * Mirroring of C# datacontract in namespace ITJakub.Shared.Contracts.Searching.Criteria
 *
 *
 * [DataContract]
    public enum SortEnum : short
    {
        [EnumMember]
        Author = 0,
        [EnumMember]
        Title = 1,
        [EnumMember]
        Editor = 2,
        [EnumMember]
        Dating = 3,
    }
 
 *
 *
 */
var SortEnum;
(function (SortEnum) {
    SortEnum[SortEnum["Author"] = 0] = "Author";
    SortEnum[SortEnum["Title"] = 1] = "Title";
    SortEnum[SortEnum["Editor"] = 2] = "Editor";
    SortEnum[SortEnum["Dating"] = 3] = "Dating";
})(SortEnum || (SortEnum = {}));
//# sourceMappingURL=itjakub.plugins.bibliography.sorting.js.map