$(document).ready(function () {

    $.ajax({
        url: '/Categories/BuildCategoriesTable',
        success: function (result) {
            $('#tableDiv2').html(result);
        }
    })
});