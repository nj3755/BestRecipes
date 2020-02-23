$(document).ready(function () {
    $("#recipeSortingOptions").change(function () {
        document.forms["sortingForm"].submit();
    });
});

function loginAjaxPost(form) {
    var formData = $(form).serialize();
    var ajaxConfig = {
        type: "POST",
        url: form.action,
        data: formData,
        success: function (response) {
            if (response.success) {
                location.href = "/";
            } else {
                $("#btnLoginDropdown").dropdown("toggle");
                $("#" + response.elementId).text(response.message);
            }
            
        }
    };
    $.ajax(ajaxConfig);

    return false;
}