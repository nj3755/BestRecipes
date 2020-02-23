$(document).ready(function () {
    $("#editImage").submit(function (e) {
        editImagePost();
        e.preventDefault();
    });
    $("#editTextForm").submit(function (e) {
        var input = $('#textInput').html();
        $('#textToPost').val(input);
        editTextPost();
        e.preventDefault();
    });
    $("#tagList").change(function () {
        if ($("#tagsRequestInput option[value='" + this.value + "']").length === 0) {
            addTag(this.value);
            $(this).get(0).selectedIndex = 0;
        }
    });
});

function addTag(name) {
    var div = document.createElement("div");
    div.setAttribute("id", name);
    div.classList.add("tag-container");

    var tag = document.createElement("span");
    tag.classList.add("tag");
    tag.appendChild(document.createTextNode(name));
    var delButton = document.createElement("i");
    var deleteBtnId = "d" + name;
    delButton.setAttribute("id", deleteBtnId);
    delButton.classList.add("fa", "fa-times", "delete-tag");
    delButton.setAttribute("aria-hidden", "true");

    tag.appendChild(delButton);
    div.appendChild(tag);

    document.getElementById("tagsContainer").appendChild(div);
    document.getElementById(deleteBtnId).addEventListener("click", removeTag);

    var tagsInput = document.getElementById("tagsRequestInput");
    var option = document.createElement("option");
    option.setAttribute("value", name);
    tagsInput.add(option); 
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

function removeTag() {
    var tag = event.target.id.substring(1);
    $("#" + tag).remove();

    $("#tagsRequestInput option[value='" + tag + "']").remove();
}

function editTextPost() {
    var form = $("#editTextForm");
    var formAction = $(form).attr("action");
    var fdata = $(form).serialize();
    normalAjaxPost(formAction, fdata);
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

function clearErrorElements(errorElementIds) {
    errorElementIds.forEach(function (element) {
        $("#" + element).text("");
    });
}

function recipeCreationPost(e) {
    e.preventDefault();

    var formAction = $(this).attr("action");
    var form = $("#recipeForm")[0];
    var fdata = new FormData(form);
    fdata.delete("tagList");

    var options = document.getElementById("tagsRequestInput");
    var tags = [];
    for (var i = 0; i < options.length; i++) {
        fdata.append('TagNames', options[i].getAttribute("value"));
    }

    $.ajax({
        type: "post",
        dataType: "json",
        url: formAction,
        data: fdata,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.success) {
                location.href = data.redirectLocation;
            } else {
                $("#" + data.elementId).text(data.message);
            }
        }
    });
}