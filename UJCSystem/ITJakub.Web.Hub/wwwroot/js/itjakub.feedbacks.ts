enum FeedbackCategoryEnum {
    None = 0,
    Dictionaries = 1,
    Editions = 2,
    BohemianTextBank = 3,
    OldGrammar = 4,
    ProfessionalLiterature = 5,
    Bibliographies = 6,
    CardFiles = 7,
    AudioBooks = 8,
    Tools = 9,
}

var categoryTranslation = [
    "None",
    "Dictionaries",
    "Editions",
    "BohemianTextBank",
    "OldGrammar",
    "ProfessionalLiterature",
    "Bibliographies",
    "CardFiles",
    "AudioBooks",
    "Tools"
];

enum FeedbackSortEnum {
    Date = 0,
    Category = 1,
}

var sortEnumTranslation = [
    "Author",
    "Email",
    "Category",
    "Date"
];

enum FeedbackTypeEnum {
    Generic = 0,
    Headword = 1,
}

var sortCriteria = FeedbackSortEnum.Date;
var sortOrderAsc = false;
var categories = new Array<number>();
var paginator: Pagination;
var feedbacksOnPage = 5;

$(document).ready(() => {

    var notFilledMessage = localization.translateFormat("NotFilled", new Array<string>("&lt;", "&gt;"), "ItJakubJs").value;


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
            data: JSON.stringify({ feedbackId: feedbackId }),
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
            success: (results: IFeedback[]) => {
                var feedbacksContainer = document.getElementById("feedbacks");
                $(feedbacksContainer).empty();

                for (var i = 0; i < results.length; i++) {
                    var actualFeedback = results[i];

                    var feedbackDiv = document.createElement("div");
                    $(feedbackDiv).addClass("feedback");
                    feedbackDiv.id = actualFeedback.id.toString();

                    var feedbackHeaderDiv = document.createElement("div");
                    $(feedbackHeaderDiv).addClass("feedback-header");


                    var feedbackHeaderInfosDiv = document.createElement("div");
                    $(feedbackHeaderInfosDiv).addClass("feedback-header-info-div");
                    

                    var name = "";
                    var email = "";
                    var signed = "";
                    var category = actualFeedback.category;
                    var date = new Date(actualFeedback.createDate);

                    var user = actualFeedback.user;
                    if (typeof user !== "undefined" && user !== null) {
                        name = user.firstName + " " + user.lastName;
                        email = user.email;
                        signed = localization.translate("Yes", "ItJakubJs").value;
                    } else {
                        name = actualFeedback.filledName;
                        email = actualFeedback.filledEmail;
                        signed = localization.translate("No", "ItJakubJs").value;
                    }

                    if (typeof name === "undefined" || name === null || name === "") {
                        name = notFilledMessage;
                    }

                    if (typeof email === "undefined" || email === null || email === "") {
                        email = notFilledMessage;
                    }

                    var feedbackNameSpan = document.createElement("span");
                    $(feedbackNameSpan).addClass("feedback-name");
                    feedbackNameSpan.innerHTML =
                        localization.translateFormat("Name:", new Array<string>(name), "ItJakubJs").value;

                    feedbackHeaderInfosDiv.appendChild(feedbackNameSpan);

                    var feedbackEmailSpan = document.createElement("span");
                    $(feedbackEmailSpan).addClass("feedback-email");
                    feedbackEmailSpan.innerHTML = 
                        localization.translateFormat("Email:", new Array<string>(email), "ItJakubJs").value;

                    feedbackHeaderInfosDiv.appendChild(feedbackEmailSpan);

                    var feedbackSignedUserSpan = document.createElement("span");
                    $(feedbackSignedUserSpan).addClass("feedback-signed");
                    feedbackSignedUserSpan.innerHTML =
                        localization.translateFormat("LoggedUser:", new Array<string>(signed), "ItJakubJs").value;

                    feedbackHeaderInfosDiv.appendChild(feedbackSignedUserSpan);

                    var feedbackCategorySpan = document.createElement("span");
                    $(feedbackCategorySpan).addClass("feedback-category");
                    feedbackCategorySpan.innerHTML = 
                        localization.translateFormat("Category:", new Array<string>(categoryTranslation[category]), "ItJakubJs").value;

                    feedbackHeaderInfosDiv.appendChild(feedbackCategorySpan);

                    var feedbackDateSpan = document.createElement("span");
                    $(feedbackDateSpan).addClass("feedback-date");
                    feedbackDateSpan.innerHTML = 
                        localization.translateFormat("Date:", new Array<string>(date.toLocaleDateString()), "ItJakubJs").value;


                    feedbackHeaderInfosDiv.appendChild(feedbackDateSpan);

                    var feedbackTimeSpan = document.createElement("span");
                    $(feedbackTimeSpan).addClass("feedback-time");
                    feedbackTimeSpan.innerHTML = 
                        localization.translateFormat("Time:", new Array<string>(date.toLocaleTimeString()), "ItJakubJs").value;

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

                    var feedbackBodyDiv = document.createElement("div");
                    $(feedbackBodyDiv).addClass("feedback-text");
                    
                    var feedbackTextDiv = document.createElement("div");
                    $(feedbackTextDiv).text(actualFeedback.text);
                    feedbackBodyDiv.appendChild(feedbackTextDiv);

                    if (actualFeedback.feedbackType === FeedbackTypeEnum.Headword) {
                        var separator = document.createElement("hr");

                        var feedbackHeadwordDiv = document.createElement("div");
                        $(feedbackHeadwordDiv).text(localization.translateFormat("DictionaryHeadword:", new Array<string>(actualFeedback.headwordInfo.defaultHeadword), "ItJakubJs").value);

                        var feedbackDictionaryDiv = document.createElement("div");
                        $(feedbackHeadwordDiv).text(localization.translateFormat("Dictionary:", new Array<string>(actualFeedback.headwordInfo.dictionaryName), "ItJakubJs").value);


                        $(feedbackBodyDiv)
                            .append(separator)
                            .append(feedbackHeadwordDiv)
                            .append(feedbackDictionaryDiv);
                    }
                    
                    feedbackDiv.appendChild(feedbackHeaderDiv);
                    feedbackDiv.appendChild(feedbackBodyDiv);


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