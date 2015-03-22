$(".cardfile-select-more").click(function () {
    var body = $(this).parents(".cardfile-select").children(".cardfile-select-body");
    if (body.is(":hidden")) {
        $(this).children().removeClass("glyphicon-chevron-down");
        $(this).children().addClass("glyphicon-chevron-up");
        body.slideDown();
    } else {
        $(this).children().removeClass("glyphicon-chevron-up");
        $(this).children().addClass("glyphicon-chevron-down");
        body.slideUp();
    }
});

$(".cardfile-clear-filter").click(function () {
    $(this).siblings(".cardfile-filter-input").val('').change();
});

$(".cardfile-filter-input").keyup(function () {
    $(this).change();
});

$(".cardfile-filter-input").change(function () {
    if ($(this).val() == '') {
        $(this).parents(".cardfile-select-body").children(".concrete-cardfile").show();
    } else {
        $(this).parents(".cardfile-select-body").children(".concrete-cardfile").hide().filter(':contains(' + $(this).val() + ')').show();
    }
});

$(".delete-cardfile").click(function () {
    $(this).siblings(".save-cardfile").show();
    $(this).hide();
    //TODO populate request on delete from favorites
});

$(".save-cardfile").click(function () {
    $(this).siblings(".delete-cardfile").show();
    $(this).hide();
    //TODO populate request on save to favorites
});

$(".concrete-cardfile-checkbox").click(function () {
    //TODO add cardfile to search criteria
});

//slider
$(document).ready(function () {
    var slider = $(".slider");
    $(slider).slider({
        min: 0,
        max: 100,
        value: 0,
        start: function (event, ui) {
            $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
        },
        stop: function (event, ui) {
            $(event.target).find('.ui-slider-handle').find('.slider-tip').fadeOut(1000);
        },
        slide: function (event, ui) {
            $(event.target).find('.ui-slider-handle').find('.slider-tip').stop(true, true);
            $(event.target).find('.ui-slider-handle').find('.slider-tip').show();
            $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html("Lístek - 2047 <br/> Heslo - xxx");
        }
    });

    var sliderTooltip = document.createElement('div');
    $(sliderTooltip).addClass('tooltip top slider-tip');
    var arrowTooltip = document.createElement('div');
    $(arrowTooltip).addClass('tooltip-arrow');
    sliderTooltip.appendChild(arrowTooltip);

    var innerTooltip = document.createElement('div');
    $(innerTooltip).addClass('tooltip-inner');
    $(innerTooltip).html("Lístek - 2046 <br/> Heslo - netbalivý");
    sliderTooltip.appendChild(innerTooltip);
    $(sliderTooltip).hide();

    var sliderHandle = $(slider).find('.ui-slider-handle');
    $(sliderHandle).append(sliderTooltip);
    $(sliderHandle).hover(function (event) {
        $(event.target).find('.slider-tip').stop(true, true);
        $(event.target).find('.slider-tip').show();
    });
    $(sliderHandle).mouseout(function (event) {
        $(event.target).find('.slider-tip').fadeOut(1000);
    });
});
//# sourceMappingURL=itjakub.cardfiles.js.map
