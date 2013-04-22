
$(document).ready(function () {
    /*$('#Messages').tinyscrollbar();*/
    /*$('#revisions').combobox();*/
    updateMessages();
    setupAutocomplete();

    $('#Messages').scrollTop = $('#Messages').scrollHeight;

    

    setInterval(function () {
        updateMessages();
    }, 1000);
});

function setupAutocomplete() {

    $("#AddEditorEdit").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Projects/FetchEditors",
                data: { id: $projectID,
                        email: $('#AddEditorEdit').val()
                        },
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; },
                success: function (data) {
                    var objData = jQuery.parseJSON(data);
                    response($.map(objData, function (item) {
                        return {
                            label: item,
                            value: item
                        }
                    }))
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(textStatus);
                }
            });
        },
        minLength: 2
    });
}

function sendMessage() {

    $.ajax({
        type: "POST",
        dataType: 'json',
        url: "/Project/PushMessage",
        data: {
            projId: $projectID,
            chatMessage: $('#NewMessageEdit').val()
        },
        success: function () {
            $('#NewMessageEdit').val('');
        },
        error: function () {
            alert('An error occurred uploading data.');
        }

    }).done(function (msg) {
        /*alert("Data Saved: " + msg);*/
    });
};

function updateMessages() {
    $url = "/Project/FetchMessages?id=" + $projectID;

    var messages;

    $.getJSON($url, null, function (data) {
        messages = jQuery.parseJSON(data);

        drawAllMessages(messages);
    });
}

function drawAllMessages(messages) {
    var scrollsave = $('#Messages').scrollTop();

    var scrlHeight = $('#Messages')[0].scrollHeight;
    var LastLine = true;
    if (scrollsave != scrlHeight) {
        LastLine = false;
    }

    $('#Messages').empty();
    $.each(messages, function (index, message) {
        drawMessage(message.SentTime, null, message.Content);
    });

    if (LastLine == true)
        $('#Messages').scrollTop();
    else
        $('#Messages').scrollTop(scrollsave);
}

function drawMessage(date, user, content) {
    var messages = $('#Messages');
    var newMessage = document.createElement('div');
    newMessage.className = "Message";
    newMessage.textContent = date + "   " + content;

    $('#Messages').append($(newMessage)).fadeIn();
}
