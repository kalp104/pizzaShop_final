$(document).ready(function () {
    toastr.options.closeButton = true;
    console.log("Document ready");

    // handle edit user submission
    $(document).on('submit','#EditUserForm',function (event) {
        event.preventDefault(); 
        var formData = new FormData(this); 
        $.ajax({
            url: '/UserTable/EditUserById', 
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error occurred: " + error);
                toastr.error("An error occurred while processing your request.");
            }
        });
    });


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
    $("#imageInput").on("change", function () {
        var file = this.files[0];
        if (file) {
            $("#fileNameDisplay").text("Selected File: " + file.name);
        } else {
            $("#fileNameDisplay").text("");
        }
    });

});