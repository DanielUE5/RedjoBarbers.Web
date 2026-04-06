document.addEventListener("DOMContentLoaded", function () {
    const acceptBtn = document.getElementById("accept-cookies");
    const rejectBtn = document.getElementById("reject-cookies");
    const manageBtn = document.getElementById("manage-cookies");
    const saveBtn = document.getElementById("save-cookie-settings");
    const closeBtn = document.getElementById("close-cookie-settings");
    const openSettingsBtn = document.getElementById("open-cookie-settings");
    const modal = document.getElementById("cookie-modal");
    const analyticsCheckbox = document.getElementById("analytics-consent");

    function setCookie(name, value, days) {
        const expires = new Date(Date.now() + days * 24 * 60 * 60 * 1000).toUTCString();
        document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expires}; path=/; SameSite=Lax; Secure`;
    }

    function getCookie(name) {
        const cookie = document.cookie
            .split("; ")
            .find(row => row.startsWith(name + "="));

        return cookie ? decodeURIComponent(cookie.split("=")[1]) : null;
    }

    function saveConsent(analyticsAllowed) {
        const payload = {
            necessary: true,
            analytics: analyticsAllowed,
            version: "1.0",
            timestamp: new Date().toISOString()
        };

        setCookie("cookie_consent", JSON.stringify(payload), 180);
        location.reload();
    }

    function openModal() {
        if (modal) {
            modal.classList.remove("d-none");
        }
    }

    function closeModal() {
        if (modal) {
            modal.classList.add("d-none");
        }
    }

    const existingConsent = getCookie("cookie_consent");

    if (existingConsent && analyticsCheckbox) {
        try {
            const parsed = JSON.parse(existingConsent);
            analyticsCheckbox.checked = parsed.analytics === true;
        } catch {
            analyticsCheckbox.checked = false;
        }
    }

    if (acceptBtn) {
        acceptBtn.addEventListener("click", function () {
            saveConsent(true);
        });
    }

    if (rejectBtn) {
        rejectBtn.addEventListener("click", function () {
            saveConsent(false);
        });
    }

    if (manageBtn) {
        manageBtn.addEventListener("click", function () {
            openModal();
        });
    }

    if (saveBtn) {
        saveBtn.addEventListener("click", function () {
            saveConsent(analyticsCheckbox ? analyticsCheckbox.checked : false);
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener("click", function () {
            closeModal();
        });
    }

    if (openSettingsBtn) {
        openSettingsBtn.addEventListener("click", function () {
            openModal();
        });
    }
});