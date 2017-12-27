enum FeedbackCategoryEnum {
    None = "None",
    Dictionaries = "Dictionaries",
    Editions = "Editions",
    BohemianTextBank = "BohemianTextBank",
    OldGrammar = "OldGrammar",
    ProfessionalLiterature = "ProfessionalLiterature",
    Bibliographies = "Bibliographies",
    CardFiles = "CardFiles",
    AudioBooks = "AudioBooks",
    Tools = "Tools",
}

var categoryTranslation = new DictionaryWrapper<string>();
categoryTranslation.add(FeedbackCategoryEnum.None, "Žádná");
categoryTranslation.add(FeedbackCategoryEnum.Dictionaries, "Slovníky");
categoryTranslation.add(FeedbackCategoryEnum.Editions, "Edice");
categoryTranslation.add(FeedbackCategoryEnum.BohemianTextBank, "Korpusy");
categoryTranslation.add(FeedbackCategoryEnum.OldGrammar, "Mluvnice");
categoryTranslation.add(FeedbackCategoryEnum.ProfessionalLiterature, "Odborná literatura");
categoryTranslation.add(FeedbackCategoryEnum.Bibliographies, "Bibliografie");
categoryTranslation.add(FeedbackCategoryEnum.CardFiles, "Kartotéky");
categoryTranslation.add(FeedbackCategoryEnum.AudioBooks, "Audioknihy");
categoryTranslation.add(FeedbackCategoryEnum.Tools, "Pomůcky");

enum FeedbackSortEnum {
    Date = 0,
    Category = 1,
}

enum FeedbackTypeEnum {
    Generic = "Generic",
    Headword = "Headword",
}

var sortCriteria = FeedbackSortEnum.Date;
var sortOrderAsc = false;
var categories = new Array<number>();
var paginator: Pagination;
var feedbacksOnPage = 5;

$(document.documentElement).ready(() => {

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
            data: { categories: categories, start: start, count: count, sortCriteria: sortCriteria, sortAsc: sortOrderAsc } as JQuery.PlainObject,
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
                    var category = actualFeedback.feedbackCategory;
                    var date = new Date(actualFeedback.createTime);

                    var user = actualFeedback.authorUser;
                    if (typeof user !== "undefined" && user !== null) {
                        name = user.firstName + " " + user.lastName;
                        email = user.email;
                        signed = "ano";
                    } else {
                        name = actualFeedback.authorName;
                        email = actualFeedback.authorEmail;
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
                    feedbackCategorySpan.innerHTML = `Kategorie: ${categoryTranslation.get(category)}`;

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

                    $(feedbackDeleteButton).click((event: JQuery.Event) => {
                        var elementId = $(event.target as Element).parents(".feedback").attr("id");
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
                        $(feedbackHeadwordDiv).text("Slovníkové heslo: " + actualFeedback.headwordInfo.defaultHeadword);

                        var feedbackDictionaryDiv = document.createElement("div");
                        $(feedbackDictionaryDiv).text("Slovník: " + actualFeedback.projectInfo.name);

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
            data: { categories: categories } as JQuery.PlainObject,
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