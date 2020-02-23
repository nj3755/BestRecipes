$(document).ready(function () {
    $("#editImage").submit(function (e) {
        editImagePost();
        e.preventDefault();
    });
    $("#editEmailForm").submit(function (e) {
        emailPost();
        e.preventDefault();
    });
    $("#editPasswordForm").submit(function (e) {
        newPasswordPost();
        e.preventDefault();
    });
    $("#editTextForm").submit(function (e) {
        var input = $('#textInput').html();
        $('#textToPost').val(input);
        editTextPost();
        e.preventDefault();
    });
});

function editTextPost() {
    var form = $("#editTextForm");
    var formAction = $(form).attr("action");
    var fdata = $(form).serialize();
    normalAjaxPost(formAction, fdata);
}

function newPasswordPost() {
    var form = $("#editPasswordForm");
    var formAction = $(form).attr("action");
    var fdata = $(form).serialize();
    normalAjaxPost(formAction, fdata);
}

function emailPost() {
    var form = $("#editEmailForm");
    var formAction = $(form).attr("action");
    var fdata = $(form).serialize();
    normalAjaxPost(formAction, fdata);
}


function clearErrorElements(errorElementIds) {
    errorElementIds.forEach(function (element) {
        $("#" + element).text("");
    });
}

function normalAjaxPost(formAction, formData) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        url: formAction,
        data: formData,
        success: function (data) {
            if (data.success) {
                $("#" + data.elementId).text(data.message);
                clearErrorElements(data.errorElementIds);
            } else {
                $("#" + data.elementId).text(data.message);
            }
        }
    });
}

function editImagePost() {
    var form = $("#editImage");
    var formAction = $(form).attr("action");
    var fdata = new FormData(form[0]);

    $.ajax({
        type: 'post',
        dataType: 'json',
        url: formAction,
        data: fdata,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.success) {
                $("#" + data.elementId).attr("src", data.imageSrc);
                clearErrorElements(data.errorElementIds);
            } else {
                $("#" + data.elementId).text(data.message);
            }
        }
    });
}
