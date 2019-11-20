class TermEditor {
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private selectedTerms: Array<ITermContract>;
    private addTermsDialog: JQuery;
    private termListContainer: JQuery;

    constructor() {
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
    }

    init() {
        this.addTermsDialog = $("#addTermsDialog");
        this.termListContainer = this.addTermsDialog.find("#termsList");

        $(".manage-terms-button").on("click", () => {
            this.selectedTerms = this.parseTerms();
            this.renderSelectedTerms();
            
            const checkboxes = this.termListContainer.find(`input[type="checkbox"]`);
            checkboxes.prop("checked", false);
            
            for (let term of this.selectedTerms){
                const checkbox = this.termListContainer.find(`input[name="term-${term.id}"]`);
                checkbox.prop("checked", true);
            }
            
            this.addTermsDialog.modal();
        });
        
        const termCategoryFilter = $("#termCategoryFilter");
        termCategoryFilter.on("change", () => {
            const categoryId = termCategoryFilter.val();
            if (categoryId !== "") {
                this.termListContainer.find(`.panel[data-category-id="${termCategoryFilter.val()}"]`).removeClass("hide");
                this.termListContainer.find(`.panel:not([data-category-id="${termCategoryFilter.val()}"])`).addClass("hide");
            } else {
                this.termListContainer.find(`.panel`).removeClass("hide");
            }
        });

        const searchTerm = $("#searchTerm");
        searchTerm.on("keyup", () => {
            this.searchTerm(searchTerm.val() as string);
        });
        
        this.addTermsDialog.find(".reset-search-button").on("click", () => {
            searchTerm.val("");
            this.showAllTerms();
        });

        this.termListContainer.find(".ridics-checkbox").on("change", (event) => {
            const checkbox = $(event.currentTarget).find(`input[type="checkbox"]`);
            const termRow = checkbox.parents(".term-row");

            if(checkbox.is(":checked")) {
                this.selectedTerms.push({
                    id: termRow.data("term-id"),
                    name: termRow.data("term-name"),
                    categoryId: termRow.data("term-category-id"),
                    position: termRow.data("term-position"),
                });
            } else {
                const termId = termRow.data("term-id") as number;
                for( let i = 0; i < this.selectedTerms.length; i++){
                    if ( this.selectedTerms[i].id === termId) {
                        this.selectedTerms.splice(i, 1);
                        i--;
                    }
                }
            }

            this.renderSelectedTerms();
        });
    }
    
    private searchTerm(searchedValue: string) {
        if (searchedValue == "") {
            this.showAllTerms();
        } else {
            searchedValue = searchedValue.toLowerCase();
            for (let term of this.termListContainer.find(`.panel .checkbox-label`).toArray()) {
                const termLabel = ($(term).text() as string).toLowerCase();
                const termCheckbox = $(term).parents(".ridics-checkbox");
                if (termLabel.indexOf(searchedValue) !== -1) {
                    termCheckbox.removeClass("hide");
                } else {
                    termCheckbox.addClass("hide");
                }
            }
        }
    }
    
    private renderSelectedTerms() {
        $("#selectedTerms").empty();
        for (let term of this.selectedTerms)
        {
            this.renderSelectedTerm(term);
        }
    }
    
    private renderSelectedTerm(term: ITermContract)
    {
        $("#selectedTerms").append(`<div data-id="${term.id}">${term.name}</div>`)
    }

    private showAllTerms() {
        this.termListContainer.find(`.panel .ridics-checkbox`).removeClass("hide");
    }

    private parseTerms(): ITermContract[] {
        const terms = [];
        for(let term of $(".content-terms .term-row").toArray())
        {
            terms.push({
                id: $(term).data("term-id"),
                name: $(term).data("term-name"),
                categoryId: $(term).data("term-category-id"),
                position: $(term).data("term-position"),
            });
        }
        return terms;
    }
}