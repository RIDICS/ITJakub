$(document).ready(function() {
    $("#dropzoneFileForm").dropzone({
        url: '/Upload/UploadFile',
        maxFilesize: 10000, // MB
        maxFiles: 1,
        success: function(file, response) {
            fillFileMetadataForm(response);
        }
    });

    $("#dropzoneFrontImageForm").dropzone({
        url: '/Upload/UploadFrontImage',
        maxFilesize: 100, // MB
        maxFiles: 1,
        success: function(file, response) {
            if (response.Error) {
                alert(response.Error);
            }
        },
        sending: function(file, xhr, formData) {
            formData.append('fileGuid', getUploladingFileGuid());
        }

    });

    $("#dropzoneImagesForm").dropzone({
        url: '/Upload/UploadImages',
        maxFilesize: 100, // MB
        success: function(file, response) {
            if (response.Error) {
                alert(response.Error);
            }
        },
        sending: function(file, xhr, formData) {
            formData.append('fileGuid', getUploladingFileGuid());
        }
    });

});

function getUploladingFileGuid() {
    return $('ol.upload-file').attr('data-file-guid');
}

$("#fileMetadataForm").submit(function() {
    var formData = $("#fileMetadataForm").serializeArray();
    formData.push({name: "fileGuid", value: getUploladingFileGuid()});
    $.ajax({
        type: "POST",
        url: "/Upload/UploadMetadata",
        data: formData,
        success: function(response) {
            if (response.Error) {
                alert(response.Error);
            }
        }
    });
    return false; // prevent submit of the form.
});


function fillFileMetadataForm(data) {
    var form = document.getElementById("fileMetadataForm");

    //for (var index = 0; index < data.FileInfo.Name.size(); ++index) {
    //    console.log(a[index]);
    //    var authorInput = document.createElement('input');
    //    authorInput.setAttribute('type', 'text');
    //    authorInput.setAttribute('placeholder', 'Author');
    //    authorInput.setAttribute('name', 'author');
    //    authorInput.setAttribute('value', a[index]);
    //    form.appendChild(authorInput);
    //}

    var labelNameInput = document.createElement('label');
    labelNameInput.setAttribute('for', 'nameInput');
    labelNameInput.innerHTML = 'Název díla:';

    var nameInput = document.createElement('input');
    nameInput.setAttribute('type', 'text');
    nameInput.setAttribute('placeholder', 'Název díla:');
    nameInput.setAttribute('name', 'name');
    nameInput.setAttribute('id', 'nameInput');
    nameInput.setAttribute('value', data.FileInfo.Name);

    var labelAuthorInput = document.createElement('label');
    labelAuthorInput.setAttribute('for', 'authorInput');
    labelAuthorInput.innerHTML = 'Jméno autora:';

    var authorInput = document.createElement('input');
    authorInput.setAttribute('type', 'text');
    authorInput.setAttribute('placeholder', 'Autor');
    authorInput.setAttribute('name', 'author');
    authorInput.setAttribute('id', 'authorInput');
    authorInput.setAttribute('value', data.FileInfo.Author);


    var submit = document.createElement('input');
    submit.setAttribute('type', 'submit');
    submit.setAttribute('value', 'Potvrdit');

    form.appendChild(labelNameInput);
    form.appendChild(nameInput);
    form.appendChild(labelAuthorInput);
    form.appendChild(authorInput);
    form.appendChild(submit);

    $('ol.upload-file').attr('data-file-guid', data.FileInfo.Guid);
}