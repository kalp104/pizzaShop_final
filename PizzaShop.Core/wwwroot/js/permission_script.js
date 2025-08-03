$(document).ready(function () {
    toastr.options.closeButton = true;

    // Function to update row checkbox based on switch states
    function updateRowCheckbox($row) {
        var anySwitchChecked = $row.find('.permission-switch:checked').length > 0;
        $row.find('.checkboxes').prop('checked', anySwitchChecked);
    }

    // Function to enforce switch dependencies and disabled states
    function enforceDependencies($row) {
        if ($row.find('.can-view').is(':disabled')) {
            updateRowCheckbox($row);
            return;
        }
        var canView = $row.find('.can-view').is(':checked');
        var canEdit = $row.find('.can-edit').is(':checked');

        // If Can View is off, turn off and disable Can Edit and Can Delete
        if (!canView) {
            $row.find('.can-edit').prop('checked', false).prop('disabled', true);
            $row.find('.can-delete').prop('checked', false).prop('disabled', true);
        } else {
            // Enable Can Edit and Can Delete if Can View is on
            $row.find('.can-edit').prop('disabled', false);
            $row.find('.can-delete').prop('disabled', !canEdit);
        }

        // If Can Edit is off, turn off and disable Can Delete
        if (!canEdit) {
            $row.find('.can-delete').prop('checked', false).prop('disabled', true);
        } else if (canView) {
            // Enable Can Delete if Can Edit and Can View are on
            $row.find('.can-delete').prop('disabled', false);
        }

        // Update checkbox state
        updateRowCheckbox($row);
    }

    // Row checkbox behavior
    $('.checkboxes').on('change', function () {
        var $checkbox = $(this);
        var $row = $checkbox.closest('tr');
        var isChecked = $checkbox.is(':checked');
        
        // Set all switches in the same row
        $row.find('.permission-switch').prop('checked', isChecked);
        // Enforce dependencies
        enforceDependencies($row);
    });

    // Can View switch behavior
    $('.can-view').on('change', function () {
        var $switch = $(this);
        var $row = $switch.closest('tr');
        enforceDependencies($row);
    });

    // Can Edit switch behavior
    $('.can-edit').on('change', function () {
        var $switch = $(this);
        var $row = $switch.closest('tr');
        enforceDependencies($row);
    });

    // Can Delete switch behavior
    $('.can-delete').on('change', function () {
        var $switch = $(this);
        var $row = $switch.closest('tr');
        enforceDependencies($row);
    });

    // Main checkbox button behavior
    $('#mainCheckBox').on('click', function (e) {
        e.preventDefault();
        var anyUnchecked = $('.permission-switch:not(:disabled):not(:checked)').length > 0;
        var newState = anyUnchecked;
        
        $('.checkboxes').each(function () {
            var $checkbox = $(this);
            var $row = $checkbox.closest('tr');
            $checkbox.prop('checked', newState);
            $row.find('.permission-switch').prop('checked', newState);
            enforceDependencies($row);
        });
    });

    // Initialize: Enforce dependencies and switch states on load
    $('tbody tr').each(function () {
        var $row = $(this);
        var $checkbox = $row.find('.checkboxes');
        var $canView = $row.find('.can-view');
        var $canEdit = $row.find('.can-edit');
        var $canDelete = $row.find('.can-delete');

        // Set checkbox based on any enabled switch being checked
        var anySwitchChecked = $row.find('.permission-switch:not(:disabled):checked').length > 0;
        $checkbox.prop('checked', anySwitchChecked);

        // Enforce dependencies based on model values
        enforceDependencies($row);
    });

    var successMessage = window.success;
    if (successMessage) {
        toastr.success(successMessage, 'Success', { timeOut: 3000 });
    }
    var ErrorPermission = window.error;
    if (ErrorPermission) {
        toastr.error(ErrorPermission, 'Error', { timeOut: 3000 });
    }
});