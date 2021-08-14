$(function () {
    $('[data-toogle="tooltip"]').tooltip();

    function getNotifications(){
        $.ajax({
            url: "/Notifications/getNotifications",
            method: "GET",
            success: function(result){
                $("#notificationsCount").html(result.count);
                console.log(result);
            },
            error: function(error){
                console.log(error);
            }
        });
    }

    getNotifications();
});