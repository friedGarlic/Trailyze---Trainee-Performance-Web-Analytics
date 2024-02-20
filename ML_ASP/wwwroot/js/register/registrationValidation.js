/*$(document).ready(function () {
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
});*/
$(document).ready(function () {
    $("#Password").on("keyup", function () {
        // Store the password for clarity
        var password = $(this).val();

        // Separate check for each password strength criteria
        var hasNumber = /(?=.*\d)/.test(password);
        var hasSpecialChar = /(?=.*\W)/.test(password);
        var hasCapital = /(?=.*[A-Z])/i.test(password); // Case-Insensitive

        // Build error messages array based on failed requirements
        var errorMsgs = [];
        if (!hasNumber) {
            errorMsgs.push("Password must contain at least 1 number");
        }
        if (!hasSpecialChar) {
            errorMsgs.push("Password must contain at least 1 special character");
        }
        if (!hasCapital) {
            errorMsgs.push("Password must contain at least 1 uppercase letter");
        }

        // Update UI and display error messages
        if (errorMsgs.length) {
            $("#password-error").html(errorMsgs.join('<br>')).show();
            $("#Password").removeClass("valid").add("Invalid");
        } else {
            $("#password-error").hide();
            $("#Password").removeClass("Invalid").add("valid");
        }
    });
});