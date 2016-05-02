class Progress {
    printModal: HTMLDivElement;
    $printModal: JQuery;

    modalDialog: HTMLDivElement;
    modalBody: HTMLDivElement;

    protected created = false;
    protected updateableField=null;

    constructor(protected containerId: string,
        protected title: string,
        protected contentConfiguration: IProgressConfiguration = {}) {
        this.printModal = document.createElement("div");
        this.printModal.id = this.containerId;
        this.printModal.classList.add("modal", "fade");
        document.body.appendChild(this.printModal);

        this.$printModal = $(this.printModal);
    }

    private create() {
        this.modalDialog = document.createElement("div");
        this.modalDialog.classList.add("modal-dialog");

        const modalContent = document.createElement("div");
        modalContent.classList.add("modal-content");

        this.modalDialog.appendChild(modalContent);

        const modalHeader = document.createElement("div");
        modalHeader.classList.add("modal-header");

        modalContent.appendChild(modalHeader);

        const modalTitle = document.createElement("h2");
        modalTitle.classList.add("modal-title");
        modalTitle.innerHTML = this.title;

        modalHeader.appendChild(modalTitle);

        this.modalBody = document.createElement("div");
        this.modalBody.classList.add("modal-body");

        modalContent.appendChild(this.modalBody);

        if (this.contentConfiguration.body !== undefined) {
            if (this.contentConfiguration.body.title !== undefined) {
                const bodyTitle = document.createElement("span");
                bodyTitle.innerHTML = this.contentConfiguration.body.title;

                this.modalBody.appendChild(bodyTitle);

                if (this.contentConfiguration.update.field === ProgressUpdateField.BodyTitle) {
                    this.updateableField = bodyTitle;
                }
            }
            if (this.contentConfiguration.body
                .showLoading !==
                undefined &&
                this.contentConfiguration.body.showLoading) {
                const modalLoading = document.createElement("div");
                modalLoading.classList.add("loading", "loading-modal");

                this.modalBody.appendChild(modalLoading);
            }
            if (this.contentConfiguration.body.afterLoadingText !== undefined) {
                const afterLoadingText = document.createElement("span");
                afterLoadingText.innerHTML = this.contentConfiguration.body.afterLoadingText;

                this.modalBody.appendChild(afterLoadingText);

                if (this.contentConfiguration.update.field === ProgressUpdateField.BodyAfterLoading) {
                    this.updateableField = afterLoadingText;
                }
            }
        }

        this.printModal.appendChild(this.modalDialog);

        this.$printModal.modal({
            backdrop: "static"
        });

        this.created = true;
    }

    show() {
        if (!this.created) {
            this.create();
        }

        this.$printModal.modal("show");

        return this;
    }

    update(value: number, max: number) {
        if (this.updateableField !== null && this.contentConfiguration.update.valueCallback !== undefined) {
            this.updateableField.innerHTML = this.contentConfiguration.update.valueCallback(value, max);
        }

        return this;
    }

    hide() {
        this.$printModal.modal("hide");

        return this;
    }
}

interface IProgressConfiguration {
    body?: {
        title?: string;
        showLoading?: boolean;
        afterLoadingText?: string;
    };
    update?: {
        field?: ProgressUpdateField;
        valueCallback?: (value: number, max:number)=>string;
    }
}

enum ProgressUpdateField {
    BodyTitle,
    BodyAfterLoading
}
