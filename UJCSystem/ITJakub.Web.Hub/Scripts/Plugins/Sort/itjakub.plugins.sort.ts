class SortBar {
    private actualSortOrder: number;
    private actualSortOptionValue: SortEnum;
    private sortChangeCallback: () => void;


    constructor(sortChangeCallback: () => void) {
        this.actualSortOrder = 1;
        this.actualSortOptionValue = SortEnum.Title;
        this.sortChangeCallback = sortChangeCallback;
    }

    public makeSortBar(sortBarContainer: string): HTMLDivElement {
        var sortBarDiv: HTMLDivElement = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');

        var select: HTMLSelectElement = document.createElement('select');

        $(select).change(() => {
            var selectedOptionValue: string = $(sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected").val();
            this.changeSortCriteria(parseInt(selectedOptionValue));
        });

        this.addOption(select, "Název", SortEnum.Title.toString());
        this.addOption(select, "Datace", SortEnum.Dating.toString());
        this.addOption(select, "Autor", SortEnum.Author.toString());

        sortBarDiv.appendChild(select);

        var sortOrderButton: HTMLButtonElement = document.createElement('button');
        sortOrderButton.type = 'button';
        $(sortOrderButton).addClass('btn btn-sm sort-button');

        var spanSortAsc: HTMLSpanElement = document.createElement('span');
        $(spanSortAsc).addClass('glyphicon glyphicon-arrow-up');
        sortOrderButton.appendChild(spanSortAsc);

        $(sortOrderButton).click((event) => {
            this.changeSortOrder();
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
        this.sortingChanged();
    }

    private changeSortCriteria(sortOptionValue: SortEnum) {
        this.actualSortOptionValue = sortOptionValue;
        this.sortingChanged();
    }

    private sortingChanged() {
        this.sortChangeCallback();
    }

    private addOption(selectbox: HTMLSelectElement, text: string, value: string) {
        var option: HTMLOptionElement = document.createElement('option');
        option.text = text;
        option.value = value;
        selectbox.appendChild(option);
    }

    public isSortedAsc(): boolean {
        return this.actualSortOrder > 0;
    }

    public getSortCriteria(): SortEnum {
        return this.actualSortOptionValue;
    }
}

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
        Dating = 2,
    }
 
 *
 * 
 */

enum SortEnum {
    Author = 0,
    Title = 1,
    Dating = 2,
}