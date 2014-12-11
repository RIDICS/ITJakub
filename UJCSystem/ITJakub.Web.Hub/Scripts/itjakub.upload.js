$(document).ready(function() {

    Dropzone.autoDiscover = false; // otherwise will be initialized twice

    $("#dropzoneFileForm").dropzone({
        url: '/Upload/UploadFile',
        maxFilesize: 10000, // MB
        uploadMultiple: true,
        clickable: "#dropzoneFileFormPreview",
        autoProcessQueue: true,
        previewsContainer: "#dropzoneFileFormPreview",
        acceptedFiles: '.doc,.docx, .jpg, .jpeg, .png, .gif, .bmp,',
        dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jiny soubor s priponou .doc, .docx, .jpg, .jpeg, .png, .gif nebo .bmp,',

        init: function() {
            var fileDropzone = this;

            //this.element.querySelector("input[type=submit]").addEventListener("click", function(e) {
            //    e.preventDefault();
            //    e.stopPropagation();
            //    fileDropzone.processQueue();
            //});

            //this.on("addedfile", function() {
            //    if (this.files[1] != null) {
            //        this.removeFile(this.files[0]);
            //    }
            //});

            //this.on("drop", function(event) {
            //    if (this.getQueuedFiles().length == 0 && this.getUploadingFiles().length == 0) {
            //        var _this = this;
            //        _this.removeAllFiles();
            //    }
            //});

            //this.on("successmultiple", function(files, response) {
            //    saveFileGuidToPage(response.FileInfo.FileGuid);
            //    saveVersionIdToPage(response.FileInfo.VersionId);
            //});


        }

    });


    $('#ProcessUploadedButton').click(function() {
        $('#upload').hide();
        $('#processing').show();
        $.ajax({
            type: "POST",
            url: "/Upload/ProcessUploadedFiles",
            data: JSON.stringify({ 'sessionId': getSessionIdFromPage() }),
            dataType: 'json',
            contentType: 'application/json',
            success: function(response) {
                var done = $('#done');
                if (response.success == true) {
                    done.find('.success').show();
                } else {
                    done.find('.error').show();
                }
                $('#processing').hide();
                done.show();

            }
        });
    });

    $("#progressbar").progressbar({
        value: false
    });


    function getSessionIdFromPage() {
        return $('#sessionId').val();
    }

});


//$("#dropzoneFrontImageForm").dropzone({
//    url: '/Upload/UploadFrontImage',
//    maxFilesize: 100, // MB
//    maxFiles: 1,
//    acceptedFiles: '.png,.jpg,.gif,.bmp',
//    dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jiny soubor s priponou .png, .jpg, .gif nebo .bmp',
//    success: function(file, response) {
//        if (response.Error) {
//            alert(response.Error);
//        }
//    },
//    sending: function(file, xhr, formData) {
//        formData.append('fileGuid', getFileGuidFromPage());
//    }

//});

//$("#dropzoneImagesForm").dropzone({
//    url: '/Upload/UploadImages',
//    maxFilesize: 100, // MB
//    acceptedFiles: '.png,.jpg,.gif,.bmp',
//    dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jine soubory s priponou .png, .jpg, .gif nebo .bmp',
//    success: function(file, response) {
//        if (response.Error) {
//            alert(response.Error);
//        }
//    },
//    sending: function(file, xhr, formData) {
//        formData.append('fileGuid', getFileGuidFromPage());
//    }
//});

////fill authors table
//$.get('/Author/GetAllAuthors', function(result) {
//    $.each(result.Authors, function(index, author) {
//        addAuthorToTable(author.Id, getAutorAsString(author.AuthorInfos));
//    });
//});

//});

//function saveFileGuidToPage(fileGuid) {
//    $('ol.upload-file').attr('data-file-guid', fileGuid);
//}

//function saveVersionIdToPage(versionId) {
//    $('ol.upload-file').attr('data-version-id', versionId);
//}

//function getFileGuidFromPage() {
//    return $('ol.upload-file').attr('data-file-guid');
//}

//function getVersionIdFromPage() {
//    return $('ol.upload-file').attr('data-version-id');
//}

//$('#fileMetadataForm').submit(function () {
//    var authorIds = [];
//    $.each($('#authorsTableBody input:checked'), function (index, checked) {
//        authorIds.push($(checked).val());
//    });
//        $.ajax({
//            type: "POST",
//            url: "/Author/AssignAuthorsToBook",
//            data: JSON.stringify({ 'bookGuid': getFileGuidFromPage(), 'bookVersionGuid': getVersionIdFromPage(), 'authorIds': authorIds }),
//            dataType: 'json',
//            contentType: 'application/json',
//            success: function (response) {
//                if (response.Error) {
//                    alert(response.Error);
//                } else {
//                    alert("uzivatele byli prirazeni");//TODO tell in better way that authors were assigned
//                }
//            }
//        });
//    return false;
//});

//function fillFileMetadataForm(data) {
//    var form = document.getElementById("fileMetadataForm");

//    var labelNameInput = document.createElement('label');
//    labelNameInput.setAttribute('for', 'nameInput');
//    labelNameInput.innerHTML = 'Název díla:';

//    var nameInput = document.createElement('input');
//    nameInput.setAttribute('type', 'text');
//    nameInput.setAttribute('placeholder', 'Název díla:');
//    nameInput.setAttribute('name', 'name');
//    nameInput.setAttribute('id', 'nameInput');
//    nameInput.setAttribute('value', data.FileInfo.Name);

//    var labelAuthorInput = document.createElement('label');
//    labelAuthorInput.setAttribute('for', 'authorInput');
//    labelAuthorInput.innerHTML = 'Jméno autora:';

//    var authorInput = document.createElement('input');
//    authorInput.setAttribute('type', 'text');
//    authorInput.setAttribute('placeholder', 'Autor');
//    authorInput.setAttribute('name', 'author');
//    authorInput.setAttribute('id', 'authorInput');
//    authorInput.setAttribute('value', data.FileInfo.Author);


//    var submit = document.createElement('input');
//    submit.setAttribute('type', 'submit');
//    submit.setAttribute('value', 'Potvrdit');

//    form.appendChild(labelNameInput);
//    form.appendChild(nameInput);
//    form.appendChild(labelAuthorInput);
//    form.appendChild(authorInput);
//    form.appendChild(submit);
//}

//$("#fileMetadataForm").submit(function () {
//    var formData = $("#fileMetadataForm").serializeArray();
//    formData.push({ name: "fileGuid", value: getUploladingFileGuid() });
//    $.ajax({
//        type: "POST",
//        url: "/Upload/UploadMetadata",
//        data: formData,
//        success: function (response) {
//            if (response.Error) {
//                alert(response.Error);
//            }
//        }
//    });
//    return false; // prevent submit of the form.
//});


//$("#searchInput").keyup(function() {
//    //split the current value of searchInput
//    var data = this.value.split(" ");
//    //create a jquery object of the rows
//    var jo = $("#authorsTableBody").find("tr");
//    if (this.value == "") {
//        jo.show();
//        return;
//    }
//    //hide all the rows
//    jo.hide();

//    //Recusively filter the jquery object to get results.
//    jo.filter(function(i, v) {
//            var $t = $(this);
//            for (var d = 0; d < data.length; ++d) {
//                if ($t.is(":contains('" + data[d] + "')")) {
//                    return true;
//                }
//            }
//            return false;
//        })
//        //show the rows that match.
//        .show();
//}).focus(function() {
//    this.value = "";
//    $(this).css({
//        "color": "black"
//    });
//    $(this).unbind('focus');
//}).css({
//    "color": "#C0C0C0"
//});

//$('#addAuthorButton')
//    .click(function(event) {
//        $('#addAuthorModal').modal('show');
//    });

//$('#createAuthorForm').submit(function () {
//    var formData= [];
//    $.each($("#createAuthorForm div.author-info"), function (index, authorInfo) {
//        var infoObject = {};
//        infoObject["Text"] = $(authorInfo).find($("input[name='Text']")).val();
//        infoObject["TextType"] = $(authorInfo).find($("select[name='TextType']")).val();
//        formData.push(infoObject);
//    });
//    createAuthor(formData);
//    $('#addAuthorModal').modal('hide');
//    return false; // prevent submit of the form.
//});

//$('#authorsTable tr')
//   .click(function (event) {
//       var _this = this;
//       $(_this).toggleClass('selected');
//       if (event.target.type !== 'checkbox') {

//           var checkbox = $(_this).find(':checkbox');
//           $(checkbox).attr('checked', !$(checkbox).is(':checked'));
//       }
//   });

//function getAutorAsString(authorData) {
//    var result = "";
//    $.each(authorData, function (index, authorInfo) {
//        result += authorInfo["Text"]+" ";
//    });
//    return result;
//}

//function createAuthor(data) {
//    var authorName = getAutorAsString(data);
//    $.ajax({
//        url: '/Author/CreateAuthor',
//        type: 'POST',
//        data: JSON.stringify(data),
//        dataType: 'json',
//        async: false,
//        contentType: 'application/json',
//        success: function(response) {
//            addAuthorToTable(response.AuthorId, authorName);
//        }
//    });
//}


//function addAuthorToTable(authorId, authorName) {
//    var tr = document.createElement('tr');
//    var checkboxTd = document.createElement('td');
//    var checkbox = document.createElement('input');
//    checkbox.type = "checkbox";
//    checkbox.name = "author";
//    checkbox.value = authorId;
//    var nameTd = document.createElement('td');
//    nameTd.appendChild(document.createTextNode(authorName));

//    checkboxTd.appendChild(checkbox);
//    tr.appendChild(checkboxTd);
//    tr.appendChild(nameTd);
//    document.getElementById('authorsTableBody').appendChild(tr);
//}

//$('#addAuthorInfo')
//    .click(function (event) {
//        var authorInfo = document.createElement("div");
//        authorInfo.innerHTML='<div class="form-group author-info" ><label>Text části jména:</label><input type="text" name="Text" class="form-control" placeholder="cast jmena"><label>Typ části jména:</label><select class="form-control" name="TextType"><option value="1">Křestní</option><option value="2">Příjmení</option><option value="3">Střední jméno</option><option value="4">Rodné místo</option><option value="5">Přezdívka</option><option value="6">Číslovka</option><option value="0">Jiné</option></select></div>'; //TODO rewrite to javascript
//        $(authorInfo).insertBefore($("#createAuthorForm button").first());
//});