var miCheckbox = document.getElementById("checkbox-all");
var checkboxes = document.querySelectorAll('input[type=checkbox]');
miCheckbox.addEventListener("change", function () {
    if (miCheckbox.checked) {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = true;
        });
    } else {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
    }
});
    