function startCountdown() {

    let sc = parseInt("@ViewBag.SecondsCompleted");
    let mc = parseInt("@ViewBag.MinutesCompleted");
    let hc = parseInt("@ViewBag.HoursCompleted");


    let rs = parseInt("@ViewBag.RemainingSeconds");
    let rm = parseInt("@ViewBag.RemainingMinutes");
    let rh = parseInt("@ViewBag.RemainingHours");

    let totalInitialSeconds = (hc * 3600) + (mc * 60) + sc;
    let totalInitialSecondsRemaining = (rh * 3600) + (rm * 60) + rs;

    countdownInterval = setInterval(() => {
        totalInitialSeconds++;
        totalInitialSecondsRemaining--;

        let newHours = Math.floor(totalInitialSeconds / 3600);
        let newMinutes = Math.floor((totalInitialSeconds % 3600) / 60);
        let newSeconds = totalInitialSeconds % 60;

        let newHoursRemaining = Math.floor(totalInitialSecondsRemaining / 3600);
        let newMinutesRemaining = Math.floor((totalInitialSecondsRemaining % 3600) / 60);
        let newSecondsRemaining = totalInitialSecondsRemaining % 60;

        // Update the countdown displayed on the HTML element
        document.getElementById('countdown').innerText = `${newHours}: ${newMinutes}: ${newSeconds}`;
        document.getElementById('countdownRemaining').innerText = `${newHoursRemaining}: ${newMinutesRemaining}: ${newSecondsRemaining}`;

    }, 1000);
}

function stopCountdown() {
    clearInterval(countdownInterval);
}