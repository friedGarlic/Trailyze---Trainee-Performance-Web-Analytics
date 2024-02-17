document.getElementById("IsTimedIn").addEventListener("click", function () {
    var slider = document.querySelector(".toggle-slider");
    slider.classList.toggle("on");
    document.getElementById("toggleForm").submit();
});

function showInputPopup() {
    Swal.fire({
        title: 'Add Reminder',
        html:
            '<div class="icon"><span class="material-icons-sharp">groups</span></div>' +
            '<div class="icon"><span class="material-icons-sharp">task</span></div>' +
            '<div class="icon"><span class="material-icons-sharp">devices</span></div>' +
            '<input id="swal-input1" class="swal2-input" placeholder="Name of reminder">',
        showCancelButton: true,
        confirmButtonText: 'Submit',
        cancelButtonText: 'Cancel',
        preConfirm: () => {
            const nameOfReminder = Swal.getPopup().querySelector('#swal-input1').value;
            // perform validation or additional processing here
            return {nameOfReminder: nameOfReminder };
        },
        allowOutsideClick: () => !Swal.isLoading()
    }).then((result) => {
        if (result.isConfirmed) {
            // data to the controller using AJAX
            sendDataToController(result.value);
        }
    });
}

function sendDataToController(data) {
    $.ajax({
        url: '/Dashboard/AddReminder',
        type: 'POST',
        dataType: 'json',
        data: data,
        success: function (response) {
            console.log("SUCCESS");
        },
        error: function (error) {
            console.error(error);
        }
    });
}