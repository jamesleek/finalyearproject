$(document).ready(function(){

    $.ajax({
        url: '/BugReports/BuildBugReportTable',
        success: function (result){
            $('#tableDiv').html(result);
        }
    })
});