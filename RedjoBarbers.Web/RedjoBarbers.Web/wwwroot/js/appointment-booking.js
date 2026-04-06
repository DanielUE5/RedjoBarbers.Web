document.addEventListener("DOMContentLoaded", function () {
    const barberSelect = document.getElementById("barberId");
    const serviceSelect = document.getElementById("barberServiceId");
    const appointmentDateInput = document.getElementById("appointmentDate");
    const appointmentTimeSelect = document.getElementById("appointmentTime");

    if (!barberSelect || !serviceSelect || !appointmentDateInput || !appointmentTimeSelect) {
        console.error("Липсва някой от нужните елементи.");
        return;
    }

    let selectedTimeValue = "";
    let initialDateValue = "";
    let initialDateTimeValue = appointmentDateInput.value;

    if (initialDateTimeValue) {
        if (initialDateTimeValue.includes("T")) {
            const parts = initialDateTimeValue.split("T");
            initialDateValue = parts[0];
            selectedTimeValue = parts[1]?.substring(0, 5) || "";
        } else {
            const parsedDate = new Date(initialDateTimeValue);

            if (!isNaN(parsedDate.getTime())) {
                const year = parsedDate.getFullYear();
                const month = String(parsedDate.getMonth() + 1).padStart(2, "0");
                const day = String(parsedDate.getDate()).padStart(2, "0");
                const hours = String(parsedDate.getHours()).padStart(2, "0");
                const minutes = String(parsedDate.getMinutes()).padStart(2, "0");

                initialDateValue = `${year}-${month}-${day}`;
                selectedTimeValue = `${hours}:${minutes}`;
            }
        }
    }

    const datePicker = flatpickr(appointmentDateInput, {
        altInput: true,
        altFormat: "d.m.Y",
        dateFormat: "Y-m-d",
        minDate: "today",
        defaultDate: initialDateValue || null,
        disable: [
            function (date) {
                return date.getDay() === 1;
            }
        ],
        onChange: async function () {
            selectedTimeValue = "";
            appointmentDateInput.value = getSelectedDateValue() || "";
            await loadAvailableSlots();
        }
    });

    barberSelect.addEventListener("change", async function () {
        selectedTimeValue = "";
        await loadAvailableSlots();
    });

    serviceSelect.addEventListener("change", async function () {
        selectedTimeValue = "";
        await loadAvailableSlots();
    });

    appointmentTimeSelect.addEventListener("change", function () {
        const selectedDateValue = getSelectedDateValue();
        const selectedTime = appointmentTimeSelect.value;

        if (!selectedDateValue || !selectedTime) {
            return;
        }

        selectedTimeValue = selectedTime;
        appointmentDateInput.value = `${selectedDateValue}T${selectedTime}`;
    });

    function getSelectedDateValue() {
        if (!datePicker.selectedDates || datePicker.selectedDates.length === 0) {
            return "";
        }

        return datePicker.formatDate(datePicker.selectedDates[0], "Y-m-d");
    }

    function setTimeSelectState() {
        const barberId = barberSelect.value;
        const barberServiceId = serviceSelect.value;
        const selectedDateValue = getSelectedDateValue();

        appointmentTimeSelect.innerHTML = "";

        if (!barberId) {
            appointmentTimeSelect.disabled = true;
            appointmentTimeSelect.innerHTML = '<option value="">Първо изберете бръснар</option>';
            return false;
        }

        if (!barberServiceId) {
            appointmentTimeSelect.disabled = true;
            appointmentTimeSelect.innerHTML = '<option value="">После изберете услуга</option>';
            return false;
        }

        if (!selectedDateValue) {
            appointmentTimeSelect.disabled = true;
            appointmentTimeSelect.innerHTML = '<option value="">Накрая изберете дата</option>';
            return false;
        }

        appointmentTimeSelect.disabled = false;
        appointmentTimeSelect.innerHTML = '<option value="">Изберете час</option>';
        return true;
    }

    async function loadAvailableSlots() {
        const canLoad = setTimeSelectState();

        if (!canLoad) {
            return;
        }

        const barberId = barberSelect.value;
        const barberServiceId = serviceSelect.value;
        const selectedDateValue = getSelectedDateValue();

        const url =
            `/Appointment/GetAvailableSlots?date=${encodeURIComponent(selectedDateValue)}` +
            `&barberId=${encodeURIComponent(barberId)}` +
            `&barberServiceId=${encodeURIComponent(barberServiceId)}`;

        console.log("Дата:", selectedDateValue);
        console.log("Бръснар:", barberId);
        console.log("Услуга:", barberServiceId);
        console.log("Request URL:", url);

        try {
            const response = await fetch(url);

            if (!response.ok) {
                console.error("Невалиден response:", response.status);
                appointmentTimeSelect.innerHTML = '<option value="">Грешка при зареждане</option>';
                appointmentTimeSelect.disabled = true;
                return;
            }

            const slots = await response.json();
            console.log("Получени часове:", slots);

            appointmentTimeSelect.innerHTML = '<option value="">Изберете час</option>';

            if (!Array.isArray(slots) || slots.length === 0) {
                appointmentTimeSelect.innerHTML = '<option value="">Няма свободни часове</option>';
                appointmentTimeSelect.disabled = true;
                return;
            }

            appointmentTimeSelect.disabled = false;

            for (const slot of slots) {
                const option = document.createElement("option");
                option.value = slot;
                option.textContent = slot;

                if (selectedTimeValue && selectedTimeValue === slot) {
                    option.selected = true;
                }

                appointmentTimeSelect.appendChild(option);
            }

            if (selectedDateValue && selectedTimeValue) {
                appointmentDateInput.value = `${selectedDateValue}T${selectedTimeValue}`;
            }
        } catch (error) {
            console.error("Грешка при зареждане на часовете:", error);
            appointmentTimeSelect.innerHTML = '<option value="">Грешка при зареждане</option>';
            appointmentTimeSelect.disabled = true;
        }
    }

    setTimeSelectState();

    if (getSelectedDateValue() && barberSelect.value && serviceSelect.value) {
        loadAvailableSlots();
    }
});