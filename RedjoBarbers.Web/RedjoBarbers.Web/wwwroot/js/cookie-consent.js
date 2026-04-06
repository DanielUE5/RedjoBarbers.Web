document.addEventListener("DOMContentLoaded", () => {
    const banner = document.getElementById("cookieConsentBanner");
    const acceptBtn = document.getElementById("cookieAcceptBtn");
    const declineBtn = document.getElementById("cookieDeclineBtn");

    if (!banner) {
        return;
    }

    const consentKey = "redjo_cookie_consent";
    const existingConsent = localStorage.getItem(consentKey);

    if (!existingConsent) {
        banner.hidden = false;
    }

    const closeBanner = (value) => {
        localStorage.setItem(consentKey, value);
        banner.hidden = true;
    };

    if (acceptBtn) {
        acceptBtn.addEventListener("click", () => closeBanner("accepted"));
    }

    if (declineBtn) {
        declineBtn.addEventListener("click", () => closeBanner("declined"));
    }
});