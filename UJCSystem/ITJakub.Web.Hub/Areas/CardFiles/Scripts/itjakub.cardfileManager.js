var CardFileManager = (function () {
    function CardFileManager(cardFilesContainer) {
        this.cardFilesContainer = cardFilesContainer;
        this.bucketsCache = new Array();
    }
    CardFileManager.prototype.makeCardFile = function (cardFileId, cardFileName, bucketId, bucketName, initCardPosition) {
        var bucket = this.getBucket(cardFileId, bucketId, bucketName);
        var cardFile = new CardFileViewer(cardFileId, cardFileName, bucket);
        $(this.cardFilesContainer).append(cardFile.getHtml());
        if (initCardPosition != null) {
            cardFile.setViewedCardByPosition(initCardPosition);
        }
    };
    CardFileManager.prototype.getKeyByCardFileAndBucket = function (cardFileId, bucketId) {
        return cardFileId + bucketId;
    };
    CardFileManager.prototype.cacheContainsKey = function (key) {
        var item = this.bucketsCache[key];
        return typeof item !== "undefined";
    };
    CardFileManager.prototype.getBucket = function (cardFileId, bucketId, bucketName) {
        var key = this.getKeyByCardFileAndBucket(cardFileId, bucketId);
        if (!this.cacheContainsKey(key)) {
            var bucket = new Bucket(cardFileId, bucketId, bucketName);
            this.bucketsCache[key] = bucket;
        }
        return this.bucketsCache[key];
    };
    CardFileManager.prototype.clearContainer = function () {
        $(this.cardFilesContainer).empty();
        this.emptyCache();
    };
    CardFileManager.prototype.emptyCache = function () {
        this.bucketsCache.length = 0;
        this.bucketsCache = new Array();
    };
    return CardFileManager;
})();
var CardFileViewer = (function () {
    function CardFileViewer(cardFileId, cardFileName, bucket) {
        var _this = this;
        this.cardFileId = cardFileId;
        this.cardFileName = cardFileName;
        var cardFileDiv = document.createElement("div");
        $(cardFileDiv).addClass("cardfile-listing loading");
        this.htmlBody = cardFileDiv;
        this.actualBucket = bucket;
        bucket.addOnFinishLoadCallback(function () { return _this.makePanel(); });
    }
    CardFileViewer.prototype.setViewedCardByPosition = function (initCardPosition) {
        var _this = this;
        this.actualBucket.addOnFinishLoadCallback(function () { return _this.changeViewedCard(initCardPosition); });
    };
    CardFileViewer.prototype.getHtml = function () {
        return this.htmlBody;
    };
    CardFileViewer.prototype.makePanel = function () {
        var cardFileDiv = this.htmlBody;
        this.makeLeftPanel(cardFileDiv);
        this.makeRightPanel(cardFileDiv);
        this.displayCardFileName(this.cardFileName);
        this.displayBucketName(this.actualBucket.getName());
        this.changeViewedCard(0);
        $(cardFileDiv).removeClass("loading");
    };
    CardFileViewer.prototype.changeViewedCard = function (newActualCardPosition) {
        var _this = this;
        if (newActualCardPosition >= 0 && newActualCardPosition < this.actualBucket.getCardsCount()) {
            this.actualCardPosition = newActualCardPosition;
            this.actualBucket.getCard(this.actualCardPosition).loadCardDetail(function (card) { return _this.displayCardDetail(card); });
            this.displayHeadwords(this.actualBucket.getCard(this.actualCardPosition).getHeadwords());
            this.displayCardPosition(this.actualCardPosition);
            this.actualizeSlider(this.actualCardPosition);
        }
    };
    CardFileViewer.prototype.displayCardDetail = function (card) {
        this.displayImages(card.getId(), card.getImagesIds());
        this.displayNotes(card.getNotes());
        this.displayWarnings(card.getWarnings());
    };
    CardFileViewer.prototype.changeActualCardPosition = function (positionChange) {
        var newPosition = this.actualCardPosition + positionChange;
        if (newPosition < 0) {
            newPosition = 0;
        }
        if (newPosition >= this.actualBucket.getCardsCount()) {
            newPosition = this.actualBucket.getCardsCount() - 1;
        }
        this.changeViewedCard(newPosition);
    };
    CardFileViewer.prototype.displayCardPosition = function (position) {
        $(this.htmlBody).find(".card-position-text").find("a").html(position.toString());
    };
    CardFileViewer.prototype.displayHeadwords = function (headwords) {
        var headwordsText = "";
        for (var i = 0; i < headwords.length; i++) {
            headwordsText += headwords[i];
            if (i < headwords.length - 1) {
                headwordsText += ", ";
            }
        }
        $(this.htmlBody).find(".headword-name").html(headwordsText);
        $(this.htmlBody).find(".cardfile-headword-text").html(headwordsText);
    };
    CardFileViewer.prototype.displayNotes = function (notes) {
        var ulContainer = $(this.htmlBody).find(".cardfile-note-list");
        ulContainer.empty();
        var parent = ulContainer.parent();
        ulContainer.detach(); //fix for infinite adding to computed height in parent div
        for (var i = 0; i < notes.length; i++) {
            if (!notes[i] || 0 === notes[i].length) {
                continue;
            }
            var li = document.createElement("li");
            $(li).html(notes[i]);
            ulContainer.append(li);
        }
        parent.append(ulContainer);
    };
    CardFileViewer.prototype.displayWarnings = function (warnings) {
        var ulContainer = $(this.htmlBody).find(".cardfile-notice-list");
        ulContainer.empty();
        var parent = ulContainer.parent();
        ulContainer.detach(); //fix for infinite adding to computed height in parent div
        for (var i = 0; i < warnings.length; i++) {
            if (!warnings[i] || 0 === warnings[i].length) {
                continue;
            }
            var li = document.createElement("li");
            $(li).html(warnings[i]);
            ulContainer.append(li);
        }
        parent.append(ulContainer);
    };
    CardFileViewer.prototype.displayImages = function (cardId, imageIds) {
        this.changeDisplayedPreviewImage(cardId, imageIds[0]);
        var pagesContainer = $(this.htmlBody).find(".cardfile-pages");
        pagesContainer.empty();
        for (var i = 0; i < imageIds.length; i++) {
            var thumbHtml = this.makeImageThumbnail(cardId, imageIds[i]);
            pagesContainer.append(thumbHtml);
        }
    };
    CardFileViewer.prototype.makeImageThumbnail = function (cardId, imageId) {
        var _this = this;
        var cardFileImageDiv = document.createElement("div");
        $(cardFileImageDiv).addClass("cardfile-image-thumbnail");
        var imageAnchor = document.createElement("a");
        imageAnchor.href = "#";
        $(imageAnchor).click(function (event) {
            event.stopPropagation();
            _this.changeDisplayedPreviewImage(cardId, imageId);
            return false;
        });
        var image = document.createElement("img");
        image.title = "Malý náhled";
        image.alt = "Malý náhled";
        image.src = "/CardFiles/CardFiles/Image?cardFileId=" + this.cardFileId + "&bucketId=" + this.actualBucket.getId() + "&cardId=" + cardId + "&imageId=" + imageId + "&imageSize=thumbnail";
        imageAnchor.appendChild(image);
        cardFileImageDiv.appendChild(imageAnchor);
        return cardFileImageDiv;
    };
    CardFileViewer.prototype.changeDisplayedPreviewImage = function (cardId, imageId) {
        var cardFileImageDiv = $(this.htmlBody).find(".cardfile-image");
        $(cardFileImageDiv).find("img").attr("src", "/CardFiles/CardFiles/Image?cardFileId=" + this.cardFileId + "&bucketId=" + this.actualBucket.getId() + "&cardId=" + cardId + "&imageId=" + imageId + "&imageSize=preview");
        $(cardFileImageDiv).find("a").attr("href", "/CardFiles/CardFiles/Image?cardFileId=" + this.cardFileId + "&bucketId=" + this.actualBucket.getId() + "&cardId=" + cardId + "&imageId=" + imageId + "&imageSize=full");
    };
    CardFileViewer.prototype.displayCardFileName = function (name) {
        $(this.htmlBody).find(".cardfile-name").html(name);
    };
    CardFileViewer.prototype.displayBucketName = function (bucketName) {
        $(this.htmlBody).find(".cardfile-drawer-name").html(bucketName);
    };
    CardFileViewer.prototype.actualizeSlider = function (position) {
        $(this.htmlBody).find(".slider").slider('value', position);
        $(this.htmlBody).find(".slider").find('.ui-slider-handle').find('.tooltip-inner').html(this.getInnerTooltipForSlider(position));
    };
    CardFileViewer.prototype.getInnerTooltipForSlider = function (cardPosition) {
        var headwords = this.actualBucket.getCard(cardPosition).getHeadwords();
        var headwordText = headwords[0];
        if (headwords.length > 1) {
            headwordText += " ...";
        }
        return "Lístek: " + cardPosition + "<br/> Hesla: " + headwordText;
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
        imageAnchor.href = "";
        var image = document.createElement("img");
        image.title = "Náhled";
        image.alt = "Náhled";
        image.src = "";
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
        this.makeNavButtons(cardFilePageControlsDiv);
        cardFileRightPanelDiv.appendChild(cardFilePageControlsDiv);
        var cardFilePagesDiv = document.createElement("div");
        $(cardFilePagesDiv).addClass("cardfile-pages");
        cardFilePagesDiv.innerText = "Stránky: ";
        cardFileRightPanelDiv.appendChild(cardFilePagesDiv);
        var cardFileScrollableRightPanelDiv = document.createElement("div");
        $(cardFileScrollableRightPanelDiv).addClass("cardfile-scrollable-part-right-panel");
        var cardFileHeadwordDescDiv = document.createElement("div");
        $(cardFileHeadwordDescDiv).addClass("cardfile-headword-description");
        cardFileHeadwordDescDiv.innerText = "Hesla: ";
        var cardFileHeadwordTextSpan = document.createElement("span");
        $(cardFileHeadwordTextSpan).addClass("cardfile-headword-text");
        cardFileHeadwordTextSpan.innerText = "";
        cardFileHeadwordDescDiv.appendChild(cardFileHeadwordTextSpan);
        cardFileScrollableRightPanelDiv.appendChild(cardFileHeadwordDescDiv);
        var cardFileNoticeDiv = document.createElement("div");
        $(cardFileNoticeDiv).addClass("cardfile-notice");
        cardFileNoticeDiv.innerText = "Upozornění: ";
        var cardFileNoticeList = document.createElement("ul");
        $(cardFileNoticeList).addClass("cardfile-notice-list");
        cardFileNoticeDiv.appendChild(cardFileNoticeList);
        cardFileScrollableRightPanelDiv.appendChild(cardFileNoticeDiv);
        var cardFileNoteDiv = document.createElement("div");
        $(cardFileNoteDiv).addClass("cardfile-note");
        cardFileNoteDiv.innerText = "Poznámka: ";
        var cardFileNoteList = document.createElement("ul");
        $(cardFileNoteList).addClass("cardfile-note-list");
        cardFileNoteDiv.appendChild(cardFileNoteList);
        cardFileScrollableRightPanelDiv.appendChild(cardFileNoteDiv);
        cardFileRightPanelDiv.appendChild(cardFileScrollableRightPanelDiv);
        cardFileDiv.appendChild(cardFileRightPanelDiv);
    };
    CardFileViewer.prototype.makeNavButtons = function (pageControlsDiv) {
        var _this = this;
        var paginationUl = document.createElement('ul');
        $(paginationUl).addClass('cardfile-pagination');
        var liElement = document.createElement('li');
        var anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '|<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeViewedCard(0);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeActualCardPosition(-10);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '<';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeActualCardPosition(-1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        $(liElement).addClass("card-position-text");
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = "";
        $(anchor).click(function (event) {
            event.stopPropagation();
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeActualCardPosition(+1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>>';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeActualCardPosition(+10);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        liElement = document.createElement('li');
        anchor = document.createElement('a');
        anchor.href = '#';
        anchor.innerHTML = '>|';
        $(anchor).click(function (event) {
            event.stopPropagation();
            _this.changeViewedCard(_this.actualBucket.getCardsCount() - 1);
            return false;
        });
        liElement.appendChild(anchor);
        paginationUl.appendChild(liElement);
        pageControlsDiv.appendChild(paginationUl);
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
    function Bucket(cardFileId, bucketId, bucketName) {
        this.id = bucketId;
        this.bucketName = bucketName;
        this.cardFileId = cardFileId;
        this.loaded = false;
        this.cards = new Array();
        this.callbacks = new Array();
        this.loadCardsInfo();
    }
    Bucket.prototype.getId = function () {
        return this.id;
    };
    Bucket.prototype.getName = function () {
        return this.bucketName;
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
    Bucket.prototype.addOnFinishLoadCallback = function (callback) {
        if (this.loaded) {
            callback();
        }
        else {
            this.callbacks.push(callback);
        }
    };
    Bucket.prototype.onFinishLoad = function () {
        this.loaded = true;
        for (var i = 0; i < this.callbacks.length; i++) {
            this.callbacks[i]();
        }
        this.callbacks.length = 0; //Clear array of callbacks
    };
    Bucket.prototype.loadCardsInfo = function () {
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
                    _this.cards.push(new Card(jsonCard["Id"], jsonCard["Position"], jsonCard["Headwords"], _this));
                }
                _this.onFinishLoad();
            },
            error: function (response) {
                //TODO resolve error
            }
        });
    };
    return Bucket;
})();
var Card = (function () {
    function Card(cardId, position, headwords, parentBucket) {
        this.id = cardId;
        this.position = position;
        this.headwords = headwords;
        this.parentBucket = parentBucket;
    }
    Card.prototype.getId = function () {
        return this.id;
    };
    Card.prototype.getPosition = function () {
        return this.position;
    };
    Card.prototype.getHeadwords = function () {
        return this.headwords;
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
                    var imageId = images[i]["Id"];
                    imagesArray.push(imageId);
                }
                callback(new CardDetail(card["Id"], card["Position"], card["Headwords"], card["Warnings"], card["Notes"], imagesArray));
            },
            error: function (response) {
                //TODO resolve error
            }
        });
    };
    return Card;
})();
var CardDetail = (function () {
    function CardDetail(cardId, position, headwords, warnings, notes, images) {
        this.id = cardId;
        this.position = position;
        this.headwords = headwords;
        this.warnings = warnings;
        this.notes = notes;
        this.images = images;
    }
    CardDetail.prototype.getId = function () {
        return this.id;
    };
    CardDetail.prototype.getPosition = function () {
        return this.position;
    };
    CardDetail.prototype.getHeadwords = function () {
        return this.headwords;
    };
    CardDetail.prototype.getWarnings = function () {
        return this.warnings;
    };
    CardDetail.prototype.getNotes = function () {
        return this.notes;
    };
    CardDetail.prototype.getImagesIds = function () {
        return this.images;
    };
    return CardDetail;
})();
//# sourceMappingURL=itjakub.cardfileManager.js.map