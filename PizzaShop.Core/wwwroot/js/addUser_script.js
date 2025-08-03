$(document).ready(function () {
    toastr.options.closeButton = true;
    // Handle country change
    $('#countrydropdown').change(function () {
        var countryId = $(this).val();
        $('#statedropdown').html('<option value="">Select State</option>');
        $('#citydropdown').html('<option value="">Select City</option>');

        console.log("Selected Country ID: " + countryId);

        if (countryId) {
            $.ajax({
                url: '/UserTable/GetStates',
                type: 'GET',
                data: { countryId: countryId },
                success: function (states) {
                    $.each(states, function (i, state) {
                        $('#statedropdown').append('<option value="' + state.stateid + '">' + state.statename + '</option>');
                    });
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching states: " + error);
                }
            });
        }
    });

    // Handle state change
    $('#statedropdown').change(function () {
        var stateId = $(this).val();
        $('#citydropdown').html('<option value="">Select City</option>');

        if (stateId) {
            $.ajax({
                url: '/UserTable/GetCities',
                type: 'GET',
                data: { stateId: stateId },
                success: function (cities) {
                    $.each(cities, function (i, city) {
                        $('#citydropdown').append('<option value="' + city.cityid + '">' + city.cityname + '</option>');
                    });
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching cities: " + error);
                }
            });
        }
    });

    $("#togglePassword").click(function () {
        let passwordField = $("#passwordField");
        let icon = $(this).find("i");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        } else {
            passwordField.attr("type", "password");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        }
    });

    var errorMessage = window.error;
    if (errorMessage) {
        toastr.error(errorMessage, { timeOut: 5000 });
    }

    $("#imageInput").on("change", function () {
        var file = this.files[0];
        if (file) {
            $("#fileNameDisplay").text("Selected File: " + file.name);
        } else {
            $("#fileNameDisplay").text("");
        }
    });

});