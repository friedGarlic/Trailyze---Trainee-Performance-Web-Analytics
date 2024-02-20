$(document).ready(function () {
    $("#Input_Password").on("keyup", function () {
        var regex = /^(?=.*[A-Z])(?=.*[0-9])(?=.*\W)[A-Za-z0-9\W]{8,}$/;
        var isValid = regex.test($(this).val());

        if (isValid) {
            $("#Input_Password").removeClass("invalid");
            $("#Input_Password").next(".text-danger").hide();
        } else {
            $("#Input_Password").addClass("invalid");
            $("#Input_Password").next(".text-danger").show().text("Password must contain at least 1 uppercase letter, 1 number, and 1 special character, and be at least 8 characters long.");
        }
    });
});