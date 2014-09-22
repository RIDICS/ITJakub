$(document).ready(function () {
    $("#dropzoneFileForm").dropzone({
        url: '/Upload/UploadFile',
        maxFilesize: 10000, // MB
        maxFiles: 1,
        success: function (file, response) {
            alert("file processed "+response.FileInfo.Name); //TODO dynamically add page elements and fill them here
        }
});

    $("#dropzoneFrontImageForm").dropzone({
        url: '/Upload/UploadFrontImage',
        maxFilesize: 100, // MB
        maxFiles: 1,
        success: function (file, response) {
            alert("front image uploaded");
        }
    });

    $("#dropzoneImagesForm").dropzone({
        url: '/Upload/UploadImages',
        maxFilesize: 100, // MB
        success: function (file, response) {
            alert("image uploaded");
        }
    });

});