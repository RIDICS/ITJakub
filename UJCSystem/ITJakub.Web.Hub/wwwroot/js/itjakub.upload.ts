$(document).ready(function() {

    Dropzone.autoDiscover = false; // otherwise will be initialized twice
    function disableUploadButton() {
        $("#ProcessUploadedButton").prop("disabled", true);
    }

    function enableUploadButton() {
        $("#ProcessUploadedButton").prop("disabled", false);

    }

    function getSessionIdFromPage() {
        return $("#sessionId").val();
    }

    function getUploadMessage() {
        return $("#uploadMessage").val();
    }

    const acceptedFiles = ".doc, .docx, .jpg, .jpeg, .png, .bmp, .gif, .xsl, .xslt, .xmd, .xml, .mp3, .ogg, .wav, .zip";
    $("#dropzoneFileForm").dropzone({
    	url: getBaseUrl() + "Upload/UploadFile",
        maxFilesize: 10000, // MB
        uploadMultiple: true,
        clickable: "#dropzoneFileFormPreview",
        autoProcessQueue: true,
        parallelUploads: 5,
        previewsContainer: "#dropzoneFileFormPreview",
        acceptedFiles: acceptedFiles,

        dictInvalidFileType: "Tento formát není podporovaný. Vyberte prosím jiný soubor s příponou " + acceptedFiles,
        dictDefaultMessage: "Pro nahrávání sem přesuňte soubory",
        dictFallbackMessage: "Váš prohlížeč nepodporuje nahrávání souborů pomocí drag'n'drop.",
        dictFallbackText: "Použijte prosím záložní formulář umíštěný níže.",
        dictFileTooBig: "Velikost souboru ({{filesize}}MB) překročila maximální povolenou velikost {{maxFilesize}}MB.",
        dictResponseError: "Server odpověděl se stavovým kódem {{statusCode}}.",
        dictCancelUpload: "Zrušit nahrávání",
        dictCancelUploadConfirmation: "Opravdu chcete zrušit nahrávání tohoto souboru?",
        dictRemoveFile: "Odstranit soubor",
        dictRemoveFileConfirmation: null,
        dictMaxFilesExceeded: "Nelze nahrát žádné další soubory.",

        init: function() {
            var fileDropzone = this;

            this.on("complete", function(file) {
                if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                    enableUploadButton();
                }
            });


            this.on("addedfile", function() {
                disableUploadButton();
            });

            //this.element.querySelector("input[type=submit]").addEventListener("click", function(e) {
            //    e.preventDefault();
            //    e.stopPropagation();
            //    fileDropzone.processQueue();
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


        },
        error: function(file, message, xhr) {
            var errorMessage = xhr
                ? this.options.dictResponseError.replace("{{statusCode}}", xhr.status.toString())
                : message;
            this.defaultOptions.error(file, errorMessage, xhr);
        }

    });


    $("#ProcessUploadedButton").click(function() {
        $("#upload").hide();
        $("#processing").show();

        //TODO COMMENT THIS Line FOR MUSIC DISABLE
        //readyplayer.playVideo();


        $.ajax({
            type: "POST",
            url: getBaseUrl() + "Upload/ProcessUploadedFiles",
            data: JSON.stringify({ 'sessionId': getSessionIdFromPage(), 'uploadMessage': getUploadMessage() }),
            dataType: "json",
            contentType: "application/json",
            success: function(response) {
                var done = $("#done");
                if (response.success === true) {
                    done.find(".success").show();
                } else {
                    done.find(".error").show();
                }
                $("#processing").hide();
                done.show();

                //TODO COMMENT THIS Line FOR MUSIC DISABLE
                stopVideo();

            },
            error: function (xmlHttpRequest, textStatus, errorMessage) {
                var done = $("#done");
                var error = done.find(".error");
                error.children(".message").append("Chyba: " + errorMessage);
                error.show();
                $("#processing").hide();
                done.show();
            }
            
        });
    });

    $("#progressbar").progressbar({
        value: false
    });    

});


//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
var tag = document.createElement('script');
tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

declare var YT; // type definition for YouTube player class

var player;
var readyplayer;
function onYouTubeIframeAPIReady() {
    player = new YT.Player('music', {
        height: '0',
        width: '0',
        videoId: 'xwuwl46kYko',
        events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });
}

function onPlayerReady(event) {
    readyplayer = event.target;
    //event.target.playVideo();
}

var done = false;
function onPlayerStateChange(event) {
    //if (event.data == YT.PlayerState.PLAYING && !done) {
    //    setTimeout(stopVideo, 6000);
    //    done = true;
    //}
}
function stopVideo() {
    player.stopVideo();
}
//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
