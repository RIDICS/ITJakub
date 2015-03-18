//window.onload = () => { alert("hello from editions!"); }


//TODO methods below are for testing purposes of Bibliography module
$(document).ready(function () {
    var searchPlugin: SearchModule = new SearchModule(new BibliographyModule('#bibliographyList', '#sortBarBibList'));

    $('#fillBibList10').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 10; i++) {
            arrayOfIds.push(i.toString());
        }
        arrayOfIds[0] = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}";
        searchPlugin.getBookWithIds(arrayOfIds);
    });

    $('#fillBibList3').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 3; i++) {
            arrayOfIds.push(i.toString());
        }
        arrayOfIds[0] = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}";
        searchPlugin.getBookWithIds(arrayOfIds);
    });
    $('#fillBibList1000').click(function () {
        var arrayOfIds: string[] = new Array();
        for (var i = 0; i < 1000; i++) {
            arrayOfIds.push(i.toString());
        }
        arrayOfIds[0] = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}";
        searchPlugin.getBookWithIds(arrayOfIds);
    });


    $('#fillBibListTypeEdition').click(function () {
        searchPlugin.getBookWithType('Edition');
    });
    $('#fillBibListTypeDictionary').click(function () {
        searchPlugin.getBookWithType('Dictionary');
    });


    //var readerPlugin: ReaderModule = new ReaderModule('#Reader');
    //var book = new BookInfo();
    //readerPlugin.makeReader(book);

});
