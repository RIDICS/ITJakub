$(document).ready(function () {
    
    $("#UserNameEdit").autocomplete({
        source: function (request, response) {
            
                $.ajax({
                    url: "/Service/FetchUsers",
                    data: {
                        term: $('#UserNameEdit').val()
                    },
                    dataType: "json",
                    type: "POST",
                    success: function (data) {
                        var objData = jQuery.parseJSON(data);
                        response($.map(objData, function (item) {
                            return {
                                label: item.Name,
                                value: item.Name
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
});

function UserRoleBtn() {

    var userName = $("#UserNameEdit").val();
    var url = "/Setup/UserRoles?user=" + userName;
    window.location = url;


}

function AddDocTypeBtn() {

    // validate it first!!

    var userName = $("#AddDocTypeEdit").val();
    var url = "/Setup/AddDocType?typeName=" + userName;
    window.location = url;
}