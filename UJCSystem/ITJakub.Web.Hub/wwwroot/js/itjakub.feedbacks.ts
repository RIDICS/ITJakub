﻿enum FeedbackCategoryEnum {
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

enum FeedbackSortEnum {
    Date = 0,
    Category = 1,
}

enum FeedbackTypeEnum {
    Generic = "Generic",
    Headword = "Headword",
}

$(document.documentElement).ready(() => {

    var categoryTranslation = new DictionaryWrapper<string>();
    categoryTranslation.add(FeedbackCategoryEnum.None, "None");
    categoryTranslation.add(FeedbackCategoryEnum.Dictionaries, "Dictionaries");
    categoryTranslation.add(FeedbackCategoryEnum.Editions, "Editions");
    categoryTranslation.add(FeedbackCategoryEnum.BohemianTextBank, "BohemianTextBank");
    categoryTranslation.add(FeedbackCategoryEnum.OldGrammar, "OldGrammar");
    categoryTranslation.add(FeedbackCategoryEnum.ProfessionalLiterature, "ProfessionalLiterature");
    categoryTranslation.add(FeedbackCategoryEnum.Bibliographies, "Bibliographies");
    categoryTranslation.add(FeedbackCategoryEnum.CardFiles, "CardFiles");
    categoryTranslation.add(FeedbackCategoryEnum.AudioBooks, "AudioBooks");
    categoryTranslation.add(FeedbackCategoryEnum.Tools, "Tools");

    var sortCriteria = FeedbackSortEnum.Date;
    var sortOrderAsc = false;
    var categories = new Array<number>();
    var paginator: Pagination;
    var feedbacksOnPage = Number($("#feedbacks").data("page-size"));
    var notFilledMessage = localization.translateFormat("NotFilled", new Array<string>("&lt;", "&gt;"), "ItJakubJs").value;
    var loader = lv.create(null, "lv-circles sm lv-mid lvt-3 lvb-3");
    var removeLoader = lv.create(null, "lv-circles tiniest feedback-loader");

    paginator = new Pagination({
        container: document.getElementById("feedbacks-paginator") as HTMLDivElement,
        pageClickCallback: paginatorClickedCallback,
        callPageClickCallbackOnInit: true
    });

    function deleteFeedback(feedbackId: string) {
        $("#" + feedbackId).children(".feedback-header").children(".feedback-delete-button-div").prepend(removeLoader.getElement());
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

                    var user = actualFeedback.authorUser;
                    if (typeof user !== "undefined" && user !== null) {
                        name = user.firstName + " " + user.lastName;
                        email = user.email;
                        signed = localization.translate("Yes", "ItJakubJs").value;
                    } else {
                        name = actualFeedback.authorName;
                        email = actualFeedback.authorEmail;
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
                    var categoryParams = [categoryTranslation.get(category)];
                    var translation = localization.translateFormat("Category:", categoryParams, "ItJakubJs").value;
                    feedbackCategorySpan.innerHTML = translation;
                        

                    feedbackHeaderInfosDiv.appendChild(feedbackCategorySpan);

                    const splittedCreateTime = actualFeedback.createTimeString.split(" ");
                    const localizedDate = splittedCreateTime[0];
                    let localizedTime = splittedCreateTime[1];
                    for (let j = 2; j < splittedCreateTime.length; j++) {
                        localizedTime += " " + splittedCreateTime[j];
                    }
                    
                    var feedbackDateSpan = document.createElement("span");
                    $(feedbackDateSpan).addClass("feedback-date");
                    feedbackDateSpan.innerHTML = 
                        localization.translateFormat("Date:", new Array<string>(localizedDate), "ItJakubJs").value;


                    feedbackHeaderInfosDiv.appendChild(feedbackDateSpan);
                    
                    var feedbackTimeSpan = document.createElement("span");
                    $(feedbackTimeSpan).addClass("feedback-time");
                    feedbackTimeSpan.innerHTML = 
                        localization.translateFormat("Time:", new Array<string>(localizedTime), "ItJakubJs").value;

                    feedbackHeaderInfosDiv.appendChild(feedbackTimeSpan);

                    var feedbackDeleteButtonDiv = document.createElement("div");
                    $(feedbackDeleteButtonDiv).addClass("feedback-delete-button-div");

                    var feedbackDeleteButton = document.createElement("button");
                    $(feedbackDeleteButton).addClass("feedback-delete-button");

                    var removeGlyph = document.createElement("span");
                    $(removeGlyph).addClass("glyphicon glyphicon-trash");
                    feedbackDeleteButton.appendChild(removeGlyph);

                    $(feedbackDeleteButton).click((event) => {
                        var elementId = $(event.target as Node as HTMLElement).parents(".feedback").attr("id");
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
                        $(feedbackHeadwordDiv).text(localization.translateFormat("Dictionary:", new Array<string>(actualFeedback.projectInfo.name), "ItJakubJs").value);


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
        $("#feedbacks").empty();
        $("#feedbacks").append(loader.getElement());
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