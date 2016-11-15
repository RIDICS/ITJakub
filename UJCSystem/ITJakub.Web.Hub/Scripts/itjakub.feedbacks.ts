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

enum FeedbackSortEnum {
    Date = 0,
    Category = 1,
}

var sortEnumTranslation = [
    "Autor",
    "E-mail",
    "Kategorie",
    "Datum"
];

var sortCriteria = FeedbackSortEnum.Date;
var sortOrderAsc = false;
var categories = new Array<number>();
var paginator: Pagination;
var feedbacksOnPage = 5;

$(document).ready(() => {

    var notFilledMessage = "&lt;Nezadáno&gt;";


    paginator = new Pagination({
        container: $("#feedbacks-paginator"),
        pageClickCallback: paginatorClickedCallback,
        callPageClickCallbackOnInit: true
    });


    function deleteFeedback(feedbackId: string) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Feedback/DeleteFeedback",
            data: { feedbackId: feedbackId },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                $(document.getElementById(feedbackId)).remove();
            }
        });
    }

    function showFeedbacks(start: number, count: number) {

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Feedback/GetFeedbacks",
            data: { categories: categories, start: start, count: count, sortCriteria: sortCriteria, sortAsc: sortOrderAsc },
            dataType: "json",
            contentType: "application/json",
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


                    var feedbackHeaderInfosDiv = document.createElement("div");
                    $(feedbackHeaderInfosDiv).addClass("feedback-header-info-div");
                    

                    var name = "";
                    var email = "";
                    var signed = "";
                    var category = actualFeedback["Category"];
                    var date = convertDate(actualFeedback["CreateDate"]);

                    var user = actualFeedback["User"];
                    if (typeof user !== "undefined" && user !== null) {
                        name = user["FirstName"] + " " + user["LastName"];
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
                    feedbackNameSpan.innerHTML = `Jméno: ${name}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackNameSpan);

                    var feedbackEmailSpan = document.createElement("span");
                    $(feedbackEmailSpan).addClass("feedback-email");
                    feedbackEmailSpan.innerHTML = `E-mail: ${email}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackEmailSpan);

                    var feedbackSignedUserSpan = document.createElement("span");
                    $(feedbackSignedUserSpan).addClass("feedback-signed");
                    feedbackSignedUserSpan.innerHTML = `Přihlášený uživatel: ${signed}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackSignedUserSpan);

                    var feedbackCategorySpan = document.createElement("span");
                    $(feedbackCategorySpan).addClass("feedback-category");
                    feedbackCategorySpan.innerHTML = `Kategorie: ${categoryTranslation[parseInt(category)]}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackCategorySpan);

                    var feedbackDateSpan = document.createElement("span");
                    $(feedbackDateSpan).addClass("feedback-date");
                    feedbackDateSpan.innerHTML = `Datum: ${date.toLocaleDateString()}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackDateSpan);

                    var feedbackTimeSpan = document.createElement("span");
                    $(feedbackTimeSpan).addClass("feedback-time");
                    feedbackTimeSpan.innerHTML = `Čas: ${date.toLocaleTimeString()}`;

                    feedbackHeaderInfosDiv.appendChild(feedbackTimeSpan);

                    var feedbackDeleteButtonDiv = document.createElement("div");
                    $(feedbackDeleteButtonDiv).addClass("feedback-delete-button-div");

                    var feedbackDeleteButton = document.createElement("button");
                    $(feedbackDeleteButton).addClass("feedback-delete-button");

                    var removeGlyph = document.createElement("span");
                    $(removeGlyph).addClass("glyphicon glyphicon-trash");
                    feedbackDeleteButton.appendChild(removeGlyph);

                    $(feedbackDeleteButton).click((event: Event) => {
                        var elementId = $(event.target).parents(".feedback")[0].id;
                        deleteFeedback(elementId);
                    });

                    feedbackDeleteButtonDiv.appendChild(feedbackDeleteButton);

                    feedbackHeaderDiv.appendChild(feedbackDeleteButtonDiv);
                    feedbackHeaderDiv.appendChild(feedbackHeaderInfosDiv);

                    var feedbackTextDiv = document.createElement("div");
                    $(feedbackTextDiv).addClass("feedback-text");
                    $(feedbackTextDiv).html(actualFeedback.Text);


                    feedbackDiv.appendChild(feedbackHeaderDiv);
                    feedbackDiv.appendChild(feedbackTextDiv);


                    $(feedbacksContainer).append(feedbackDiv);
                }
            }

        });
    }

    function paginatorClickedCallback(pageNumber: number) {
        var start = (pageNumber - 1) * feedbacksOnPage;
        showFeedbacks(start, feedbacksOnPage);
    }

    function getFeedbacksCount() {

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Feedback/GetFeedbacksCount",
            data: { categories: categories },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response;
                document.getElementById("feedbacks-count").innerHTML = count;
                paginator.make(count, feedbacksOnPage);
            }
        });
    }

    getFeedbacksCount();

});