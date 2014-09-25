$(document).ready(function () {

    Dropzone.autoDiscover = false; // otherwise will be initialized twice

    $("#dropzoneFileForm").dropzone({
        url: '/Upload/UploadFile',
        maxFilesize: 10000, // MB
        maxFiles: 1,
        uploadMultiple: true,
        clickable: "#dropzoneFileFormPreview",
        autoProcessQueue: false,
        previewsContainer: "#dropzoneFileFormPreview",
        acceptedFiles: '.doc,.docx',
        dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jiny soubor s priponou .doc nebo .docx',

        init: function() {
            var fileDropzone = this;

            this.element.querySelector("input[type=submit]").addEventListener("click", function(e) {
                e.preventDefault();
                e.stopPropagation();
                fileDropzone.processQueue();
            });

            this.on("addedfile", function () {
                if (this.files[1] != null) {
                    this.removeFile(this.files[0]);
                }
            });

            this.on("drop", function (event) {
                if (this.getQueuedFiles().length == 0 && this.getUploadingFiles().length == 0) {
                    var _this = this;
                    _this.removeAllFiles();
                }
            });

            this.on("successmultiple", function(files, response) {
                saveFileGuidToPage(response.FileInfo.FileGuid);
                saveVersionIdToPage(response.FileInfo.VersionId);
            });
        }
    });

    $("#dropzoneFrontImageForm").dropzone({
        url: '/Upload/UploadFrontImage',
        maxFilesize: 100, // MB
        maxFiles: 1,
        acceptedFiles: '.png,.jpg,.gif,.bmp',
        dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jiny soubor s priponou .png, .jpg, .gif nebo .bmp',
        success: function(file, response) {
            if (response.Error) {
                alert(response.Error);
            }
        },
        sending: function(file, xhr, formData) {
            formData.append('fileGuid', getFileGuidFromPage());
        }

    });

    $("#dropzoneImagesForm").dropzone({
        url: '/Upload/UploadImages',
        maxFilesize: 100, // MB
        acceptedFiles: '.png,.jpg,.gif,.bmp',
        dictInvalidFileType: 'Tento format neni podporovany. Vyberte prosim jine soubory s priponou .png, .jpg, .gif nebo .bmp',
        success: function(file, response) {
            if (response.Error) {
                alert(response.Error);
            }
        },
        sending: function(file, xhr, formData) {
            formData.append('fileGuid', getFileGuidFromPage());
        }
    });

});

function saveFileGuidToPage(fileGuid) {
    $('ol.upload-file').attr('data-file-guid', fileGuid);
}

function saveVersionIdToPage(versionId) {
    $('ol.upload-file').attr('data-version-id', versionId);
}

function getFileGuidFromPage() {
    $('ol.upload-file').attr('data-file-guid');
}

function getVersionIdFromPage() {
    $('ol.upload-file').attr('data-version-id');
}

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