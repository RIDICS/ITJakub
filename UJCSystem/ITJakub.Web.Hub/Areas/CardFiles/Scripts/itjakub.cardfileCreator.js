var CardFileCreator = (function () {
    function CardFileCreator(cardFilesContainer) {
        this.cardFilesContainer = cardFilesContainer;
    }
    CardFileCreator.prototype.makeCardFile = function () {
        var cardFileDiv = document.createElement("div");
        $(cardFileDiv).addClass("cardfile-listing");
        this.makeLeftPanel(cardFileDiv);
        this.makeRightPanel(cardFileDiv);
        $(this.cardFilesContainer).append(cardFileDiv);
    };
    CardFileCreator.prototype.makeLeftPanel = function (cardFileDiv) {
        var cardFileLeftPanelDiv = document.createElement("div");
        $(cardFileLeftPanelDiv).addClass("card-file-listing-left-panel");
        var headwordDiv = document.createElement("div");
        $(headwordDiv).addClass("headword-description");
        var headwordNameSpan = document.createElement("span");
        $(headwordNameSpan).addClass("headword-name");
        headwordNameSpan.innerText = "netbalivy"; //TODO load from parameter
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
    CardFileCreator.prototype.makeRightPanel = function (cardFileDiv) {
        var cardFileRightPanelDiv = document.createElement("div");
        $(cardFileRightPanelDiv).addClass("card-file-listing-right-panel");
        var cardFileDescDiv = document.createElement("div");
        $(cardFileDescDiv).addClass("cardfile-description");
        cardFileDescDiv.innerText = "Kartotéka: ";
        var cardFileNameSpan = document.createElement("span");
        $(cardFileNameSpan).addClass("cardfile-name");
        cardFileNameSpan.innerText = "Excerpce"; //TODO load from parameter
        cardFileDescDiv.appendChild(cardFileNameSpan);
        cardFileRightPanelDiv.appendChild(cardFileDescDiv);
        var cardFileDrawerDescDiv = document.createElement("div");
        $(cardFileDrawerDescDiv).addClass("cardfile-drawer-description");
        cardFileDrawerDescDiv.innerText = "Zásuvka: ";
        var cardFileDrawerNameSpan = document.createElement("span");
        $(cardFileDrawerNameSpan).addClass("cardfile-drawer-name");
        cardFileDrawerNameSpan.innerText = "netbalivy-odymovati"; //TODO load from parameter
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
        cardFileHeadwordTextSpan.innerText = "netbalivy"; //TODO load from parameter
        cardFileHeadwordDescDiv.appendChild(cardFileHeadwordTextSpan);
        cardFileRightPanelDiv.appendChild(cardFileHeadwordDescDiv);
        var cardFileNoticeDiv = document.createElement("div");
        $(cardFileNoticeDiv).addClass("cardfile-notice");
        cardFileNoticeDiv.innerText = "Upozornění: ";
        var cardFileNoticeSpan = document.createElement("span");
        $(cardFileNoticeSpan).addClass("cardfile-notice-text");
        cardFileNoticeSpan.innerText = "upozorn"; //TODO load from parameter
        cardFileNoticeDiv.appendChild(cardFileNoticeSpan);
        cardFileRightPanelDiv.appendChild(cardFileNoticeDiv);
        var cardFileNoteDiv = document.createElement("div");
        $(cardFileNoteDiv).addClass("cardfile-note");
        cardFileNoteDiv.innerText = "Poznámka: ";
        var cardFileNoteSpan = document.createElement("span");
        $(cardFileNoteSpan).addClass("cardfile-note-text");
        cardFileNoteSpan.innerText = "poznamk"; //TODO load from parameter
        cardFileNoteDiv.appendChild(cardFileNoteSpan);
        cardFileRightPanelDiv.appendChild(cardFileNoteDiv);
        cardFileDiv.appendChild(cardFileRightPanelDiv);
    };
    CardFileCreator.prototype.makeSlider = function (pageControlsDiv) {
        var sliderDiv = document.createElement("div");
        $(sliderDiv).addClass("slider");
        $(sliderDiv).slider({
            min: 0,
            max: 100,
            value: 0,
            start: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            },
            stop: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
            },
            slide: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.slider-tip').stop(true, true);
                $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Lístek - 2047 <br/> Heslo - xxx");
            }
        });
        var sliderTooltip = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);
        var innerTooltip = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html("Lístek - 2046 <br/> Heslo - netbalivý");
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
    return CardFileCreator;
})();
//# sourceMappingURL=itjakub.cardfileCreator.js.map