$(document).ready(() => {
    var maxChars = $(".feedback-text-area").attr("data-val-maxlength-max");
    var maxCharsNumber = Number(maxChars);
    $(".text-area-max-chars").html(maxChars);
    $(".text-area-remaining-chars").html(maxChars);

    $(".feedback-text-area").keyup(function () {
        var text = $(this).val();
        var actualCharsRemaining = maxCharsNumber - text.length;
        if (actualCharsRemaining < 0) {
            actualCharsRemaining = 0;
            $(this).val(text.substring(0, maxCharsNumber));
        }
        $(".text-area-remaining-chars").html(String(actualCharsRemaining));
    });

    if (!$("input[name='Headword']").val() && !$("input[name='Dictionary']").val()) {
        $(".optional").addClass("hidden");
    }
});