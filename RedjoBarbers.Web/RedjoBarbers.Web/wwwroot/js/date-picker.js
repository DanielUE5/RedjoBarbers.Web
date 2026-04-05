document.addEventListener("DOMContentLoaded", function () {
    const fromInput = document.querySelector('input[name="FromDate"]');
    const toInput = document.querySelector('input[name="ToDate"]');

    if (!fromInput || !toInput) {
        return;
    }

    let fromPicker;
    let toPicker;

    fromPicker = flatpickr(fromInput, {
        altInput: true,
        altFormat: "d.m.Y",
        dateFormat: "Y-m-d",
        allowInput: false,
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                toPicker.set("minDate", selectedDates[0]);

                const toDate = toPicker.selectedDates[0];
                if (toDate && toDate < selectedDates[0]) {
                    toPicker.clear();
                }
            } else {
                toPicker.set("minDate", null);
            }
        }
    });

    toPicker = flatpickr(toInput, {
        altInput: true,
        altFormat: "d.m.Y",
        dateFormat: "Y-m-d",
        allowInput: false,
        onChange: function (selectedDates) {
            if (selectedDates.length > 0) {
                fromPicker.set("maxDate", selectedDates[0]);

                const fromDate = fromPicker.selectedDates[0];
                if (fromDate && fromDate > selectedDates[0]) {
                    fromPicker.clear();
                }
            } else {
                fromPicker.set("maxDate", null);
            }
        }
    });

    if (fromPicker.selectedDates.length > 0) {
        toPicker.set("minDate", fromPicker.selectedDates[0]);
    }

    if (toPicker.selectedDates.length > 0) {
        fromPicker.set("maxDate", toPicker.selectedDates[0]);
    }
});