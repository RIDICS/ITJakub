class TermEditor {
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private pageId: number;
    private selectedTerms: Array<ITermContract>;
    private addTermsDialog: JQuery;
    private termListContainer: JQuery;
    private saveTermsCallback: () => void;
    private selectedTermsSelector = "#selectedTerms";

    constructor(saveTermsCallback: () => void = null) {
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
        this.saveTermsCallback = saveTermsCallback;
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
                this.setTermCheckbox(term.id, true);
            }
            
            this.addTermsDialog.modal();
        });
        
        $(".save-terms").on("click", () => {
            const alertHolder = this.addTermsDialog.find(".alert-holder");
            alertHolder.empty();
            this.client.setTerms(this.pageId, this.selectedTerms.map(t => t.id)).done(() => {
                this.addTermsDialog.modal("hide");
                if (this.saveTermsCallback !== null) {
                    this.saveTermsCallback.call(this.saveTermsCallback);
                }
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                alertHolder.empty().append(alert);
            }); 
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
            const termId = termRow.data("term-id") as number;
            
            if(checkbox.is(":checked")) {
                this.selectedTerms.push({
                    id: termId,
                    name: termRow.data("term-name"),
                    categoryId: termRow.data("term-category-id"),
                    position: termRow.data("term-position"),
                });
            } else {
                this.removeTerm(termId);
            }

            this.renderSelectedTerms();
        });
    }
    
    setPageId(pageId: number) {
        this.pageId = pageId;    
    }
    private removeTerm(termId: number) {
        for( let i = 0; i < this.selectedTerms.length; i++){
            if ( this.selectedTerms[i].id === termId) {
                this.selectedTerms.splice(i, 1);
                i--;
            }
        }
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
        $(this.selectedTermsSelector).empty();
        for (let term of this.selectedTerms)
        {
            this.renderSelectedTerm(term);
        }
        
        $(".selected-term span").on("click", (event) => {
            const termId = $(event.currentTarget).parent(".selected-term").data("id") as number;
            
            this.setTermCheckbox(termId, false);
            
            this.removeTerm(termId);
            this.renderSelectedTerms();
        });
    }
    
    private setTermCheckbox(termId: number, isChecked: boolean) {
        const checkbox = this.termListContainer.find(`input[name="term-${termId}"]`);
        checkbox.prop("checked", isChecked);
    }
    
    private renderSelectedTerm(term: ITermContract)
    {
        $(this.selectedTermsSelector).append(`<div data-id="${term.id}" class="selected-term">${term.name} <span><i class="fa fa-minus"></i></span></div>`);
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