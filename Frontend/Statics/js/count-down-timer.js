let maximumTimeInSecond = 90;

function UpdateCountdownTimer()
{
    if(maximumTimeInSecond != 0)
    {
        maximumTimeInSecond--;
        document.getElementById("countDownTimerSeconds").textContent = maximumTimeInSecond;
        if(maximumTimeInSecond == 0)
        {
            document.getElementById("resend-button-container").style.display = "block";
            document.getElementById("countDownTimerContainer").style.display = "none";
        }
    }
}

setInterval(UpdateCountdownTimer, 1000);