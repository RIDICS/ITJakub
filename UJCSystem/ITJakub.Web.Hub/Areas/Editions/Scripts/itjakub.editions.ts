//window.onload = () => { alert("hello from editions!"); }

/// <reference path='itjakub.editions.bibliography.ts'/>

$(document).ready(function() {
    $('#fillBibList10').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 10; i++) {
            arrayOfIds.push(i.toString());
        }

        BibliographyModul.getInstance().showBibliographies(arrayOfIds, '#bibliographyList');
    });

    $('#fillBibList3').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 3; i++) {
            arrayOfIds.push(i.toString());
        }

        BibliographyModul.getInstance().showBibliographies(arrayOfIds, '#bibliographyList');
    });
    $('#fillBibList1000').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 1000; i++) {
            arrayOfIds.push(i.toString());
        }

        BibliographyModul.getInstance().showBibliographies(arrayOfIds, '#bibliographyList');
    });
});
