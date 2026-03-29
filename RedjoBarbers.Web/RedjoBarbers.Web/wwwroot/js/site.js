function handleLogoutRedirect() {
    const logoutMessage = document.getElementById("logoutMessage");

    if (!logoutMessage) return;

    const countdownElement = document.getElementById("countdown");
    const redirectUrl = logoutMessage.dataset.redirect;

    let seconds = 2;

    const interval = setInterval(() => {
        seconds--;
        if (seconds > 0 && countdownElement) {
            countdownElement.textContent = seconds;
        } else {
            clearInterval(interval);
        }
    }, 1000);

    setTimeout(() => {
        logoutMessage.classList.add("fade-out");
    }, 2000);

    setTimeout(() => {
        window.location.href = redirectUrl;
    }, 2500);
}

document.addEventListener("DOMContentLoaded", handleLogoutRedirect);