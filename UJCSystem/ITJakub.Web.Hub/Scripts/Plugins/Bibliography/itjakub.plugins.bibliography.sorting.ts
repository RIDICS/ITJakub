class SortBar {
    comparatorResolver: ComparatorResolver;
    actualSortOrder: number;
    actualSortOptionValue: string;


    constructor() {
        this.comparatorResolver = new ComparatorResolver();
        this.actualSortOrder = 1;
        this.actualSortOptionValue = "bookid"; //TODO get default sort option from config
    }

    public makeSortBar(booksContainer: string, sortBarContainer: string): HTMLDivElement {
        var sortBarDiv: HTMLDivElement = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select: HTMLSelectElement = document.createElement('select');
        $(select).change(() => {
            var selectedOptionValue = $(sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected").val();
            this.actualSortOptionValue = selectedOptionValue;
            var comparator = this.comparatorResolver.getComparatorForOptionValue(selectedOptionValue);
            this.sort(comparator, this.actualSortOrder, booksContainer);
        });
        this.addOption(select, "Název", "name");
        this.addOption(select, "Id", "bookid");
        this.addOption(select, "Datace", "century"); //TODO add options to json config
        this.addOption(select, "Typ", "booktype");
        sortBarDiv.appendChild(select);

        var sortOrderButton: HTMLButtonElement = document.createElement('button');
        sortOrderButton.type = 'button';
        $(sortOrderButton).addClass('btn btn-sm sort-button');
        var spanSortAsc: HTMLSpanElement = document.createElement('span');
        $(spanSortAsc).addClass('glyphicon glyphicon-arrow-up');
        sortOrderButton.appendChild(spanSortAsc);
        $(sortOrderButton).click((event) => {
            this.changeSortOrder();
            var comparator = this.comparatorResolver.getComparatorForOptionValue(this.actualSortOptionValue);
            this.sort(comparator, this.actualSortOrder, booksContainer);
            $(event.currentTarget).children('span').toggleClass('glyphicon-arrow-up glyphicon-arrow-down');
        });
        sortBarDiv.appendChild(sortOrderButton);

        return sortBarDiv;
    }

    private sort(comparator: (a: HTMLLIElement, b: HTMLLIElement) => number, order: number, booksContainer: string) {
        var elems: Array<HTMLLIElement> = <Array<HTMLLIElement>><any>$(booksContainer).children('ul.bib-listing').children('li').detach();
        var sortFunction = (a, b) => { return order * comparator(a, b); };
        elems.sort(sortFunction);
        $(booksContainer).children('ul.bib-listing').append(elems);
    }

    private changeSortOrder() {
        this.actualSortOrder = -this.actualSortOrder;
    }

    private addOption(selectbox: HTMLSelectElement, text: string, value: string) {
        var option: HTMLOptionElement = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
    }
}


class ComparatorResolver {
    private comparators: { (a: HTMLLIElement, b: HTMLLIElement): number; }[];

    constructor() {
        this.comparators = new Array();
        this.comparators['Default'] = (value: string) => {
            return (a: HTMLLIElement, b: HTMLLIElement) => {
                var aval = $(a).data(value);
                var bval = $(b).data(value);
                if (aval == bval) {         //TODO to keep sort order compare by unique value, shoul be loaded from config
                    aval = $(a).data("bookid");
                    bval = $(b).data("bookid");
                }
                return aval > bval ? 1 : -1;
            };
        };
    }

    public getComparatorForOptionValue(optionValue: string): (a: HTMLLIElement, b: HTMLLIElement) => number {
        if (typeof this.comparators[optionValue] !== 'undefined') {
            return this.comparators[optionValue](optionValue);
        }
        return this.comparators['Default'](optionValue);
    }
}