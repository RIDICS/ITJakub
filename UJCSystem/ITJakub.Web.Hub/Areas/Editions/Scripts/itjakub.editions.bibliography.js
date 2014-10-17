var BibliographyModul = (function () {
    function BibliographyModul() {
        this.bibliographyModulControllerUrl = "";
        if (BibliographyModul._instance) {
            throw new Error("Cannot instantiate...Use getInstance method instead");
        }
        BibliographyModul._instance = this;
    }
    BibliographyModul.getInstance = function () {
        if (BibliographyModul._instance === null) {
            BibliographyModul._instance = new BibliographyModul();
        }
        return BibliographyModul._instance;
    };

    BibliographyModul.prototype.showBibliographies = function (bookIds, container) {
        var _this = this;
        $(container).empty();
        var rootElement = document.createElement('ul');
        $(rootElement).addClass('listing');

        var items = bookIds;

        $.each(items, function (index, item) {
            var bibliographyHtml = _this.makeBibliography(item);
            rootElement.appendChild(bibliographyHtml);
        });
        $(container).append(rootElement);
    };

    BibliographyModul.prototype.makeBibliography = function (bibItem) {
        var liElement = document.createElement('li');
        $(liElement).addClass('list-item');

        var visibleContent = document.createElement('div');
        $(visibleContent).addClass('visible-content');

        var rightPanel = document.createElement('div');
        $(rightPanel).addClass('right-panel');

        var bookButton = document.createElement('button');
        bookButton.type = 'button';
        $(bookButton).addClass('btn btn-sm book-button');
        var spanBook = document.createElement('span');
        $(spanBook).addClass('glyphicon glyphicon-book');
        bookButton.appendChild(spanBook);
        rightPanel.appendChild(bookButton);

        var infoButton = document.createElement('button');
        infoButton.type = 'button';
        $(infoButton).addClass('btn btn-sm information-button');
        var spanInfo = document.createElement('span');
        $(spanInfo).addClass('glyphicon glyphicon-info-sign');
        infoButton.appendChild(spanInfo);
        rightPanel.appendChild(infoButton);

        var showContentButton = document.createElement('button');
        showContentButton.type = 'button';
        $(showContentButton).addClass('btn btn-sm show-button');
        var spanChevrDown = document.createElement('span');
        $(spanChevrDown).addClass('glyphicon glyphicon-chevron-down');
        showContentButton.appendChild(spanChevrDown);
        $(showContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').show("slow");
            $(this).siblings('.hide-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(showContentButton);

        var hideContentButton = document.createElement('button');
        hideContentButton.type = 'button';
        $(hideContentButton).addClass('btn btn-sm hide-button');
        var spanChevrUp = document.createElement('span');
        $(spanChevrUp).addClass('glyphicon glyphicon-chevron-up');
        hideContentButton.appendChild(spanChevrUp);
        $(hideContentButton).click(function (event) {
            $(this).parents('li.list-item').first().find('.hidden-content').hide("slow");
            $(this).siblings('.show-button').show();
            $(this).hide();
        });
        rightPanel.appendChild(hideContentButton);

        visibleContent.appendChild(rightPanel);

        var middlePanel = document.createElement('div');
        $(middlePanel).addClass('middle-panel');
        middlePanel.innerHTML = "Fusce tellus odio, dapibus id fermentum quis, suscipit id erat.Aliquam ante.Vestibulum fermentum tortor id mi.Sed convallis magna eu sem.Curabitur ligula sapien, pulvinar a vestibulum quis, facilisis vel sapien.Nullam faucibus mi quis velit.Nunc tincidunt ante vitae massa.Duis bibendum, lectus ut viverra rhoncus, dolor nunc faucibus libero, eget facilisis enim ipsum id lacus.Etiam bibendum elit eget erat.";
        visibleContent.appendChild(middlePanel);

        $(liElement).append(visibleContent);

        var hiddenContent = document.createElement('div');
        $(hiddenContent).addClass('hidden-content');
        hiddenContent.innerHTML = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.Maecenas aliquet accumsan leo.Donec quis nibh at felis congue commodo.Phasellus et lorem id felis nonummy placerat.Quisque porta.Vivamus porttitor turpis ac leo.Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.Fusce tellus.Nunc dapibus tortor vel mi dapibus sollicitudin.Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Quisque porta.Nullam at arcu a est sollicitudin euismod.Donec ipsum massa, ullamcorper in, auctor et, scelerisque sed, est.Integer vulputate sem a nibh rutrum consequat.Aliquam erat volutpat.Duis sapien nunc, commodo et, interdum suscipit, sollicitudin et, dolor.Proin pede metus, vulputate nec, fermentum fringilla, vehicula vitae, justo.Mauris tincidunt sem sed arcu.Etiam neque.";
        $(liElement).append(hiddenContent);

        return liElement;
    };
    BibliographyModul._instance = null;
    return BibliographyModul;
})();
//# sourceMappingURL=itjakub.editions.bibliography.js.map
