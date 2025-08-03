$(document).ready(function () {
    toastr.options.closeButton = true;
    var searchTerm = "";
    var taxModal = new bootstrap.Modal('#taxModal', { backdrop: 'static', keyboard: false });
    var deleteTaxModal = new bootstrap.Modal('#deleteTaxModal', { backdrop: 'static', keyboard: false });

    // Fetch tax data via AJAX
    function fetchData(searchTerm) {
        $.ajax({
            type: "GET",
            url: "/Tax/FatchTaxes",
            data: { searchTerm: searchTerm },
            success: function (data) {
                $("#TaxContainer").html(data);
            },
            error: function () {
                toastr.error("Error loading taxes.", "Error", { timeOut: 3000 });
            }
        });
    }

    // Search input handler
    $(document).on("input", "#search_tax", function () {
        searchTerm = $(this).val();
        fetchData(searchTerm);
    });

    // Open modal for adding a new tax
    $("#addTaxBtn").on("click", function () {
        $("#taxModalLabel").text("Add Tax");
        $("#taxForm").attr("action", "/Tax/AddTax");
        $("#TaxId").val("");
        $("#TaxName").val("");
        $("#taxType").val("");
        $("#TaxAmount").val("");
        $("#Isenabled").prop("checked", false);
        $("#Isdefault").prop("checked", false);
        taxModal.show();
    });

    // Open modal for editing an existing tax
    $(document).on("click", ".edit-Tax-link", function () {
        var TaxId = $(this).data("tax-id");
        var TaxName = $(this).data("tax-name");
        var TaxType = $(this).data("tax-type");
        var TaxIsEnabled = $(this).data("tax-isenabled");
        var TaxIsDefault = $(this).data("tax-isdefault");
        var TaxValue = $(this).data("tax-value");

        $("#taxModalLabel").text("Edit Tax");
        $("#taxForm").attr("action", "/Tax/EditTax");
        $("#TaxId").val(TaxId);
        $("#TaxName").val(TaxName);
        $("#taxType").val(TaxType);
        $("#TaxAmount").val(TaxValue);
        $("#Isenabled").prop("checked", TaxIsEnabled === true || TaxIsEnabled === 'True');
        $("#Isdefault").prop("checked", TaxIsDefault === true || TaxIsDefault === 'True');
        taxModal.show();
    });

    // Open delete tax modal
    $(document).on("click", ".delete-Tax-link", function () {
        var TaxId = $(this).data("tax-id");
        $("#deleteTaxId").val(TaxId);
        deleteTaxModal.show();
    });

    // Add/Edit tax form submission via AJAX
    $("#taxForm").on("submit", function (e) {
        e.preventDefault();
        var $form = $(this);
        if ($form.valid()) {
            $.ajax({
                type: "POST",
                url: $form.attr("action"),
                data: $form.serialize(),
                success: function (response) {
                    if (response.success) {
                        toastr.success(response.message, "Success", { timeOut: 3000 });
                        fetchData(searchTerm);
                        $form[0].reset();
                        taxModal.hide();
                        removeBackdrop();
                    } else {
                        toastr.error(response.message, "Error", { timeOut: 3000 });
                    }
                },
                error: function () {
                    toastr.error("Error saving tax.", "Error", { timeOut: 3000 });
                }
            });
        }
    });

    // Delete tax form submission via AJAX
    $("#deleteTaxForm").on("submit", function (e) {
        e.preventDefault();
        var $form = $(this);
        $.ajax({
            type: "POST",
            url: $form.attr("action"),
            data: $form.serialize(),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message, "Success", { timeOut: 3000 });
                    fetchData(searchTerm);
                    deleteTaxModal.hide();
                    removeBackdrop();
                } else {
                    toastr.error(response.message, "Error", { timeOut: 3000 });
                }
            },
            error: function () {
                toastr.error("Error deleting tax.", "Error", { timeOut: 3000 });
            }
        });
    });

    // Helper function to remove modal backdrop
    function removeBackdrop() {
        setTimeout(() => {
            document.body.classList.remove('modal-open');
            $('.modal-backdrop').remove();
        }, 300);
    }

    // Handle modal close (No or cross button)
    $('#taxModal, #deleteTaxModal').on('hidden.bs.modal', function () {
        removeBackdrop();
    });

    // Initial fetch
    fetchData(searchTerm);
}); 