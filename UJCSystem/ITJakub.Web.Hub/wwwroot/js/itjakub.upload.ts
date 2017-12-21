$(document).ready(() => {
    var bookUploader = new BookUploader();
    bookUploader.init();
});

class BookUploader {
    public init() {
        var self = this;
        var dropzoneOptions = DropzoneHelper.getFullConfiguration({
            url: getBaseUrl() + "Upload/UploadFile",
            init: function () {
                var fileDropzone: Dropzone = this;

                fileDropzone.on("complete", function (file) {
                    if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                        self.enableUploadButton();
                    }
                });


                fileDropzone.on("addedfile", function () {
                    self.disableUploadButton();
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
            error: DropzoneHelper.getErrorFunction()
        });

        $("#dropzoneFileForm").dropzone(dropzoneOptions);

        $("#ProcessUploadedButton").click(() => {
            $("#upload").hide();
            $("#processing").show();

            //TODO COMMENT THIS Line FOR MUSIC DISABLE
            //player.playVideo();
            

            $.ajax({
                type: "POST",
                url: getBaseUrl() + "Upload/ProcessUploadedFiles",
                data: JSON.stringify({ 'sessionId': this.getSessionIdFromPage(), 'uploadMessage': this.getUploadMessage() }),
                dataType: "json",
                contentType: "application/json",
                success: response => {
                    var done = $("#done");
                    if (response.success === true) {
                        done.find(".success").show();
                    } else {
                        done.find(".error").show();
                    }
                    $("#processing").hide();
                    done.show();

                    //TODO COMMENT THIS Line FOR MUSIC DISABLE
                    //player.stopVideo();
                },
                error: (xmlHttpRequest, textStatus, errorMessage) => {
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

        //TODO COMMENT THIS 2 lines FOR MUSIC DISABLE
        //var easterEgg = new YouTubeEasterEgg();
        //easterEgg.init();
    }

    public disableUploadButton() {
        $("#ProcessUploadedButton").prop("disabled", true);
    }

    public enableUploadButton() {
        $("#ProcessUploadedButton").prop("disabled", false);
    }

    public getSessionIdFromPage(): string {
        return $("#sessionId").val() as string;
    }

    public getUploadMessage(): string {
        return $("#uploadMessage").val() as string;
    }
}


//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD
//DELETE THIS SECTION FOR NO MUSIC LOAD

declare var YT; // type definition for YouTube player class
var player: YouTubeEasterEggPlayer;

function onYouTubeIframeAPIReady() {
    player = new YouTubeEasterEggPlayer();
    player.init();
}

class YouTubeEasterEgg {
    public init() {
        var tag = document.createElement('script');
        tag.src = "https://www.youtube.com/iframe_api";
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
    }
}

class YouTubeEasterEggPlayer {
    private player;
    private readyplayer;
    private done = false;

    public init() {
        this.player = new YT.Player('music', {
            height: '0',
            width: '0',
            videoId: 'xwuwl46kYko',
            events: {
                'onReady': this.onPlayerReady.bind(this),
                'onStateChange': this.onPlayerStateChange.bind(this)
            }
        });
    }

    private onPlayerReady(event) {
        this.readyplayer = event.target;
        //event.target.playVideo();
    }

    private onPlayerStateChange(event) {
        //if (event.data == YT.PlayerState.PLAYING && !done) {
        //    setTimeout(stopVideo, 6000);
        //    done = true;
        //}
    }

    public stopVideo() {
        this.player.stopVideo();
    }

    public playVideo() {
        this.readyplayer.playVideo();
    }
}

//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
//END MUSIC SECTION
