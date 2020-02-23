document.getElementById("B").addEventListener("click", function () {
    document.execCommand('bold', false, null);
});
document.getElementById("I").addEventListener("click", function () {
    document.execCommand('italic', false, null);
});
document.getElementById("U").addEventListener("click", function () {
    document.execCommand('underline', false, null);
});