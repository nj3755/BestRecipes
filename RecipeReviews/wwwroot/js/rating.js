
$(document).ready(function () {
    $("#1stars").click(function (e) { newRating(e); });
    $("#2stars").click(function (e) { newRating(e); });
    $("#3stars").click(function (e) { newRating(e); });
    $("#4stars").click(function (e) { newRating(e); });
    $("#5stars").click(function (e) { newRating(e); });
});

function newRating(event) {
    $rating = event.target.id.substring(0, 1);
    $("#rating").text($rating);
    var url = $("#ratingAction").attr("href");
    var fdata = new FormData();
    fdata.append("recipeId", $("#recipeId").attr("value"));
    fdata.append("rating", $("#rating").text());
    postRating(url, fdata);
}

function postRating(url, data) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        url: url,
        data: data,
        processData: false,
        contentType: false,
        success: function (data) {
            if (data.success) {
                changeRatingStars(data.rating);
            }
            $("#rating").text(data.rating);
        }
    });
}

function changeRating(event) {
    $rating = event.target.id.substring(0, 1);
    $("#rating").text($rating);
    changeRatingStars($rating);
}

function changeRatingStars(rating) {
    rating = parseFloat(rating);
    for (var i = 1; i <= 5; i++) {
        $id = "#" + i.toString() + "stars";
        if (i <= rating) {
            $($id).addClass("checked");
        } else {
            $($id).removeClass("checked");
        }
    }
}