$(document).ready(function() {

    Dropzone.autoDiscover = false; // otherwise will be initialized twice
    function disableUploadButton() {
        $("#ProcessUploadedButton").attr("disabled", true);
    }

    function enableUploadButton() {
        $("#ProcessUploadedButton").removeAttr("disabled");

    }

    function getSessionIdFromPage() {
        return $("#sessionId").val();
    }

    function getUploadMessage() {
        return $("#uploadMessage").val();
    }

 
    $("#dropzoneFileForm").dropzone({
    	url: "../Upload/UploadFile",
        maxFilesize: 10000, // MB
        uploadMultiple: true,
        clickable: "#dropzoneFileFormPreview",
        autoProcessQueue: true,
        paralelUploads: 5,
        previewsContainer: "#dropzoneFileFormPreview",
        acceptedFiles: ".doc, .docx, .jpg, .jpeg, .png, .bmp, .gif, .xsl, .xslt, .xmd, .mp3, .ogg, .wav",
        dictInvalidFileType: "Tento format neni podporovany. Vyberte prosim jiny soubor s priponou .doc, .docx, .jpg, .jpeg, .png, .bmp, .gif, .xsl, .xslt, .xmd, .mp3, .ogg, .wav",

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


        }

    });


    $("#ProcessUploadedButton").click(function() {
        $("#upload").hide();
        $("#processing").show();

        //TODO COMMENT THIS Line FOR MUSIC DISABLE
        readyplayer.playVideo();


        $.ajax({
            type: "POST",
            url: "../Upload/ProcessUploadedFiles",
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
