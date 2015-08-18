//enum FeedbackCategory {
//    None = 0,
//    Dictionaries = 1,
//    Editions = 2,
//    BohemianTextBank = 3,
//    OldGrammar = 4,
//    ProfessionalLiterature = 5,
//    Bibliographies = 6,
//    CardFiles = 7,
//    AudioBooks = 8,
//    Tools = 9,
//}

var categoryTranslation = [
    "Žádná",
    "Slovníky",
    "Edice",
    "Korpusy",
    "Mluvnice",
    "Odborná literatura",
    "Bibliografie",
    "Kartotéky",
    "Audioknihy",
    "Pomůcky"
];

$(document).ready(() => {

    var notFilledMessage = "<Nezadáno>";


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

                var feedbackHeaderDiv = document.createElement("div");
                $(feedbackHeaderDiv).addClass("feedback-header");

                var name = "";
                var email = "";
                var signed = "";
                var category = actualFeedback["Category"];

                var user = actualFeedback["User"];
                if (typeof user !== "undefined" && user !== null) {
                    name = user["FirstName"]+" "+user["LastName"];
                    email = user["Email"];
                    signed = "ano";
                } else {
                    name = actualFeedback["FilledName"];
                    email = actualFeedback["FilledEmail"];
                    signed = "ne";
                }

                if (typeof name === "undefined" || name === null || name === "") {
                    name = notFilledMessage;
                }

                if (typeof email === "undefined" || email === null || email === "") {
                    email = notFilledMessage;
                }

                var feedbackNameSpan = document.createElement("span");
                $(feedbackNameSpan).addClass("feedback-name");
                feedbackNameSpan.innerHTML = "Jméno: " + name;

                feedbackHeaderDiv.appendChild(feedbackNameSpan);

                var feedbackEmailSpan = document.createElement("span");
                $(feedbackEmailSpan).addClass("feedback-email");
                feedbackEmailSpan.innerHTML = "E-mail: " + email;

                feedbackHeaderDiv.appendChild(feedbackEmailSpan);

                var feedbackSignedUserSpan = document.createElement("span");
                $(feedbackSignedUserSpan).addClass("feedback-signed");
                feedbackSignedUserSpan.innerHTML = "Přihlášený uživatel: " + signed;

                feedbackHeaderDiv.appendChild(feedbackSignedUserSpan);

                var feedbackCategorySpan = document.createElement("span");
                $(feedbackCategorySpan).addClass("feedback-category");
                feedbackCategorySpan.innerHTML = "Kategorie: " + categoryTranslation[parseInt(category)];

                feedbackHeaderDiv.appendChild(feedbackCategorySpan);


                var feedbackDeleteButton = document.createElement("button");
                feedbackDeleteButton.type = "button";
                feedbackDeleteButton.innerHTML = "Smazat";
                $(feedbackDeleteButton).addClass("feedback-delete-button btn btn-default styled-button ");

                $(feedbackDeleteButton).click((event: Event) => {
                    var elementId = $(event.target).parents(".feedback")[0].id;
                    deleteFeedback(elementId);
                });

                feedbackHeaderDiv.appendChild(feedbackDeleteButton);

                var feedbackTextDiv = document.createElement("div");
                $(feedbackTextDiv).addClass("feedback-text");
                $(feedbackTextDiv).html(actualFeedback.Text);


                feedbackDiv.appendChild(feedbackHeaderDiv);
                feedbackDiv.appendChild(feedbackTextDiv);


                $(feedbacksContainer).append(feedbackDiv);
            }
        }
    });

});
