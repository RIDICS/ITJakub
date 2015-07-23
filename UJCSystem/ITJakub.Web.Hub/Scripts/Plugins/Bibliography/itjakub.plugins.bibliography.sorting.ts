class SortBar {
    comparatorResolver: ComparatorResolver;
    actualSortOrder: number;
    actualSortOptionValue: SortEnum;


    constructor() {
        this.comparatorResolver = new ComparatorResolver();
        this.actualSortOrder = 1;
        this.actualSortOptionValue = SortEnum.Title;
    }

    public makeSortBar(booksContainer: string, sortBarContainer: string): HTMLDivElement {
        var sortBarDiv: HTMLDivElement = document.createElement('div');
        $(sortBarDiv).addClass('bib-sortbar');
        var select: HTMLSelectElement = document.createElement('select');
        $(select).change(() => {
            var selectedOptionValue:string = $(sortBarContainer).find('div.bib-sortbar').find('select').find("option:selected").val();
            this.actualSortOptionValue = parseInt(selectedOptionValue);
            var comparator = this.comparatorResolver.getComparatorForOptionValue(selectedOptionValue);
            this.sort(comparator, this.actualSortOrder, booksContainer);
        });
        this.addOption(select, "Název", SortEnum.Title.toString());
        this.addOption(select, "Datace", SortEnum.Dating.toString()); //TODO add options to json config
        this.addOption(select, "Autor", SortEnum.Author.toString());
        this.addOption(select, "Editor", SortEnum.Editor.toString());
        sortBarDiv.appendChild(select);

        var sortOrderButton: HTMLButtonElement = document.createElement('button');
        sortOrderButton.type = 'button';
        $(sortOrderButton).addClass('btn btn-sm sort-button');
        var spanSortAsc: HTMLSpanElement = document.createElement('span');
        $(spanSortAsc).addClass('glyphicon glyphicon-arrow-up');
        sortOrderButton.appendChild(spanSortAsc);
        $(sortOrderButton).click((event) => {
            this.changeSortOrder();
            var comparator = this.comparatorResolver.getComparatorForOptionValue(this.actualSortOptionValue.toString());
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

    public isSortedAsc(): boolean {
        return this.actualSortOrder > 0;
    }

    public getSortCriteria(): SortEnum {
        return this.actualSortOptionValue; //TODO make enum
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

enum SortEnum 
{
    Author = 0,
    Title = 1,
    Editor = 2,
    Dating = 3,
}