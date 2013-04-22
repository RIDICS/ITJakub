$(document).ready(function () {
    $("#ViewMode").change(function () {
        window.location = "/Sources/Listing/" + $(this).val() + "/A";
    });
});