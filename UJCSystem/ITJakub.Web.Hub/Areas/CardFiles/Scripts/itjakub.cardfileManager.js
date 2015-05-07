var CardFileManager = (function () {
    function CardFileManager(cardFilesContainer) {
        this.cardFilesContainer = cardFilesContainer;
    }
    CardFileManager.prototype.makeCardFile = function (cardFileId, bucketId) {
        var cardFile = new CardFileViewer(cardFileId, bucketId);
        $(this.cardFilesContainer).append(cardFile.getHtml());
    };
    CardFileManager.prototype.clearContainer = function () {
        $(this.cardFilesContainer).empty();
    };
    return CardFileManager;
})();
var CardFileViewer = (function () {
    function CardFileViewer(cardFileId, bucketId) {
        var _this = this;
        this.cardFileId = cardFileId;
        var cardFileDiv = document.createElement("div");
        $(cardFileDiv).addClass("cardfile-listing loading");
        this.htmlBody = cardFileDiv;
        this.actualBucket = new Bucket(cardFileId, bucketId, function () { return _this.makePanel(); });
    }
    CardFileViewer.prototype.makePanel = function () {
        var cardFileDiv = this.htmlBody;
        this.makeLeftPanel(cardFileDiv);
        this.makeRightPanel(cardFileDiv);
        this.displayCardFileName(""); //TODO ajax load and fill
        this.displayBucketName(""); //TODO ajax load and fill
        this.changeViewedCard(0);
        $(cardFileDiv).removeClass("loading");
    };
    CardFileViewer.prototype.getHtml = function () {
        return this.htmlBody;
    };
    CardFileViewer.prototype.changeViewedCard = function (newActualCardPosition) {
        var _this = this;
        if (newActualCardPosition >= 0 && newActualCardPosition < this.actualBucket.getCardsCount()) {
            this.actualCardPosition = newActualCardPosition;
            this.actualBucket.getCard(this.actualCardPosition).loadCardDetail(function (card) { return _this.displayCardDetail(card); });
            this.displayHeadword(this.actualBucket.getCard(this.actualCardPosition).getHeadword());
        }
    };
    CardFileViewer.prototype.displayCardDetail = function (card) {
        this.displayImage("", "");
        this.displayNote(card.getId());
        this.displayWarning(card.getPosition().toString());
    };
    CardFileViewer.prototype.changeActualCardPosition = function (positionChange) {
        this.changeViewedCard(this.actualCardPosition + positionChange);
    };
    CardFileViewer.prototype.displayHeadword = function (headword) {
        $(this.htmlBody).find(".headword-name").html(headword);
        $(this.htmlBody).find(".cardfile-headword-text").html(headword);
    };
    CardFileViewer.prototype.displayNote = function (note) {
        $(this.htmlBody).find(".cardfile-note-text").html(note);
    };
    CardFileViewer.prototype.displayWarning = function (warning) {
        $(this.htmlBody).find(".cardfile-notice-text").html(warning);
    };
    CardFileViewer.prototype.displayImage = function (imagePreviewSrc, imageSrc) {
        var cardFileImageDiv = $(this.htmlBody).find(".cardfile-image");
        $(cardFileImageDiv).find("img").attr("src", imagePreviewSrc);
        $(cardFileImageDiv).find("a").attr("href", imageSrc);
    };
    CardFileViewer.prototype.displayCardFileName = function (name) {
        $(this.htmlBody).find(".cardfile-name").html(name);
    };
    CardFileViewer.prototype.displayBucketName = function (bucketName) {
        $(this.htmlBody).find(".cardfile-drawer-name").html(bucketName);
    };
    CardFileViewer.prototype.getInnerTooltipForSlider = function (cardPosition) {
        return "Lístek: " + cardPosition + "<br/> Heslo: " + this.actualBucket.getCard(cardPosition).getHeadword();
    };
    CardFileViewer.prototype.makeLeftPanel = function (cardFileDiv) {
        var cardFileLeftPanelDiv = document.createElement("div");
        $(cardFileLeftPanelDiv).addClass("card-file-listing-left-panel");
        var headwordDiv = document.createElement("div");
        $(headwordDiv).addClass("headword-description");
        var headwordNameSpan = document.createElement("span");
        $(headwordNameSpan).addClass("headword-name");
        headwordNameSpan.innerText = "";
        headwordDiv.appendChild(headwordNameSpan);
        cardFileLeftPanelDiv.appendChild(headwordDiv);
        var cardFileImageDiv = document.createElement("div");
        $(cardFileImageDiv).addClass("cardfile-image");
        var imageAnchor = document.createElement("a");
        imageAnchor.target = "_image";
        imageAnchor.href = "http://bara.ujc.cas.cz/bara/img/71/911005803_745788?db=2"; //TODO load from parameter
        var image = document.createElement("img");
        image.title = "Náhled";
        image.alt = "Náhled";
        image.src = "http://bara.ujc.cas.cz/bara/preview/71/911751591_31487.jpeg?db=2"; //TODO load from parameter
        imageAnchor.appendChild(image);
        cardFileImageDiv.appendChild(imageAnchor);
        cardFileLeftPanelDiv.appendChild(cardFileImageDiv);
        cardFileDiv.appendChild(cardFileLeftPanelDiv);
    };
    CardFileViewer.prototype.makeRightPanel = function (cardFileDiv) {
        var cardFileRightPanelDiv = document.createElement("div");
        $(cardFileRightPanelDiv).addClass("card-file-listing-right-panel");
        var cardFileDescDiv = document.createElement("div");
        $(cardFileDescDiv).addClass("cardfile-description");
        cardFileDescDiv.innerText = "Kartotéka: ";
        var cardFileNameSpan = document.createElement("span");
        $(cardFileNameSpan).addClass("cardfile-name");
        cardFileNameSpan.innerText = "";
        cardFileDescDiv.appendChild(cardFileNameSpan);
        cardFileRightPanelDiv.appendChild(cardFileDescDiv);
        var cardFileDrawerDescDiv = document.createElement("div");
        $(cardFileDrawerDescDiv).addClass("cardfile-drawer-description");
        cardFileDrawerDescDiv.innerText = "Zásuvka: ";
        var cardFileDrawerNameSpan = document.createElement("span");
        $(cardFileDrawerNameSpan).addClass("cardfile-drawer-name");
        cardFileDrawerNameSpan.innerText = "";
        cardFileDrawerDescDiv.appendChild(cardFileDrawerNameSpan);
        cardFileRightPanelDiv.appendChild(cardFileDrawerDescDiv);
        var cardFilePageControlsDiv = document.createElement("div");
        $(cardFilePageControlsDiv).addClass("cardfile-paging-controls");
        this.makeSlider(cardFilePageControlsDiv);
        cardFileRightPanelDiv.appendChild(cardFilePageControlsDiv);
        var cardFilePagesDiv = document.createElement("div");
        $(cardFilePagesDiv).addClass("cardfile-pages");
        cardFilePagesDiv.innerText = "Stránky: ";
        cardFileRightPanelDiv.appendChild(cardFilePagesDiv);
        var cardFileHeadwordDescDiv = document.createElement("div");
        $(cardFileHeadwordDescDiv).addClass("cardfile-headword-description");
        cardFileHeadwordDescDiv.innerText = "Heslo: ";
        var cardFileHeadwordTextSpan = document.createElement("span");
        $(cardFileHeadwordTextSpan).addClass("cardfile-headword-text");
        cardFileHeadwordTextSpan.innerText = "";
        cardFileHeadwordDescDiv.appendChild(cardFileHeadwordTextSpan);
        cardFileRightPanelDiv.appendChild(cardFileHeadwordDescDiv);
        var cardFileNoticeDiv = document.createElement("div");
        $(cardFileNoticeDiv).addClass("cardfile-notice");
        cardFileNoticeDiv.innerText = "Upozornění: ";
        var cardFileNoticeSpan = document.createElement("span");
        $(cardFileNoticeSpan).addClass("cardfile-notice-text");
        cardFileNoticeSpan.innerText = "";
        cardFileNoticeDiv.appendChild(cardFileNoticeSpan);
        cardFileRightPanelDiv.appendChild(cardFileNoticeDiv);
        var cardFileNoteDiv = document.createElement("div");
        $(cardFileNoteDiv).addClass("cardfile-note");
        cardFileNoteDiv.innerText = "Poznámka: ";
        var cardFileNoteSpan = document.createElement("span");
        $(cardFileNoteSpan).addClass("cardfile-note-text");
        cardFileNoteSpan.innerText = "";
        cardFileNoteDiv.appendChild(cardFileNoteSpan);
        cardFileRightPanelDiv.appendChild(cardFileNoteDiv);
        cardFileDiv.appendChild(cardFileRightPanelDiv);
    };
    CardFileViewer.prototype.makeSlider = function (pageControlsDiv) {
        var _this = this;
        var actualCardPosition = 0;
        var sliderDiv = document.createElement("div");
        $(sliderDiv).addClass("slider");
        $(sliderDiv).slider({
            min: 0,
            max: this.actualBucket.getCardsCount() - 1,
            value: actualCardPosition,
            start: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: function (event, ui) {
                _this.changeViewedCard(ui.value);
                $(event.target).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
            },
            slide: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').stop(true, true);
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html(_this.getInnerTooltipForSlider(ui.value));
            }
        });
        var sliderTooltip = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);
        var innerTooltip = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html(this.getInnerTooltipForSlider(actualCardPosition));
        sliderTooltip.appendChild(innerTooltip);
        $(sliderTooltip).hide();
        var sliderHandle = $(sliderDiv).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        $(sliderHandle).hover(function (event) {
            $(event.target).find('.slider-tip').stop(true, true);
            $(event.target).find('.slider-tip').show();
        });
        $(sliderHandle).mouseout(function (event) {
            $(event.target).find('.slider-tip').fadeOut(1000);
        });
        pageControlsDiv.appendChild(sliderDiv);
    };
    return CardFileViewer;
})();
var Bucket = (function () {
    function Bucket(cardFileId, bucketId, initFinishedCallback) {
        this.id = bucketId;
        this.cardFileId = cardFileId;
        this.cards = new Array();
        this.loadCardsInfo(initFinishedCallback);
    }
    Bucket.prototype.getId = function () {
        return this.id;
    };
    Bucket.prototype.getCardFileId = function () {
        return this.cardFileId;
    };
    Bucket.prototype.getCardsCount = function () {
        return this.cardsCount;
    };
    Bucket.prototype.getCard = function (position) {
        if (position >= 0 && position < this.cardsCount) {
            return this.cards[position];
        }
        return null;
    };
    Bucket.prototype.loadCardsInfo = function (callback) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: this.cardFileId, bucketId: this.id },
            url: "/CardFiles/CardFiles/CardsShort",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                var cards = response["cards"];
                _this.cardsCount = cards.length;
                for (var i = 0; i < cards.length; i++) {
                    var jsonCard = cards[i];
                    _this.cards.push(new Card(jsonCard["Id"], jsonCard["Position"], jsonCard["Headword"], _this));
                }
                callback();
            },
            error: function (response) {
                //TODO resolve error
            }
        });
    };
    return Bucket;
})();
var Card = (function () {
    function Card(cardId, position, headword, parentBucket) {
        this.id = cardId;
        this.position = position;
        this.headword = headword;
        this.parentBucket = parentBucket;
    }
    Card.prototype.getId = function () {
        return this.id;
    };
    Card.prototype.getPosition = function () {
        return this.position;
    };
    Card.prototype.getHeadword = function () {
        return this.headword;
    };
    Card.prototype.getParentBucket = function () {
        return this.parentBucket;
    };
    Card.prototype.loadCardDetail = function (callback) {
        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: this.parentBucket.getCardFileId(), bucketId: this.parentBucket.getId(), cardId: this.getId() },
            url: "/CardFiles/CardFiles/Card",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                var card = response["card"];
                var images = card["Images"];
                var imagesArray = new Array();
                for (var i = 0; i < images.length; i++) {
                    imagesArray.push(images[i]["Id"]);
                }
                callback(new CardDetail(card["Id"], card["Position"], card["Headword"], card["Warning"], card["Note"], images));
            },
            error: function (response) {
                //TODO resolve error
            }
        });
    };
    return Card;
})();
var CardDetail = (function () {
    function CardDetail(cardId, position, headword, warning, note, images) {
        this.id = cardId;
        this.position = position;
        this.headword = headword;
        this.warning = warning;
        this.note = note;
        this.images = images;
    }
    CardDetail.prototype.getId = function () {
        return this.id;
    };
    CardDetail.prototype.getPosition = function () {
        return this.position;
    };
    CardDetail.prototype.getHeadword = function () {
        return this.headword;
    };
    CardDetail.prototype.getImages = function () {
        return this.images;
    };
    CardDetail.prototype.getWarning = function () {
        return this.warning;
    };
    CardDetail.prototype.getNote = function () {
        return this.note;
    };
    return CardDetail;
})();
//# sourceMappingURL=itjakub.cardfileManager.js.map