$(document).ready(function () {
    toastr.options.closeButton = true;
    var sectionId = null;
    var totalPersons = null;

    $('.KOTLink').on('click', function () {
        $('.KOTLink').removeClass('active');
        $(this).addClass('active');
    });

    function FetchWaitingList() {
        sectionId = $('.KOTLink.active').data('section-id');
        $.ajax({
            url: '/OrderApp/GetWaitingLists',
            type: 'GET',
            data: { sectionId: sectionId },
            success: function (data) {
                $('#WaitingListContainer').html(data);
            },
            error: function (xhr, status, error) {
                toastr.error('Error fetching waiting list.');
            }
        });
    }

    $('.KOTLink').on('click', function () {
        sectionId = $(this).data('section-id');
        FetchWaitingList();
    });

    $('.WaitingTokenButtonAtWaiting').on('click', function () {
        //$('#WaitingTokenModalAtWaiting').modal('show');
        var modalElement = document.getElementById('WaitingTokenModalAtWaiting');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();
    });


    $("#EmailInput").on('input', function () {
        var email = $(this).val();
        if (email.length > 0) {
            $('.customersDetails').removeClass('d-none');
        }
        else {
            $('.customersDetails').addClass('d-none');
        }
        if (email.length > 0) {
            $.ajax({
                url: '/OrderApp/GetCustomerDetails',
                type: 'GET',
                data: { email: email },
                success: function (data) {
                    if (data.success == false) {
                        $('.customersDetails').addClass('d-none');
                    } else {
                        $('.customersDetails').removeClass('d-none').html(data);
                    }
                },
                error: function (xhr, status, error) {
                    toastr.error('Error fetching customer details.');
                }
            });
        } else {
            $('.customersDetails').addClass('d-none');
        }
    })


    //edit waiting token
    $(document).on('click', '.editWaitingToken', function () {
        var waitingTokenId = $(this).data('waiting-id');
        var customerName = $(this).data('customer-name');
        var totalPersons = $(this).data('total-persons');
        var customerEmail = $(this).data('customer-email');
        var customerPhone = $(this).data('customer-phone');
        var sectionId = $(this).data('section-id');
        var sectionName = $(this).data('section-name');


        //$('#EditWaitingTokenModal').modal('show');
        var modalElement = document.getElementById('EditWaitingTokenModal');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();

        $('#EditNameInput').val(customerName);
        $('#EditTotalPerson').val(totalPersons);
        $('#EditEmailInput').val(customerEmail);
        $('#EditPhoneInput').val(customerPhone);
        $('#EditorderStatusFilter').val(sectionId);
        $('#EditSectionName').val(sectionName);
        $('#EditWaitingTokenId').val(waitingTokenId);

    });

    // delete waiting token
    $(document).on('click', '.deleteWaitingToken', function () {
        var waitingTokenId = $(this).data('id');
        $('#deleteWaitingTokenId').val(waitingTokenId);

        //$('#deleteWaitingModal').modal('show');
        var modalElement = document.getElementById('deleteWaitingModal');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();
    });

    // aasign table
    $(document).on('click', '.assignTable', function () {
        var waitingTokenId = $(this).data('waiting-id');
        var customerName = $(this).data('customer-name');
        totalPersons = $(this).data('total-persons');
        var customerEmail = $(this).data('customer-email');
        var customerPhone = $(this).data('customer-phone');
        var sectionId = $(this).data('section-id');
        var sectionName = $(this).data('section-name');
        $('#CustomernameAttAdd').val(customerName);
        $('#CustomeremailAttAdd').val(customerEmail);
        $('#CustomerphoneAttAdd').val(customerPhone);
        $('#TotalPersonsAttAdd').val(totalPersons);
        $('#SectionAtAdd').val(sectionId);
    
        $.ajax({
            url: '/OrderApp/GetTables',
            type: 'GET',
            data: { 
                sectionId: sectionId,
                capacity: totalPersons
             },
            success: function (data) {
                console.log(data.data);
                if (data.success == false) {
                    $('#TablesAtAdd').html('<option value="">No tables available</option>');
                } else {
                    var options = '<option value="">Select Table</option>';
                    $.each(data.data, function (index, table) {
                        options += '<option value="' + table.tableid + '">' + table.tablename + ' (capacity : ' + table.capacity + ')' + '</option>';
                    });
                    $('#TablesAtAdd').html(options);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('Error fetching tables.');
            }
        });



        $('#WaitingIdAtAddAttAdd').val(waitingTokenId);

        var modalElement = document.getElementById('AssignTableModel');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();
    });

    $(document).on("submit","#AddCustomerFromWaitingList", function(e){
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            url: '/OrderApp/AddCustomer',
            type: 'POST',
            data: formData,
            dataType: "json",
            success: function (response) {
                if(response.success)
                {
                    window.location.href = '/OrderApp/Menu?tableId=' + response.tableId ;
                }else{
                    toastr.error(response.message,"error",{ timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error("Error adding Customer:", xhr, status, error);
                toastr.error(
                    "An error occurred while adding Customer.",
                    "Error",
                    { timeOut: 3000 }
                );
            }
        })

    })

    $("#SectionAtAdd").on('change', function () {
        sectionId = $(this).val();

        $.ajax({
            url: '/OrderApp/GetTables',
            type: 'GET',
            data: { 
                sectionId: sectionId,
                capacity : totalPersons
            },
            success: function (data) {
                console.log(data.data);
                if (data.success == false) {
                    $('#TablesAtAdd').html('<option value="">No tables available</option>');
                } else {
                    var options = '<option value="">Select Table</option>';
                    $.each(data.data, function (index, table) {
                        options += '<option value="' + table.tableid + '">' + table.tablename + ' (capacity : ' + table.capacity + ')' + '</option>';
                    });
                    $('#TablesAtAdd').html(options);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('Error fetching tables.');
            }
        });
    });

    FetchWaitingList();
}); 