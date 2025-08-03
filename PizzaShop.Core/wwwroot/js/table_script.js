$(document).ready(function () {

    toastr.options.closeButton = true;
    var tableIdList = [];

  
    $(document).on('click',".section-table-link", function () {
        $(".section-table-container").not($(this).next()).slideUp("fast");
        $(".section-table-link i").not($(this).find("i")).removeClass("bi-arrow-down-circle").addClass("bi-arrow-right-circle");
        $(this).closest(".section-table-link").next(".section-table-container").slideToggle("fast");
        $(this).find("i").toggleClass("bi-arrow-right-circle bi-arrow-down-circle");
        
        // Reset tableIdList and remove Selected class from all table cards
        tableIdList = [];
        $(".TableCardClass").removeClass("Selected");
        
        console.log("table list : ", tableIdList);
    });

    $(document).on('click',".TableCardClass", function () {

        var tableId = $(this).data("table-id");
        var tableStatus = $(this).data("table-status");
        var sectionId = $(this).data("section-id");
        if (tableStatus == 2 || tableStatus == 3) {
            window.location.href = "/OrderApp/AssignTableToMenu?tableId=" + tableId;
        }
        //else {
        //    toastr.error('Running Tables can not select','Error',{timeout:3000})
        //}
        if (tableIdList.includes(tableId)) {
            tableIdList = tableIdList.filter(function (value) {
                return value != tableId;
            });
        } else {
            tableIdList.push(tableId);
        }
        $("#orderStatusFilter").val(sectionId);
        console.log("table list : ", tableIdList);
        console.log("section id selected ",  $("#orderStatusFilter").val());
        $(this).toggleClass("Selected");
    });

    $(document).on('click',".AssingTable", function () {

        if (tableIdList.length == 0) {
            toastr.error("Please select a table", "Error", { setTimeout: 2000 });
            return;
        }
        $("#TableOffCanvas").offcanvas("show");
        var list = tableIdList.join(",");
        // console.log("table list : ", list);
        $("#tableIds").val(list);
        // console.log("hidden values", $("#tableIds").val(list));
    })


    $(document).on('click',".closeCanvas", function () {
        $("#TableOffCanvas").offcanvas("hide");
        tableIdList = [];
        $(".TableCardClass").removeClass("Selected");
        console.log("table list : ", tableIdList);
        $("#tableIds").val("");
        $("#CustomerName").val("");
        $("#TotalPersons").val("");
        $("#EmailInput").val("");
        $("#CustomerPhone").val("");
        $("#Waitingid").val("");

        // console.log($("#tableIds").val());
    });

    $(document).on('click',".WaitingTokenButton", function (e) {
        e.stopPropagation();
        var sectionid = $(this).data("section-id");
        $("#FilterSectionAtWaitingToken").val(sectionid);
        var modalElement = document.getElementById('WaitingTokenModal');
        var modal = new bootstrap.Modal(modalElement);
        modal.show();
    });




    $(document).on('click','.selectWaitingToken', function () {
        var selected = $(this).is(':checked');
        $('.selectWaitingToken').not(this).prop('checked', false); // Uncheck other checkboxes

        var waitingId = $(this).data("waiting-id");
        var customerName = $(this).data("customer-name");
        var totalPersons = $(this).data("total-persons");
        var customerEmail = $(this).data("customer-email");
        var customerPhone = $(this).data("customer-phone");
        var sectionId = $(this).data("section-id");
        var sectionName = $(this).data("section-name");



        if (selected) {
            $("#CustomerName").val(customerName);
            $("#TotalPersons").val(totalPersons);
            $("#EmailInput").val(customerEmail);
            $("#CustomerPhone").val(customerPhone);
            $("#Waitingid").val(waitingId);
        } else {
            $("#CustomerName").val("");
            $("#TotalPersons").val("");
            $("#EmailInput").val("");
            $("#CustomerPhone").val("");
            $("#Waitingid").val("");
        }
    });




    $(document).on('input',"#EmailInput", function () {
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

    $(document).on('input',"#EmailInputWaitingToken", function () {
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



    $(document).on('submit','#AddCustomerForm', function(e){
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            url: '/OrderApp/AddCustomer',
            type: "POST",
            data: formData,
            success: function (response) {
                if(response.success){
                    window.location.href = '/OrderApp/Menu?tableId=' + response.tableId ;
                }else{
                    toastr.error(response.message,"error",{ timeOut: 3000 });
                }
            },
            error: function (error) {
                toastr.error(
                  "An error occurred while Adding new customer.",
                  "Error",
                  { timeOut: 3000 }
                );
              },

        })
    })

    $(document).on('click','.AssingTable', function(e)
    {
        var sectionid = $(this).data('section-id');
        $.ajax({
            url: '/OrderApp/GetWaitingTokenBySectionId',
            type: "GET",
            data : {sectionId:sectionid},
            success : function (data)
            {
                if(data)
                {
                    // console.log(data);
                    $('#WaititngListContainer').html(data);
                }
            },
            error: function (error) {
                toastr.error(
                  "An error occurred while fetching token data.",
                  "Error",
                  { timeOut: 3000 }
                );
            }
        })
    })

});