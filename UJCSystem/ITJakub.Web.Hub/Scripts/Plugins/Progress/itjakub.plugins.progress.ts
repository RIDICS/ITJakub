class Progress {
    printModal:HTMLDivElement;
    $printModal:JQuery;

    modalDialog: HTMLDivElement;
    modalBody: HTMLDivElement;

    protected created=false;

    constructor(protected containerId: string, protected title: string, protected contentConfiguration: IProgressConfiguration = {}) {
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
            }
            if (this.contentConfiguration.body.showLoading !== undefined && this.contentConfiguration.body.showLoading) {
                const modalLoading = document.createElement("div");
                modalLoading.classList.add("loading", "loading-modal");

                this.modalBody.appendChild(modalLoading);
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
    }

    update(value: number, max: number) {

    }

    hide() {
        this.$printModal.modal("hide");
    }
}

interface IProgressConfiguration {
    body?: {
        title?: string;
        showLoading?:boolean;
    };
}