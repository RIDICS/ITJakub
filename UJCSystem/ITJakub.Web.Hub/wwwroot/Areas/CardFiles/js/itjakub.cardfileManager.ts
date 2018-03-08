class CardFileManager {

    private cardFilesContainer: string;
    private bucketsCache: Array<Bucket>;

    constructor(cardFilesContainer: string) {
        this.cardFilesContainer = cardFilesContainer;
        this.bucketsCache = new Array<Bucket>();
     
    }

    public makeCardFile(cardFileId: string, cardFileName: string, bucketId: string, bucketName: string,  initCardPosition?: number) {
        var bucket = this.getBucket(cardFileId, bucketId, bucketName);
        var cardFile = new CardFileViewer(cardFileId, cardFileName, bucket, initCardPosition);
        cardFile.onErrorCallback = (cardFileViewer: CardFileViewer) => { this.moveViewerWithErrorOnTheEnd(cardFileViewer) };
        $(this.cardFilesContainer).append(cardFile.getHtml());
    }

    private moveViewerWithErrorOnTheEnd(cardFile: CardFileViewer) {
        var cardFileHtml = cardFile.getHtml();
        $(cardFileHtml).detach();
        $(this.cardFilesContainer).append(cardFile.getHtml());
    }

    private getKeyByCardFileAndBucket(cardFileId: string, bucketId: string): string {
        return cardFileId + bucketId;
    }

    private cacheContainsKey(key: string): boolean {
        var item = this.bucketsCache[key];
        return typeof item !== "undefined";
    }

    private getBucket(cardFileId : string, bucketId: string, bucketName: string): Bucket {
        var key = this.getKeyByCardFileAndBucket(cardFileId, bucketId);
        if (!this.cacheContainsKey(key)) {
            var bucket = new Bucket(cardFileId, bucketId, bucketName);
            this.bucketsCache[key] = bucket;
        }
        return this.bucketsCache[key];
    }

    public clearContainer() {
        $(this.cardFilesContainer).empty();
        this.emptyCache();
    }

    private emptyCache() {
        this.bucketsCache.length = 0;
        this.bucketsCache = new Array<Bucket>();
    }
}

class CardFileViewer {

    private htmlBody: HTMLDivElement;

    private cardFileId: string;
    private cardFileName: string;
    private cardFileDescription: string;

    private bucketsCount: number;
    private actualBucket: Bucket;

    private actualCardPosition: number;

    public onErrorCallback: (viewer: CardFileViewer) => void;

    private localization: Localization; 

    constructor(cardFileId: string, cardFileName: string, bucket: Bucket, initCardPosition?: number) {
        this.localization = new Localization();

        this.cardFileId = cardFileId;
        this.cardFileName = cardFileName;
        var cardFileDiv = document.createElement("div");
        $(cardFileDiv).addClass("cardfile-listing loading");
        this.htmlBody = cardFileDiv;
        this.actualBucket = bucket;
        bucket.addOnFinishLoadCallback(() => this.makePanel(initCardPosition));
        bucket.addOnErrorLoadCallback(() => this.showError());
    }

    public setViewedCardByPosition(initCardPosition: number) {
        this.actualBucket.addOnFinishLoadCallback(() => this.changeViewedCard(initCardPosition));
    }

    public getHtml(): HTMLDivElement {
        return this.htmlBody;
    }

    private showError() {
        $(this.htmlBody).removeClass("loading");
        $(this.htmlBody).addClass("error");
        //$(this.htmlBody).html("Nepodařilo se načíst výsledek ze zásuvky '" + this.actualBucket.getName() + "' z kartotéky '" + this.cardFileName + "'");
        $(this.htmlBody).html(this.localization.translateFormat("LoadingError", new Array<string>(this.actualBucket.getName(), this.cardFileName)  , "CardFiles").value);
        this.onErrorCallback(this);
    }

    private makePanel(initCardPosition?: number) {
        var cardFileDiv = this.htmlBody;
        this.makeRightPanel(cardFileDiv);
        this.makeLeftPanel(cardFileDiv);
        this.displayCardFileName(this.cardFileName);
        this.displayBucketName(this.actualBucket.getName());
        if (initCardPosition != null) {
            this.changeViewedCard(initCardPosition);
        } else {
            this.changeViewedCard(0);    
        }
        $(cardFileDiv).removeClass("loading");
    }

    private changeViewedCard(newActualCardPosition: number) {
        if (newActualCardPosition >= 0 && newActualCardPosition < this.actualBucket.getCardsCount()) {
            this.actualCardPosition = newActualCardPosition;
            this.actualBucket.getCard(this.actualCardPosition).loadCardDetail((card: CardDetail) => this.displayCardDetail(card));
            this.displayHeadwords(this.actualBucket.getCard(this.actualCardPosition).getHeadwords());
            this.displayCardPosition(this.actualCardPosition);
            this.actualizeSlider(this.actualCardPosition);
        }
    }

    private displayCardDetail(card: CardDetail) {
        this.displayImages(card.getId(), card.getImagesIds());
        this.displayNotes(card.getNotes());
        this.displayWarnings(card.getWarnings());
    }

    private changeActualCardPosition(positionChange: number) {
        var newPosition = this.actualCardPosition + positionChange;
        if (newPosition < 0) {
            newPosition = 0;
        }
        if (newPosition >= this.actualBucket.getCardsCount()) {
            newPosition = this.actualBucket.getCardsCount() - 1;
        }
        this.changeViewedCard(newPosition);
    }

    private displayCardPosition(position: number) {
        $(this.htmlBody).find(".card-position-text").find("a").html(position.toString());
    }

    private displayHeadwords(headwords: Array<string>) {
        var headwordsText = this.getHeadwordText(headwords);
        $(this.htmlBody).find(".headword-name").html(headwordsText);
        $(this.htmlBody).find(".cardfile-headword-text").html(headwordsText);
    }

    private getHeadwordText(headwords: Array<string>):string {
        var headwordsText: string = "";
        for (var i = 0; i < headwords.length; i++) {
            if (typeof headwords[i] === "undefined" || headwords[i] == null) {
                headwordsText = this.localization.translate("NotFilled", "CardFiles").value;
            } else {
                headwordsText += headwords[i];
                if (i < headwords.length - 1) {
                    headwordsText += ", ";
                }
            }
        }
        if (headwordsText === "") {
            headwordsText = this.localization.translate("NotFilled", "CardFiles").value;
        }

        return headwordsText;
    }

    private displayNotes(notes: Array<string>) {
        var ulContainer = $(this.htmlBody).find(".cardfile-note-list");
        ulContainer.empty();
        var parent = ulContainer.parent();
        ulContainer.detach();               //fix for infinite adding to computed height in parent div
        for (var i = 0; i < notes.length; i++) {
            if (!notes[i] || 0 === notes[i].length) {
                continue;
            }
            var li: HTMLLIElement = document.createElement("li");
            $(li).html(notes[i]);
            ulContainer.append(li);
        }
        parent.append(ulContainer);
    }

    private displayWarnings(warnings: Array<string>) {
        var ulContainer = $(this.htmlBody).find(".cardfile-notice-list");
        ulContainer.empty();
        var parent = ulContainer.parent();
        ulContainer.detach();                //fix for infinite adding to computed height in parent div
        for (var i = 0; i < warnings.length; i++) {
            if (!warnings[i] || 0 === warnings[i].length) {
                continue;
            }
            var li: HTMLLIElement = document.createElement("li");
            $(li).html(warnings[i]);
            ulContainer.append(li);
        }
        parent.append(ulContainer);
    }

    private displayImages(cardId: string, imageIds: string[]) {
        this.changeDisplayedPreviewImage(cardId, imageIds[0]);
        var pagesContainer = $(this.htmlBody).find(".cardfile-pages");
        pagesContainer.empty();
        for (var i = 0; i < imageIds.length; i++) {
            var thumbHtml = this.makeImageThumbnail(cardId, imageIds[i]);
            pagesContainer.append(thumbHtml);
        }
    }

    private makeImageThumbnail(cardId :string, imageId: string ): HTMLDivElement {
        var cardFileImageDiv: HTMLDivElement = document.createElement("div");
        $(cardFileImageDiv).addClass("cardfile-image-thumbnail");

        var imageAnchor: HTMLAnchorElement = document.createElement("a");
        imageAnchor.href = "#";
        $(imageAnchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeDisplayedPreviewImage(cardId, imageId);
            return false;
        });

        var image: HTMLImageElement = document.createElement("img");
        image.title = this.localization.translate("SmallPreview", "CardFiles").value;
        image.alt = this.localization.translate("SmallPreview", "CardFiles").value;
        image.src = getBaseUrl() +"CardFiles/CardFiles/Image?cardFileId="+this.cardFileId+"&bucketId="+this.actualBucket.getId()+"&cardId="+cardId+"&imageId="+imageId+"&imageSize=thumbnail";

        imageAnchor.appendChild(image);
        cardFileImageDiv.appendChild(imageAnchor);
        return cardFileImageDiv;
    }

    private changeDisplayedPreviewImage(cardId: string, imageId: string) {
        var cardFileImageDiv = $(this.htmlBody).find(".cardfile-image");
        $(cardFileImageDiv).find("img").attr("src", getBaseUrl()+"CardFiles/CardFiles/Image?cardFileId=" + this.cardFileId + "&bucketId=" + this.actualBucket.getId() + "&cardId=" + cardId + "&imageId=" + imageId + "&imageSize=preview");
        $(cardFileImageDiv).find("a").attr("href", getBaseUrl() +"CardFiles/CardFiles/Image?cardFileId=" + this.cardFileId + "&bucketId=" + this.actualBucket.getId() + "&cardId=" + cardId + "&imageId=" + imageId + "&imageSize=full");
    }

    private displayCardFileName(name: string) {
        $(this.htmlBody).find(".cardfile-name").html(name);
    }

    private displayBucketName(bucketName: string) {
        $(this.htmlBody).find(".cardfile-drawer-name").html(bucketName);
    }

    private actualizeSlider(position:number) {
        $(this.htmlBody).find(".slider").slider('value', position);
        $(this.htmlBody).find(".slider").find('.ui-slider-handle').find('.tooltip-inner').html(this.getInnerTooltipForSlider(position));
    }

    private getInnerTooltipForSlider(cardPosition: number): string {
        var headwords = this.actualBucket.getCard(cardPosition).getHeadwords();
        var headwordText: string = this.getHeadwordText(new Array(headwords[0]));
        if (headwords.length > 1) {
            headwordText += " ...";
        }

        return this.localization.translate("List", "CardFiles").value + cardPosition + "<br/>" + this.localization.translate("Keywords", "CardFiles").value + headwordText;
    }

    private makeLeftPanel(cardFileDiv : HTMLDivElement) {
        var cardFileLeftPanelDiv : HTMLDivElement = document.createElement("div");
        $(cardFileLeftPanelDiv).addClass("card-file-listing-left-panel");

        var headwordDiv : HTMLDivElement = document.createElement("div");
        $(headwordDiv).addClass("headword-description");

        var headwordNameSpan : HTMLSpanElement = document.createElement("span");
        $(headwordNameSpan).addClass("headword-name");
        headwordNameSpan.innerHTML = "";

        headwordDiv.appendChild(headwordNameSpan);
        cardFileLeftPanelDiv.appendChild(headwordDiv);

        var cardFileImageDiv: HTMLDivElement = document.createElement("div");
        $(cardFileImageDiv).addClass("cardfile-image");

        var imageAnchor: HTMLAnchorElement = document.createElement("a");
        imageAnchor.target = "_image";
        imageAnchor.href = "";

        var image: HTMLImageElement = document.createElement("img");
        image.title = this.localization.translate("Preview", "CardFiles").value;
        image.alt = this.localization.translate("Preview", "CardFiles").value;
        image.src = "";

        imageAnchor.appendChild(image);
        cardFileImageDiv.appendChild(imageAnchor);
        cardFileLeftPanelDiv.appendChild(cardFileImageDiv);

        cardFileDiv.appendChild(cardFileLeftPanelDiv);
    }


    private makeRightPanel(cardFileDiv: HTMLDivElement) {

        var cardFileRightPanelDiv: HTMLDivElement = document.createElement("div");
        $(cardFileRightPanelDiv).addClass("card-file-listing-right-panel");

        var cardFileControlPanelDiv: HTMLDivElement = document.createElement("div");
        $(cardFileControlPanelDiv).addClass("cardfile-listing-controls");

        var cardFileDescDiv: HTMLDivElement = document.createElement("div");
        $(cardFileDescDiv).addClass("cardfile-description");
        cardFileDescDiv.innerHTML = this.localization.translate("CardFile", "CardFiles").value + ": ";

        var cardFileNameSpan: HTMLSpanElement = document.createElement("span");
        $(cardFileNameSpan).addClass("cardfile-name");
        cardFileNameSpan.innerHTML = "";

        cardFileDescDiv.appendChild(cardFileNameSpan);
        cardFileControlPanelDiv.appendChild(cardFileDescDiv);
        
        var cardFileDrawerDescDiv: HTMLDivElement = document.createElement("div");
        $(cardFileDrawerDescDiv).addClass("cardfile-drawer-description");
        cardFileDrawerDescDiv.innerHTML = this.localization.translate("Drawer", "CardFiles").value + ": ";

        var cardFileDrawerNameSpan: HTMLSpanElement = document.createElement("span");
        $(cardFileDrawerNameSpan).addClass("cardfile-drawer-name");
        cardFileDrawerNameSpan.innerHTML = "";

        cardFileDrawerDescDiv.appendChild(cardFileDrawerNameSpan);
        cardFileControlPanelDiv.appendChild(cardFileDrawerDescDiv);

        var cardFilePageControlsDiv: HTMLDivElement = document.createElement("div");
        $(cardFilePageControlsDiv).addClass("cardfile-paging-controls");

        this.makeSlider(cardFilePageControlsDiv);

        this.makeNavButtons(cardFilePageControlsDiv);
        
        cardFileControlPanelDiv.appendChild(cardFilePageControlsDiv);

        var cardFilePagesDiv: HTMLDivElement = document.createElement("div");
        $(cardFilePagesDiv).addClass("cardfile-pages");
        cardFilePagesDiv.innerHTML = this.localization.translate("Pages", "CardFiles").value;

        cardFileControlPanelDiv.appendChild(cardFilePagesDiv);

        var cardFileScrollableRightPanelDiv: HTMLDivElement = document.createElement("div");
        $(cardFileScrollableRightPanelDiv).addClass("cardfile-scrollable-part-right-panel");

        var cardFileHeadwordDescDiv: HTMLDivElement = document.createElement("div");
        $(cardFileHeadwordDescDiv).addClass("cardfile-headword-description");
        cardFileHeadwordDescDiv.innerHTML = this.localization.translate("Keywords", "CardFiles").value;

        var cardFileHeadwordTextSpan: HTMLSpanElement = document.createElement("span");
        $(cardFileHeadwordTextSpan).addClass("cardfile-headword-text");
        cardFileHeadwordTextSpan.innerHTML = "";

        cardFileHeadwordDescDiv.appendChild(cardFileHeadwordTextSpan);
        cardFileScrollableRightPanelDiv.appendChild(cardFileHeadwordDescDiv);

        var cardFileNoticeDiv: HTMLDivElement = document.createElement("div");
        $(cardFileNoticeDiv).addClass("cardfile-notice");
        cardFileNoticeDiv.innerHTML = this.localization.translate("Notice", "CardFiles").value;

        var cardFileNoticeList: HTMLUListElement = document.createElement("ul");
        $(cardFileNoticeList).addClass("cardfile-notice-list");

        cardFileNoticeDiv.appendChild(cardFileNoticeList);
        cardFileScrollableRightPanelDiv.appendChild(cardFileNoticeDiv);

        var cardFileNoteDiv: HTMLDivElement = document.createElement("div");
        $(cardFileNoteDiv).addClass("cardfile-note");
        cardFileNoteDiv.innerHTML = this.localization.translate("Note", "CardFiles").value;

        var cardFileNoteList: HTMLUListElement = document.createElement("ul");
        $(cardFileNoteList).addClass("cardfile-note-list");

        cardFileNoteDiv.appendChild(cardFileNoteList);
        cardFileScrollableRightPanelDiv.appendChild(cardFileNoteDiv);

        cardFileControlPanelDiv.appendChild(cardFileScrollableRightPanelDiv);

        cardFileRightPanelDiv.appendChild(cardFileControlPanelDiv);
        cardFileDiv.appendChild(cardFileRightPanelDiv);
    }

    private makeNavButtons(pageControlsDiv: HTMLDivElement) {
        var paginationUl: HTMLUListElement = document.createElement('ul');
        $(paginationUl).addClass('cardfile-pagination');

        var liElement: HTMLLIElement = document.createElement('li');
        var anchor: HTMLAnchorElement = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '|<';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeViewedCard(0);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<<';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeActualCardPosition(- 10);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeActualCardPosition(- 1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);


        liElement = document.createElement('li');
        $(liElement).addClass("card-position-text");
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = "";
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
     

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeActualCardPosition(+1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>>';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeActualCardPosition(+10);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>|';
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.changeViewedCard(this.actualBucket.getCardsCount() - 1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);

        pageControlsDiv.appendChild(paginationUl);
    }

    private makeSlider(pageControlsDiv: HTMLDivElement) {
        var actualCardPosition: number = 0;
        var sliderDiv: HTMLDivElement = document.createElement("div");
        $(sliderDiv).addClass("slider");        
        $(sliderDiv).slider({
            min: 0,
            max: this.actualBucket.getCardsCount() - 1,
            value: actualCardPosition,
            start: (event, ui) => {
                $(event.target as Element).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: (event, ui) => {
                this.changeViewedCard(ui.value);
                $(event.target as Element).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
            },
            slide: (event, ui) => {
                const targetEl = $(event.target as Element);
                targetEl.find('.ui-slider-handle').find('.slider-tip').stop(true, true);
                targetEl.find('.ui-slider-handle').find('.slider-tip').show();
                targetEl.find('.ui-slider-handle').find('.tooltip-inner').html(this.getInnerTooltipForSlider(ui.value));
            }

        });

        var sliderTooltip: HTMLDivElement = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip: HTMLDivElement = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html(this.getInnerTooltipForSlider(actualCardPosition));
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();

        var sliderHandle = $(sliderDiv).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover((event) => {
            $(event.target as Node as Element).find('.slider-tip').stop(true, true);
            $(event.target as Node as Element).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout((event) => {
            $(event.target as Node as Element).find('.slider-tip').fadeOut(1000);
        });

        pageControlsDiv.appendChild(sliderDiv);
    }
}

class Bucket {
    private id: string;
    private cardFileId: string;
    private cardsCount: number;
    private cards: Array<Card>;
    private callbacks: Array<() => void>;
    private errorCallbacks: Array<() => void>;
    private loaded: boolean;
    private loadingError: boolean;
    private bucketName : string;

    constructor(cardFileId: string, bucketId: string, bucketName: string) {
        this.id = bucketId;
        this.bucketName = bucketName;
        this.cardFileId = cardFileId;
        this.loaded = false;
        this.cards = new Array<Card>();
        this.callbacks = new Array<() => void>();
        this.errorCallbacks = new Array<() => void>();
        this.loadCardsInfo();
    }

    public getId(): string {
        return this.id;
    }

    public getName(): string {
        return this.bucketName;
    }

    public getCardFileId(): string {
        return this.cardFileId;
    }

    public getCardsCount(): number {
        return this.cardsCount;
    }

    public getCard(position: number): Card {
        if (position >= 0 && position < this.cardsCount) {
            return this.cards[position];   
        }
        return null;
    }

    public addOnFinishLoadCallback(callback: () => void) {
        if (this.loaded) {
            callback();
        } else {
            this.callbacks.push(callback);
        }
    }

    public addOnErrorLoadCallback(errorCallback: () => void) {
        if (this.loadingError) {
            errorCallback();
        } else {
            this.errorCallbacks.push(errorCallback);
        }
    }

    private onFinishLoad() {
        this.loaded = true;
        for (var i = 0; i < this.callbacks.length; i++) {
            this.callbacks[i]();
        }

        this.callbacks.length = 0; //Clear array of callbacks
    }

    private onErrorLoad() {
        this.loadingError = true;
        for (var i = 0; i < this.errorCallbacks.length; i++) {
            this.errorCallbacks[i]();
        }

        this.errorCallbacks.length = 0; //Clear array of callbacks
    }

    private loadCardsInfo() {
        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: this.cardFileId, bucketId: this.id } as JQuery.PlainObject,
            url: getBaseUrl()+"CardFiles/CardFiles/CardsShort",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                var cards = response["cards"];
                this.cardsCount = cards.length;
                for (var i = 0; i < cards.length; i++) {
                    var jsonCard = cards[i];
                    this.cards.push(new Card(jsonCard["Id"], jsonCard["Position"], jsonCard["Headwords"], this));
                }
                this.onFinishLoad();
            },
            error: (response) => {
                this.onErrorLoad();
            }
        });
    }
}

class Card {
    private id: string;
    private position: number;
    private headwords: Array<string>;
    private parentBucket: Bucket;

    constructor(cardId: string, position: number, headwords: Array<string>, parentBucket: Bucket) {
        this.id = cardId;
        this.position = position;
        this.headwords = headwords;
        this.parentBucket = parentBucket;
    }

    public getId(): string {
        return this.id;
    }

    public getPosition(): number {
        return this.position;
    }

    public getHeadwords(): Array<string> {
        return this.headwords;
    }

    public getParentBucket(): Bucket {
        return this.parentBucket;
    }

    public loadCardDetail(callback: (card: CardDetail) => void) {
        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: this.parentBucket.getCardFileId(), bucketId: this.parentBucket.getId(), cardId: this.getId()} as JQuery.PlainObject,
            url: getBaseUrl()+"CardFiles/CardFiles/Card",
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                var card = response["card"];
                var images = card["Images"];
                var imagesArray = new Array<string>();
                for (var i = 0; i < images.length; i++) {
                    var imageId = images[i]["Id"];
                    imagesArray.push(imageId);
                }
                callback(new CardDetail(card["Id"], card["Position"], card["Headwords"], card["Warnings"], card["Notes"], imagesArray));
            },
            error: (response) => {
                //TODO resolve error
            }
        });
    }
}

class CardDetail {
    private id: string;
    private position: number;
    private headwords: Array<string>;
    private warnings: Array<string>;
    private notes: Array<string>;
    private images: Array<string>;

    constructor(cardId: string, position: number, headwords: Array<string>, warnings: Array<string>, notes: Array<string>, images: Array<string>) {
        this.id = cardId;
        this.position = position;
        this.headwords = headwords;
        this.warnings = warnings;
        this.notes = notes;
        this.images = images;
    }

    public getId(): string {
        return this.id;
    }

    public getPosition(): number {
        return this.position;
    }

    public getHeadwords(): Array<string> {
        return this.headwords;
    }

    public getWarnings(): Array<string> {
        return this.warnings;
    }

    public getNotes(): Array<string> {
        return this.notes;
    }


    public getImagesIds(): Array<string> {
        return this.images;
    }
}



