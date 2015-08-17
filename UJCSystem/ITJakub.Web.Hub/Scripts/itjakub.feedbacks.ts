
$(document).ready(() => {

    function deleteFeedback(feedbackId: string)
    {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Feedback/DeleteFeedback",
            data: { feedbackId: feedbackId},
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                $(document.getElementById(feedbackId)).remove();
            }
        });
    }

    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Feedback/GetFeedbacksCount",
        data: {},
        dataType: 'json',
        contentType: 'application/json',
        success: response => {
            document.getElementById("feedbacks-count").innerHTML = response;
        }
    });


    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Feedback/GetFeedbacks",
        data: {},
        dataType: 'json',
        contentType: 'application/json',
        success: results => {
            var feedbacksContainer = document.getElementById("feedbacks");
            $(feedbacksContainer).empty();

            for (var i = 0; i < results.length; i++) {
                var actualFeedback = results[i];

                var feedbackDiv = document.createElement("div");
                $(feedbackDiv).addClass("feedback");
                feedbackDiv.id = actualFeedback["Id"];

                var feedbackTextDiv = document.createElement("div");
                $(feedbackTextDiv).addClass("feedback-text");
                $(feedbackTextDiv).html(actualFeedback.Text);


                var feedbackDeleteButton = document.createElement("button");
                feedbackDeleteButton.type = "button";
                feedbackDeleteButton.innerHTML = "Smazat";
                $(feedbackDeleteButton).addClass("feedback-delete-button");

                $(feedbackDeleteButton).click((event: Event) => {
                    var elementId = $(event.target).parents(".feedback")[0].id;
                    deleteFeedback(elementId);
                });


                feedbackDiv.appendChild(feedbackTextDiv);
                feedbackDiv.appendChild(feedbackDeleteButton);


                $(feedbacksContainer).append(feedbackDiv);
            }
        }
    });

});
