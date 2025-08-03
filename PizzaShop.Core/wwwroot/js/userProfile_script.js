$(document).ready(function () {
    toastr.options.closeButton = true;
    $('#fileInput').change(function () {
        if (this.files && this.files.length > 0) {
            // Submit the form when a file is selected
            $(this).closest('form').submit();
        }
    });


    $('#countrydropdown').change(function () {
        var countryId = $(this).val();
        $('#statedropdown').html('<option value="0">Select State</option>');
        $('#citydropdown').html('<option value="0">Select City</option>');

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
        $('#citydropdown').html('<option value="0">Select City</option>');

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



    var errorMessage = window.error;
    if (errorMessage) {
        toastr.error(errorMessage, 'Error', { timeOut: 5000 });
    }
    var successMessage = window.update;
    if (successMessage) {
        toastr.success(successMessage, 'success', { timeOut: 5000 });
    }
});