$(function () {
    $('[data-toogle="tooltip"]').tooltip();

    function getNotifications(){
        $.ajax({
            url: "/Notifications/getNotifications",
            method: "GET",
            success: function (result) {

                if (result.count != 0) {
                    $("#notificationsCount").html(result.count);
                }
                else {
                    $("#notificationsCount").html();
                }
                console.log(result);
            },
            error: function(error){
                console.log(error);
            }
        });
    }

    getNotifications();
});