$(document).ready(function () {

    rootNode.init();
    
    //$('.advanced-search-wrapper').advancedSearch();
    $("#search-results-ordering > ul > li > a[data-toggle=tab]").initLoadingSearchResults();
    $("#search-results-alphabetical a[data-toggle=tab]").initLoadingAlphTermDetail();
    $("#results-type a[data-toggle=tab]").initLoadingTypeTermDetail();

    history.initHash();
    $(window).on('hashchange', function () {
            history.initHash();
    });
});

(function ($) {
    $.fn.moveTo = function (selector) {
        return this.each(function () {
            var cl = $(this).clone();
            $(cl).appendTo(selector);
            $(this).remove();
        });
    };
})(jQuery);

function isBlankString(str) {
    return (!str || /^\s*$/.test(str));
}

function ulfirst(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}

var SelectedSources = function () {
    var selectedCategories = new Array();
    var selectedBooks = new Array();

    this.book = "book";
    this.category = "category";

    this.addSource = function (type, id, name) {
        if (type == this.category) {
            selectedCategories.push({
                id: id,
                name: name
            });
        }
        if (type == this.book) {
            selectedBooks.push({
                id: id,
                name: name
            });
        }
    };

    this.removeSource = function (type, id, name) {
        if (type == this.category) {
            for (var i = 0; i < selectedCategories.length; i++) {
                if (id == selectedCategories[i].id) {
                    selectedCategories.splice(i, 1);
                }
            }
        }
        if (type == this.book) {
            for (var i = 0; i < selectedBooks.length; i++) {
                if (id == selectedBooks[i].id) {
                    selectedBooks.splice(i, 1);
                }
            }
        }
    }

    this.getSelectedHtml = function () {
        var categoriesNames = new Array();
        for (var i = 0; i < selectedCategories.length; i++) {
            categoriesNames.push(selectedCategories[i].name);
        }
        var booksNames = new Array();
        for (var i = 0; i < selectedBooks.length; i++) {
            booksNames.push(selectedBooks[i].name);
        }

        if (categoriesNames.length == 0 && booksNames.length == 0) {
            return "<span class=\"muted\">Aktivní prohledávání ve všech dostupných dílech</span>";
        }

        var categoriesString = categoriesNames.join(", ");
        var booksString = booksNames.join(", ");

        if (categoriesNames.length == 0) {
            categoriesString = "-";
        }

        if (booksNames.length == 0) {
            booksString = "-";
        }

        return "<div class=\"muted\">" +
            "<strong>Kategorie:</strong> " + categoriesString + "<br>" +
            "<strong>Díla:</strong> " + booksString;
    };

    this.getSelectedUrlParam = function () {
        var categoriesIds = new Array();
        for (var i = 0; i < selectedCategories.length; i++) { 
            categoriesIds.push(selectedCategories[i].id); 
        }
        var booksIds = new Array();
        for (var i = 0; i < selectedBooks.length; i++) { 
            booksIds.push(selectedBooks[i].id); 
        }

        var categoriesString = categoriesIds.join("+");
        var booksString = booksIds.join("+");

        return "kategorie=" + categoriesString + "&dila=" + booksString;
    };

    this.checkCheckboxes = function (selector) {
        var _this = this;
        selector.each(function () {
            _this.addSource($(this).attr("data-type"), $(this).attr("data-id"), $(this).attr("data-name"));
        });
        $(".advanced-search-wrapper .searched-books").html(this.getSelectedHtml());
    };

    this.uncheckCheckboxes = function (selector) {
        var _this = this;
        selector.each(function () {
            _this.removeSource($(this).attr("data-type"), $(this).attr("data-id"), $(this).attr("data-name"));
        });
        $(".advanced-search-wrapper .searched-books").html(this.getSelectedHtml());
    };

}

var selectedsources = new SelectedSources();

(function ($) {
    $.fn.extend({
        loadChildren: function (options) {

            var defaults = {
                categoryId: null,
                categoriesUrl: "/hledani/search-category-children"
            };

            var options = $.extend(defaults, options);

            function showLevel(iconElement) {
                iconElement.parent().find(" > ul.nav.show-categories").slideDown();
                iconElement.parent().find(" > div.category-select").slideDown();
                iconElement.attr("class", "icon-chevron-down");
                iconElement.unbind('click');
                iconElement.click(function () {
                    hideLevel(iconElement);
                });
            }

            function hideLevel(iconElement) {
                iconElement.parent().find(" > ul.nav.show-categories").slideUp();
                iconElement.parent().find(" > div.category-select").slideUp();
                iconElement.attr("class", "icon-chevron-right");
                iconElement.unbind('click');
                iconElement.click(function () {
                    showLevel(iconElement);
                });
            }

            return this.each(function () {
                var categoriesUrl = options.categoriesUrl;
                if (options.categoryId != null) {
                    categoriesUrl = categoriesUrl + "/" + options.categoryId;
                }
                
                var parentElement = $(this);

                if (parentElement.find(" > ul.nav").length > 0) {
                    showLevel(parentElement.find(" > i[class=icon-chevron-down]"));
                } else {
                    $.get(categoriesUrl, function (data) {
                        parentElement.append(data);
                        
                        /*todelete*/rootNode.reloadTree($("#search-categories-tree > ul"));

                        parentElement.find(".category-select input[type=text]").typeahead({
                            source: function () {
                                var children = new Array();
                                parentElement.find('> .nav input[type=checkbox]').each(function () {
                                    children.push($(this).attr('data-name'));
                                });
                                return children;
                            }
                        });
                        parentElement.find(".category-select form").submit(function () {
                            var selectedName = $(this).find('input[type=text]').val();
                            $(this).find('input[type=text]').val("");
                            var formEl = $(this);
                            $(this).parent().parent().find('input[type=checkbox]').each(function () {
                                if ($(this).attr("data-name") == selectedName) {
                                    $(this).prop("checked", true);
                                    selectedsources.checkCheckboxes($(this));
                                    formEl.parent().find('.selected-categories').append($(this).parent().parent());
                                }
                            });
                            return false;
                        });

                        parentElement.find("input[type=checkbox]").unbind("change");
                        parentElement.find("input[type=checkbox]").change(function () {

                            function uncheckParentIfAllChildrenUnchecked(chckbx) {
                                if (chckbx.parent().parent().parent().find("input[type=checkbox]").length > 0) {
                                    var allUnchecked = true;

                                    chckbx.parent().parent().parent().find("input[type=checkbox]").each(function () {
                                        if ($(this).is(":checked")) {
                                            allUnchecked = false;
                                        }
                                    });

                                    if (allUnchecked) {
                                        chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]").prop("checked", false);
                                        selectedsources.uncheckCheckboxes(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                        uncheckParentIfAllChildrenUnchecked(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                    }
                                }
                            }
                            function checkParentIfAllChildrenChecked(chckbx) {
                                if (chckbx.parent().parent().parent().parent().find("ul.nav.show-categories > li > label > input[type=checkbox]").length > 0) {
                                    var allChecked = true;

                                    chckbx.parent().parent().parent().parent().find("ul.nav.show-categories > li > label > input[type=checkbox]").each(function () {
                                        if (!$(this).is(":checked")) {
                                            allChecked = false;
                                        }
                                    });

                                    if (allChecked) {
                                        chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]").prop("checked", true);
                                        selectedsources.checkCheckboxes(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                        checkParentIfAllChildrenChecked(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                    }
                                }
                            }
                            function uncheckAllParent(chckbx) {
                                chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]").prop("checked", false);
                                selectedsources.uncheckCheckboxes(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                if (chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]").length > 0) {
                                    uncheckAllParent(chckbx.parent().parent().parent().parent().find("> label > input[type=checkbox]"));
                                }
                            }

                            if ($(this).is(":checked")) {
                                $(this).parent().parent().find("> label > input[type=checkbox]").each(function () {
                                        $(this).prop("checked", true);
                                        selectedsources.checkCheckboxes($(this));
                                });
                                $(this).parent().parent().find("ul.nav.show-categories > li > label > input[type=checkbox]").each(function () {
                                    if (!$(this).prop("checked")) {
                                        $(this).prop("checked", true);
                                        selectedsources.checkCheckboxes($(this));
                                    }
                                });
                                if (!$(this).parent().parent().parent().is(".selected-categories")) {
                                    checkParentIfAllChildrenChecked($(this));
                                }
                            } else {
                                selectedsources.uncheckCheckboxes($(this));
                                if (!$(this).parent().parent().parent().is(".selected-categories")) {
                                    //uncheckParentIfAllChildrenUnchecked($(this));
                                    //checkParentIfAllChildrenChecked($(this));
                                    uncheckAllParent($(this));
                                }
                                if ($(this).parent().parent().parent().is(".selected-categories")) {
                                    $(this).parent().parent().parent().parent().parent().find("> ul.nav").append($(this).parent().parent());
                                }
                            }
                        });

                        if (isBlankString(data)) {
                            parentElement.find(" > i[class=icon-chevron-right]").attr("class", "icon-chevron-down");
                            parentElement.find(" > i[class=icon-chevron-right]").unbind("click");
                        } else {
                            if (parentElement.find(" > i[class=icon-chevron-right]").length > 0) {
                                showLevel(parentElement.find(" > i[class=icon-chevron-right]"));
                            } else {
                                parentElement.find(" > ul.nav.show-categories").slideDown();
                                parentElement.find(" > div.category-select").slideDown();
                            }
                            parentElement.find(" > ul.nav i[class=icon-chevron-right]").click(function () {
                                $(this).parent().loadChildren({ categoriesUrl: options.categoriesUrl, categoryId: $(this).parent().attr("data-category-id") });
                            });
                        }
                    });
                }
            });
        }
    });
})(jQuery);


(function ($) {
    $.fn.extend({
        advancedSearch: function (options) {

            var defaults = {};

            var options = $.extend(defaults, options);

            var advancedSearchVisible = true;

            var categoriesUrl = null;

            function changeASVisibility(asElement) {
                if (advancedSearchVisible) {
                    asElement.find('.advanced-search').slideUp();
                    advancedSearchVisible = false;
                } else {
                    asElement.find('.advanced-search').slideDown();
                    advancedSearchVisible = true;
                }
            }

            return this.each(function () {
                var asElement = $(this);

                categoriesUrl = asElement.find(".categories").attr("data-categories-url");
                    
                asElement.find('.advanced-search').hide();
                asElement.find(".categories").loadChildren({ categoriesUrl: categoriesUrl });

                advancedSearchVisible = false;
                $('.show-advanced-search').click(function () {
                    changeASVisibility(asElement);
                });

                $(".advanced-search-wrapper form.search-form").submit(function () {
                    window.location.href = $(this).attr("action") + "?searchTerm=" + $(this).find("#search-term").val() + "&" + selectedsources.getSelectedUrlParam();
                    return false;
                });
            });
        }
    });
})(jQuery);

/************************************************************************
 *  Advance search category tree
 ***********************************************************************/
var TreeNode = function () {
    this.ìd = null;
    this.name = null;
    this.type = null;
    this.parent = null;
    this.children = new Array;
    this.treeSelector = null;
    this.selected = false;
    
    var advancedSearchVisible = false;

    this.init = function () {
        var currentNode = this;
        $(".advanced-search").hide();

        if ($("#created-searched-tree").length > 0) {
            $("#search-categories-tree").html("");
            $("#search-categories-tree").html($("#created-searched-tree").html());
            $("#created-searched-tree").remove();
            currentNode.loadedChildTree($('#search-categories-tree'));
        } else {
            currentNode.loadChildTreeHtml($('#search-categories-tree'));
        }
        
        $('.show-advanced-search').click(function () {
            changeASVisibility($(this).parent().parent());
            $(this).blur();
        });
        
        $(".advanced-search-wrapper form.search-form").submit(function () {
            window.location.href = $(this).attr("action") + "?searchTerm=" + $(this).find("#search-term").val() + "&" + getUrlCatBookParams();
            return false;
        });
    };

    function changeASVisibility(asElement) {
        if (advancedSearchVisible) {
            asElement.find('.advanced-search').slideUp();
            advancedSearchVisible = false;
        } else {
            asElement.find('.advanced-search').slideDown();
            advancedSearchVisible = true;
        }
    }

    function showLevel(iconElement) {
        iconElement.parent().find(" > ul.nav.show-categories").slideDown();
        iconElement.parent().find(" > div.category-select").slideDown();
        iconElement.attr("class", "icon-chevron-down");
        iconElement.unbind('click');
        iconElement.click(function () {
            hideLevel(iconElement);
        });
    }

    function hideLevel(iconElement) {
        iconElement.parent().find(" > ul.nav.show-categories").slideUp();
        iconElement.parent().find(" > div.category-select").slideUp();
        iconElement.attr("class", "icon-chevron-right");
        iconElement.unbind('click');
        iconElement.click(function () {
            showLevel(iconElement);
        });
    }

    this.initTree = function (ulSelector) {
        this.treeSelector = ulSelector;
        var childrenSelectors = ulSelector.find("> li > label > input[type=checkbox]");
        
        for (var i = 0; i < childrenSelectors.length; i++) {
            var childrenNode = new TreeNode;
            var childrenSelector = $(childrenSelectors[i]);
            
            childrenNode.id = childrenSelector.attr("data-id");
            childrenNode.name = childrenSelector.attr("data-name");
            childrenNode.type = childrenSelector.attr("data-type");
            childrenNode.parent = this;
            childrenNode.treeSelector = childrenSelector;
            this.children.push(childrenNode);
            allNodes[childrenNode.id] = childrenNode;
            childrenNode.initTree(childrenSelector.parent().parent().find("> ul.nav"));
        }
    };

    this.loadedChildTree = function (inTo, noInitTree) {
        var currentNode = this;
        
        inTo.find("> ul.nav.show-categories").slideDown();
        inTo.find("> div.category-select").slideDown();

        if (!noInitTree) {
            if (inTo.attr("data-category-id")) {
                allNodes[inTo.attr("data-category-id")].initTree(inTo.find("> ul.nav"));
            } else {
                currentNode.initTree(inTo.find("> ul.nav"));
            }
        }

        inTo.find("> ul.nav > li > i[class=icon-chevron-right]").click(function () {
            showLevel($(this));
            if ($(this).parent().find("> ul.nav").length > 0) {
                currentNode.loadedChildTree($(this).parent(), true);
            } else {
                currentNode.loadChildTreeHtml($(this).parent(), $(this).parent().attr("data-category-id"));
            }
        });

        var parentCheckbox = inTo.find("> label > input");
        if (parentCheckbox.is(":checked")) {
            inTo.find("> ul.nav > li > label > input[type=checkbox]").each(function () {
                select($(this).attr("data-id"));
            });
            updateCheckboxesView();
            changeState();
        }

        inTo.find("> ul.nav > li > label > input[type=checkbox]").change(function () {
            if ($(this).is(":checked")) {
                currentNode.checkInput(this);
            } else {
                currentNode.uncheckInput(this);
            }
        });
        
        inTo.find("> .category-select > form > input[type=text]").typeahead({
            source: function () {
                var children = new Array();
                inTo.find('> .nav > li > label > input[type=checkbox]').each(function () {
                    children.push($(this).attr('data-name'));
                });
                return children;
            }
        });
        inTo.find(".category-select > form").submit(function () {
            var selectedName = $(this).find('> input[type=text]').val();
            $(this).find('> input[type=text]').val("");
            var formEl = $(this);
            $(this).parent().parent().find('input[type=checkbox]').each(function () {
                if ($(this).attr("data-name") == selectedName) {
                    $(this).prop("checked", true);
                    $(this).change();
                    $(this).parent().parent().hide();
                    formEl.parent().find('.selected-categories').append($(this).parent().parent());
                    $(this).parent().parent().slideDown();
                }
            });
            return false;
        });

        $.each(loadedCheckedCategories, function (index, value) {
            var checkedCategorySelector = inTo.find("input[data-id='" + value + "']");
            if (checkedCategorySelector.length > 0) {
                checkedCategorySelector.prop("checked", true);
                allNodes[checkedCategorySelector.attr("data-id")].selected = true;
                
                if (checkedCategorySelector.parent().parent().parent().parent().find("> div.category-select").length > 0) {
                    var movedCheckbox = checkedCategorySelector.parent().parent();
                    movedCheckbox.parent().parent().find("> div.category-select > ul.nav").append(movedCheckbox);
                    movedCheckbox.slideDown();
                    checkedCategorySelector.change(function () {
                        if ($(this).is(":checked")) {
                            allNodes[checkedCategorySelector.attr("data-id")].checkInput(this);
                        } else {
                            allNodes[checkedCategorySelector.attr("data-id")].uncheckInput(this);
                        }
                    });
                }
            }
        });

        $.each(loadedCheckedBook, function (index, value) {
            var checkedBookSelector = inTo.find("input[data-id='" + value + "']");
            if (checkedBookSelector.length > 0) {
                checkedBookSelector.prop("checked", true);
                allNodes[checkedBookSelector.attr("data-id")].selected = true;

                if (checkedBookSelector.parent().parent().parent().parent().find("> div.category-select").length > 0) {
                    var movedCheckbox = checkedBookSelector.parent().parent();
                    movedCheckbox.parent().parent().find("> div.category-select > ul.nav").append(movedCheckbox);
                    movedCheckbox.slideDown();
                    checkedBookSelector.change(function () {
                        if ($(this).is(":checked")) {
                            allNodes[checkedBookSelector.attr("data-id")].checkInput(this);
                        } else {
                            allNodes[checkedBookSelector.attr("data-id")].uncheckInput(this);
                        }
                    });
                }
            }
        });
        
        createCheckedCategoriesArray();
        createCheckedBooksArray();
        $(".searched-books").html(getSelectedSourcesHtml());
    };

    this.loadChildTreeHtml = function (inTo, categoryId) {
        if (!categoryId) {
            categoryId = "";
        }
        var url = $('#search-categories-tree').attr("data-categories-url") + "/" + categoryId;
        var currentNode = this;
        $.get(url, function (data) {
            inTo.append(data);

            inTo.find("> ul.nav.show-categories").slideDown();
            inTo.find("div.category-select").slideDown();

            if (inTo.attr("data-category-id")) {
                allNodes[inTo.attr("data-category-id")].initTree(inTo.find("> ul.nav"));
            } else {
                currentNode.initTree(inTo.find("> ul.nav"));
            }
            inTo.find("> ul.nav i[class=icon-chevron-right]").click(function () {
                showLevel($(this));
                currentNode.loadChildTreeHtml($(this).parent(), $(this).parent().attr("data-category-id"));
            });

            var parentCheckbox = inTo.find("> label > input");
            if (parentCheckbox.is(":checked")) {
                inTo.find("> ul.nav > li > label > input[type=checkbox]").each(function () {
                    select($(this).attr("data-id"));
                });
                updateCheckboxesView();
                changeState();
            }

            inTo.find("> ul.nav > li > label > input[type=checkbox]").change(function () {
                if ($(this).is(":checked")) {
                    currentNode.checkInput(this);
                } else {
                    currentNode.uncheckInput(this);
                }
            });
            inTo.find(".category-select input[type=text]").typeahead({
                source: function () {
                    var children = new Array();
                    inTo.find('> .nav input[type=checkbox]').each(function () {
                        children.push($(this).attr('data-name'));
                    });
                    return children;
                }
            });
            inTo.find(".category-select form").submit(function () {
                var selectedName = $(this).find('input[type=text]').val();
                $(this).find('input[type=text]').val("");
                var formEl = $(this);
                $(this).parent().parent().find('input[type=checkbox]').each(function () {
                    if ($(this).attr("data-name") == selectedName) {
                        $(this).prop("checked", true);
                        $(this).change();
                        $(this).parent().parent().hide();
                        formEl.parent().find('.selected-categories').append($(this).parent().parent());
                        $(this).parent().parent().slideDown();
                    }
                });
                return false;
            });
        });
    };
    
    function changeState () {
        createCheckedCategoriesArray();
        createCheckedBooksArray();
        $(".advanced-search-wrapper .searched-books").html(getSelectedSourcesHtml());
    }

    this.checkInput = function (checkedInput) {
        var checkedInputSelector = $(checkedInput);
        allNodes[checkedInputSelector.attr("data-id")].selected = true;
        this.apllyCheckRules(checkedInputSelector);

        changeState();
    };

    this.uncheckInput = function (checkedInput) {
        var checkedInputSelector = $(checkedInput);
        allNodes[checkedInputSelector.attr("data-id")].selected = false;
        this.apllyUncheckRules(checkedInputSelector);

        changeState();

        var mbsel = checkedInputSelector.parent().parent().parent().parent();
        if (mbsel.is("div.category-select")) {
            var movedCheckbox = checkedInputSelector.parent().parent();
            movedCheckbox.slideUp('fast', function () {
                mbsel.parent().find("> ul").append($(this));
            });
        }
    };

    this.apllyCheckRules = function (checkedInputSelector) {
        checkAllChildren(checkedInputSelector.attr("data-id"));
        checkParentIfChildrenChecked(checkedInputSelector.attr("data-id"));
        updateCheckboxesView();
    };

    this.apllyUncheckRules = function (checkedInputSelector) {
        unCheckAllChildren(checkedInputSelector.attr("data-id"));
        uncheckAllParents(checkedInputSelector.attr("data-id"));
        updateCheckboxesView();
    };

    function select(checkboxId) {
        allNodes[checkboxId].selected = true;
        checkedCheckboxes.push(checkboxId);
    };

    function deselect(checkboxId) {
        allNodes[checkboxId].selected = false;
        uncheckedCheckboxes.push(checkboxId);
    };

    function checkAllChildren(checkedInputId) {
        for (var i = 0; i < allNodes[checkedInputId].children.length; i++) {
            var child = allNodes[checkedInputId].children[i];
            select(child.id);
            checkAllChildren(child.id);
        }
    };

    function unCheckAllChildren(checkedInputId) {
        for (var i = 0; i < allNodes[checkedInputId].children.length; i++) {
            var child = allNodes[checkedInputId].children[i];
            deselect(child.id);
            unCheckAllChildren(child.id);
        }
    };

    function checkParentIfChildrenChecked(checkedInputId) {
            if (allChildrenChecked(checkedInputId)) {
                if (allNodes[checkedInputId].parent.id) {
                    select(allNodes[checkedInputId].parent.id);
                    checkParentIfChildrenChecked(allNodes[checkedInputId].parent.id);
                }
            }
    };

    function uncheckAllParents(checkedInputId) {
        if (allNodes[checkedInputId] && allNodes[checkedInputId].parent.id != null) {
            if (allNodes[checkedInputId].parent.id) {
                deselect(allNodes[checkedInputId].parent.id);
                uncheckAllParents(allNodes[checkedInputId].parent.id);
            }
        }
    };

    function updateCheckboxesView () {
        for (var i = 0; i < checkedCheckboxes.length; i++) {
            var checkboxSelector = $("input[data-id=" + checkedCheckboxes[i] + "]");
            checkboxSelector.prop("checked", true);
            //selectedsources.checkCheckboxes(checkboxSelector);
        }
        checkedCheckboxes = new Array;
        for (var i = 0; i < uncheckedCheckboxes.length; i++) {
            var checkboxSelector = $("input[data-id=" + uncheckedCheckboxes[i] + "]");
            checkboxSelector.prop("checked", false);
            //selectedsources.uncheckCheckboxes(checkboxSelector);
            var mbsel = checkboxSelector.parent().parent().parent().parent();
            if (mbsel.is("div.category-select")) {
                var movedCheckbox = checkboxSelector.parent().parent();
                movedCheckbox.slideUp('fast', function () {
                    mbsel.parent().find("> ul").append($(this));
                });
            }
        }
        uncheckedCheckboxes = new Array;
    };

    function allChildrenChecked(checkedInputId) {
        var checked = true;
        for (var i = 0; i < allNodes[checkedInputId].parent.children.length; i++) {
            var child = allNodes[checkedInputId].parent.children[i];
            if (!child.selected) {
                checked = false;
            }
        }
        return checked;
    };

    function allChildrenUnChecked(checkedInputId) {
        var has = true;
        for (var i = 0; i < allNodes[checkedInputId].children.length; i++) {
            var child = allNodes[checkedInputId].children[i];
            if (child.selected && child.type == "category") {
                has = false;
            }
        }
        return has;
    };

    function createCheckedCategoriesArray() {
        checkedCategoriesIds = new Array();
        $.each(allNodes, function (index, value) {
            if (value.selected && value.type == "category" && allChildrenUnChecked(value.id)) {
                var names = new Array;
                names.push(ulfirst(value.name));
                var currentNode = value.parent;
                while (currentNode.name != null) {
                    names.push(ulfirst(currentNode.name));
                    currentNode = currentNode.parent;
                }
                names.reverse();
                checkedCategoriesNames.push(names.join(" — "));
            }
            if (value.selected && value.type == "category") {
                checkedCategoriesIds.push(value.id);
            }
        });
    };

    function createCheckedBooksArray() {
        checkedBookIds = new Array();
        $.each(allNodes, function (index, value) {
            if (value.selected && value.type == "book" && !value.parent.selected) {
                checkedBookNames.push(value.name);
            }
            if (value.selected && value.type == "book") {
                checkedBookIds.push(value.id);
            }
        });
    };
    
    function getSelectedSourcesHtml() {
        if ((checkedBookNames.length == 0 && checkedCategoriesNames.length == 0) || (checkedCategoriesNames.length + checkedBookNames.length == allNodes.length)) {
            return "<span class=\"muted\">Aktivní prohledávání ve všech dostupných dílech</span>";
        }

        var categoriesString = checkedCategoriesNames.join(", ");
        var booksString = checkedBookNames.join(", ");

        if (checkedCategoriesNames.length == 0) {
            categoriesString = "-";
        }

        if (checkedBookNames.length == 0) {
            booksString = "-";
        }

        checkedCategoriesNames = new Array;
        checkedBookNames = new Array;
        return "<div class=\"muted\">" +
            "<strong>Kategorie:</strong> " + categoriesString + "<br>" +
            "<strong>Díla:</strong> " + booksString;
    };
    
    function getUrlCatBookParams() {
        var categoriesString = checkedCategoriesIds.join("+");
        var booksString = checkedBookIds.join("+");
        
        return "kategorie=" + categoriesString + "&dila=" + booksString;
    };

};
var rootNode = new TreeNode;
var allNodes = new Object;
var checkedCheckboxes = new Array;
var uncheckedCheckboxes = new Array;
var checkedCategoriesNames = new Array;
var checkedBookNames = new Array;
var checkedCategoriesIds = new Array;
var checkedBookIds = new Array;
var loadedCheckedCategories = new Array;
var loadedCheckedBook = new Array;
/************************************************************************
 *  Advance search category tree
 ***********************************************************************/





/************************************************************************
 *  Tabs loading
 ***********************************************************************/
var History = function() {
    this.main = null;
    this.alphabet = null;
    this.type = null;

    this.initHash = function () {
        var hash = window.location.hash.replace('#', '');;
        var hashAr = hash.split("+");
        if (hashAr.length == 3) {
            this.main = hashAr[0];
            this.alphabet = hashAr[1];
            this.type = hashAr[2];

            $("#search-results-ordering > ul > li > a[href='#" + this.main + "']").first().tab("show");
            $("#search-results-alphabetical > ul > li > a[href='#" + this.alphabet + "']").first().tab("show");
            $("#results-type > ul > li > a[href='#" + this.type + "']").first().tab("show");
        } else {
            $("#search-results-ordering > ul > li > a").first().tab("show");
            $("#search-results-alphabetical > ul > li > a").first().tab("show");
            $("#results-type > ul > li > a").first().tab("show");
        }
    };

    this.getHash = function() {
        return this.main + "+" + this.alphabet + "+" + this.type;
    };

    this.renewHash = function() {
        window.location.hash = "#" + this.getHash();
    };
};
var history = new History;

(function ($) {

    $.fn.extend({
        initLoadingSearchResults: function (options) {
            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                element.on('shown', function (e) {
                    history.main = element.attr("data-result");
                    history.renewHash();
                });
            });
        }
    });
})(jQuery);

(function ($) {

    $.fn.extend({
        initLoadingAlphTermDetail: function (options) {
            var loadingHTML = "<div class=\"loading\"></div>";

            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                var show = false;

                element.on('shown', function (e) {
                    history.alphabet = element.attr("data-result");
                    history.renewHash();

                    if (!show) {
                        var tabElement = $("#" + element.attr("data-result"));
                        tabElement.html(loadingHTML);
                        $.get(element.attr("data-url"), function (data) {
                            tabElement.html(data);
                        });
                        show = true;
                    }
                });
            });
        }
    });
})(jQuery);


(function ($) {

    $.fn.extend({
        initLoadingTypeTermDetail: function (options) {
            var loadingHTML = "<div class=\"loading\"></div>";

            var defaults = {};

            var options = $.extend(defaults, options);

            return this.each(function () {
                var element = $(this);
                var show = false;

                element.on('shown', function (e) {
                    history.type = element.attr("data-result");
                    history.renewHash();
                    
                    if (!show) {
                        var tabElement = $("#" + element.attr("data-result"));
                        tabElement.html(loadingHTML);
                        $.get(element.attr("data-url"), function (data) {
                            tabElement.html(data);
                        });
                        show = true;
                    }
                });
            });
        }
    });
})(jQuery);

/************************************************************************
 *  Tabs loading
 ***********************************************************************/


