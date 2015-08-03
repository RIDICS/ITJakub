/// <reference path="../../../Scripts/Plugins/itjakub.plugins.pagination.ts" />
var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, paginationContainer, headwordDescriptionContainer, lazyLoad) {
        this.isRequestToPrint = false;
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.isLazyLoad = lazyLoad;
        this.pagination = new Pagination(paginationContainer);
    }
    DictionaryViewer.prototype.createViewer = function (recordCount, showPageCallback, pageSize, searchCriteria, isCriteriaJson, addNewFavoriteCallback) {
        if (searchCriteria === void 0) { searchCriteria = null; }
        if (isCriteriaJson === void 0) { isCriteriaJson = false; }
        if (addNewFavoriteCallback === void 0) { addNewFavoriteCallback = null; }
        this.recordCount = recordCount;
        this.showPageCallback = showPageCallback;
        this.pageSize = pageSize;
        this.searchCriteria = searchCriteria;
        this.isCriteriaJson = isCriteriaJson;
        this.addNewFavoriteCallback = addNewFavoriteCallback;
        if (this.defaultPageNumber)
            this.pagination.createPagination(this.recordCount, this.pageSize, this.searchAndDisplay.bind(this), this.defaultPageNumber);
        else
            this.pagination.createPagination(this.recordCount, this.pageSize, this.searchAndDisplay.bind(this));
    };
    DictionaryViewer.prototype.setDefaultPageNumber = function (pageNumber) {
        this.defaultPageNumber = pageNumber;
    };
    DictionaryViewer.prototype.goToPage = function (pageNumber) {
        this.pagination.goToPage(pageNumber);
    };
    DictionaryViewer.prototype.searchAndDisplay = function (pageNumber) {
        this.isRequestToPrint = false;
        if (this.recordCount === 0) {
            $(this.headwordListContainer).empty();
            $(this.headwordDescriptionContainer).empty();
            return;
        }
        this.showPageCallback(pageNumber);
    };
    DictionaryViewer.prototype.showHeadwords = function (headwords) {
        var _this = this;
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        this.headwordDescriptionDivs = [];
        this.dictionariesInfo = [];
        this.headwordList = [];
        this.dictionariesMetadataList = headwords.BookList;
        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.HeadwordList.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords.HeadwordList[i];
            var headwordSpan = document.createElement("span");
            $(headwordSpan).text(record.Headword);
            $(headwordSpan).addClass("dictionary-result-headword");
            headwordLi.appendChild(headwordSpan);
            if (this.addNewFavoriteCallback != null) {
                var favoriteGlyphSpan = document.createElement("span");
                favoriteGlyphSpan.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length));
                $(favoriteGlyphSpan).addClass("glyphicon").addClass("glyphicon-star-empty").addClass("dictionary-result-headword-favorite");
                $(favoriteGlyphSpan).click(function (event) {
                    var index = $(event.target).data("entry-index");
                    _this.addNewFavoriteHeadword(index);
                });
                headwordLi.appendChild(favoriteGlyphSpan);
            }
            var dictionaryListDiv = document.createElement("div");
            $(dictionaryListDiv).addClass("dictionary-result-book-list");
            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                var dictionaryMetadata = this.dictionariesMetadataList[dictionary.BookXmlId];
                var currentIndex = this.headwordDescriptionDivs.length;
                // create description
                var mainHeadwordDiv = document.createElement("div");
                if (dictionary.ImageUrl) {
                    var imageCheckBoxDiv = document.createElement("div");
                    var imageCheckBox = document.createElement("input");
                    var imageIconSpan = document.createElement("span");
                    var imageCheckBoxLabel = document.createElement("label");
                    imageCheckBox.type = "checkbox";
                    imageCheckBox.autocomplete = "off";
                    $(imageCheckBox).change(function (event) {
                        _this.updateImageVisibility(event.target);
                    });
                    imageCheckBoxDiv.setAttribute("data-toggle", "buttons");
                    $(imageCheckBoxDiv).addClass("dictionary-entry-image-switch").addClass("btn-group");
                    $(imageIconSpan).addClass("glyphicon").addClass("glyphicon-picture");
                    $(imageCheckBoxLabel).addClass("btn").addClass("btn-primary");
                    imageCheckBoxLabel.appendChild(imageCheckBox);
                    imageCheckBoxLabel.appendChild(imageIconSpan);
                    imageCheckBoxDiv.appendChild(imageCheckBoxLabel);
                    mainHeadwordDiv.appendChild(imageCheckBoxDiv);
                }
                var imageContainerDiv = document.createElement("div");
                $(imageContainerDiv).addClass("dictionary-entry-image");
                var descriptionDiv = document.createElement("div");
                $(mainHeadwordDiv).addClass("loading-background");
                $(descriptionDiv).addClass("dictionary-entry-description-container");
                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);
                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionaryMetadata.BookTitle);
                dictionaryLink.href = "?bookId=" + dictionary.BookXmlId;
                $(dictionaryDiv).addClass("dictionary-entry-name");
                dictionaryDiv.appendChild(dictionaryLink);
                mainHeadwordDiv.appendChild(descriptionDiv);
                mainHeadwordDiv.appendChild(imageContainerDiv);
                mainHeadwordDiv.appendChild(commentsDiv);
                mainHeadwordDiv.appendChild(dictionaryDiv);
                mainHeadwordDiv.appendChild(document.createElement("hr"));
                mainHeadwordDiv.setAttribute("data-entry-index", String(currentIndex));
                this.headwordDescriptionDivs.push(mainHeadwordDiv);
                this.dictionariesInfo.push(dictionary);
                this.headwordList.push(record.Headword);
                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                }
                else {
                    this.getAndShowHeadwordDescription(currentIndex, descriptionDiv);
                }
                descriptionsDiv.appendChild(mainHeadwordDiv);
                // create link
                if (j > 0) {
                    var delimiterSpan = document.createElement("span");
                    $(delimiterSpan).text(" | ");
                    dictionaryListDiv.appendChild(delimiterSpan);
                }
                var aLink = document.createElement("a");
                aLink.href = "#";
                aLink.innerHTML = dictionaryMetadata.BookAcronym;
                aLink.setAttribute("data-entry-index", String(currentIndex));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.Headword, dictionary, descriptionDiv);
                dictionaryListDiv.appendChild(aLink);
            }
            headwordLi.appendChild(dictionaryListDiv);
            listUl.appendChild(headwordLi);
        }
        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);
    };
    DictionaryViewer.prototype.updateImageVisibility = function (checkBox) {
        var _this = this;
        var mainDiv = $(checkBox).closest("[data-entry-index]");
        var imageContainer = $(".dictionary-entry-image", mainDiv);
        if (checkBox.checked) {
            if (imageContainer.hasClass("hidden")) {
                imageContainer.removeClass("hidden");
                return;
            }
            var index = $(mainDiv).data("entry-index");
            var entryInfo = this.dictionariesInfo[index];
            var imageLink = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordImage?bookXmlId=" + entryInfo.BookXmlId + "&entryXmlId=" + entryInfo.EntryXmlId;
            var imageElement = document.createElement("img");
            imageElement.setAttribute("src", imageLink);
            imageContainer.append(imageElement);
            $(imageContainer).addClass("loading");
            imageElement.onload = function () {
                $(imageContainer).removeClass("loading");
            };
            imageElement.onerror = function () {
                $(imageContainer).removeClass("loading");
                $(imageContainer).empty();
                var errorDiv = document.createElement("div");
                $(errorDiv).text("Chyba při načítání obrázku k heslu '" + _this.headwordList[index] + "'.");
                $(errorDiv).addClass("entry-load-error");
                imageContainer.append(errorDiv);
            };
        }
        else {
            imageContainer.addClass("hidden");
        }
    };
    DictionaryViewer.prototype.addNewFavoriteHeadword = function (index) {
        var dictionaryInfo = this.dictionariesInfo[index];
        this.addNewFavoriteCallback(dictionaryInfo.BookXmlId, dictionaryInfo.EntryXmlId);
    };
    DictionaryViewer.prototype.createLinkListener = function (aLink, headword, headwordInfo, container) {
        var _this = this;
        $(aLink).click(function (event) {
            event.preventDefault();
            var index = $(event.target).data("entry-index");
            var headwordDiv = _this.headwordDescriptionDivs[index];
            for (var k = 0; k < _this.headwordDescriptionDivs.length; k++) {
                $(_this.headwordDescriptionDivs[k]).addClass("hidden");
            }
            $(headwordDiv).removeClass("hidden");
            if ($(headwordDiv).hasClass("lazy-loading")) {
                _this.loadHeadwordDescription(index);
            }
        });
    };
    DictionaryViewer.prototype.prepareLazyLoad = function (mainDescriptionElement) {
        var _this = this;
        $(mainDescriptionElement).addClass("lazy-loading");
        $(mainDescriptionElement).bind("appearing", function (event) {
            var descriptionDiv = event.target;
            var index = $(descriptionDiv).data("entry-index");
            _this.loadHeadwordDescription(index);
        });
    };
    DictionaryViewer.prototype.showLoadHeadword = function (response, container) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        container.innerHTML = response;
        if (this.isRequestToPrint)
            this.print();
    };
    DictionaryViewer.prototype.showLoadError = function (headword, container) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        var errorDiv = document.createElement("div");
        $(errorDiv).text("Chyba při náčítání hesla '" + headword + "'.");
        $(errorDiv).addClass("entry-load-error");
        container.appendChild(errorDiv);
        if (this.isRequestToPrint)
            this.print();
    };
    DictionaryViewer.prototype.loadImageOnError = function (index, container) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        var mainDiv = this.headwordDescriptionDivs[index];
        var headwordDescriptionContainer = $(".dictionary-entry-description-container", mainDiv);
        var toggleButtonLabel = $(".dictionary-entry-image-switch label", mainDiv);
        var checkBox = $("input", toggleButtonLabel);
        var headwordLabelSpan = document.createElement("span");
        $(headwordLabelSpan).addClass("entry-image-header");
        $(headwordLabelSpan).text(this.headwordList[index]);
        headwordDescriptionContainer.append(headwordLabelSpan);
        if (checkBox.length !== 0 && !checkBox.get(0).checked) {
            toggleButtonLabel.trigger("click");
        }
        if (this.isRequestToPrint)
            this.print();
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescription = function (headwordIndex, container) {
        if (this.searchCriteria == null)
            this.getAndShowHeadwordDescriptionBasic(headwordIndex, container);
        else
            this.getAndShowHeadwordDescriptionFromSearch(headwordIndex, container);
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescriptionBasic = function (headwordIndex, container) {
        var _this = this;
        var headword = this.headwordList[headwordIndex];
        var headwordInfo = this.dictionariesInfo[headwordIndex];
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescription",
            data: {
                bookGuid: headwordInfo.BookXmlId,
                xmlEntryId: headwordInfo.EntryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.showLoadHeadword(response, container);
            },
            error: function () {
                if (!headwordInfo.ImageUrl) {
                    _this.showLoadError(headword, container);
                }
                else {
                    _this.loadImageOnError(headwordIndex, container);
                }
            }
        });
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescriptionFromSearch = function (headwordIndex, container) {
        var _this = this;
        var headword = this.headwordList[headwordIndex];
        var headwordInfo = this.dictionariesInfo[headwordIndex];
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescriptionFromSearch",
            data: {
                criteria: this.searchCriteria,
                isCriteriaJson: this.isCriteriaJson,
                bookGuid: headwordInfo.BookXmlId,
                xmlEntryId: headwordInfo.EntryXmlId
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.showLoadHeadword(response, container);
            },
            error: function () {
                if (!headwordInfo.ImageUrl) {
                    _this.showLoadError(headword, container);
                }
                else {
                    _this.loadImageOnError(headwordIndex, container);
                }
            }
        });
    };
    DictionaryViewer.prototype.loadHeadwordDescription = function (index) {
        var mainDescriptionDiv = this.headwordDescriptionDivs[index];
        var descriptionContainer = $(".dictionary-entry-description-container", mainDescriptionDiv).get(0);
        $(mainDescriptionDiv).unbind("appearing");
        $(mainDescriptionDiv).removeClass("lazy-loading");
        this.getAndShowHeadwordDescription(index, descriptionContainer);
    };
    DictionaryViewer.prototype.isAllLoaded = function () {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".loading-background", descriptions);
        var notLoadedVisible = notLoaded.filter(":not(.hidden)");
        return notLoadedVisible.length === 0;
    };
    DictionaryViewer.prototype.loadAllHeadwords = function () {
        for (var i = 0; i < this.headwordDescriptionDivs.length; i++) {
            var descriptionDiv = this.headwordDescriptionDivs[i];
            if ($(descriptionDiv).hasClass("lazy-loading") && !$(descriptionDiv).hasClass("hidden")) {
                this.loadHeadwordDescription(i);
            }
        }
    };
    DictionaryViewer.prototype.showPrintModal = function () {
        if (this.isRequestToPrint)
            return;
        $("#print-modal").modal({
            backdrop: "static",
            show: true
        });
    };
    DictionaryViewer.prototype.hidePrintModal = function () {
        $("#print-modal").modal("hide");
    };
    DictionaryViewer.prototype.print = function () {
        // check if all entries are loaded
        if (!this.isAllLoaded()) {
            this.showPrintModal();
            this.isRequestToPrint = true;
            if (this.isLazyLoad) {
                this.loadAllHeadwords();
            }
            return;
        }
        this.isRequestToPrint = false;
        this.hidePrintModal();
        window.print();
    };
    return DictionaryViewer;
})();
//# sourceMappingURL=itjakub.dictionariesViewer.js.map