
$(document).ready(function () {
    $("#ViewMode").change(function () {
        window.location.href = $(this).find("option:selected").attr("data-url");
    });

    $("#sources-search-form").submit(function () {
        window.location.href = $("#sources-search-form").attr("action") + "/" + $("#sources-search-form #searchTerm").val();
        return false;
    });
});
