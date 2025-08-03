$(document).ready(function () {
    toastr.options.closeButton = true;

    // Variables for modifier group view
    var rowsPerPageMain = 5;
    var currentPageMain = 1;
    var totalItemsMain = window.totalItemsMain || 0;
    var selectedCategoryMain = null;
    var searchTermMain = '';
    var canEdit = window.canEdit;
    var canDelete = window.canDelete;
    var selectedModifierIdsMain = [];
    var filterModifiersUrl = window.filterModifiersUrl || '';
    var selectedModifierGroups = []; // Store selected modifier group IDs and names

    // Modal initialization
    var openAddModifierModal = new bootstrap.Modal('#addModifiers');
    var openDeleteModifierModal = new bootstrap.Modal('#staticBackdrop1-Modifier', { backdrop: 'static', keyboard: false });
    var openDeleteModifierGroupModal = new bootstrap.Modal('#delteModifierGroupModal', { backdrop: 'static', keyboard: false });
    var openAddModifierGroupModal = new bootstrap.Modal('#exampleModal1', { backdrop: 'static', keyboard: false });
    var openUpdateModifierModal = new bootstrap.Modal('#editModal-Modifier', { backdrop: 'static', keyboard: false });
    var openUpdateModifierGroupModal = new bootstrap.Modal('#editModal', { backdrop: 'static', keyboard: false });

    // Toggle custom dropdown visibility
    $('#itemsPerPageBtnMain').on('click', function () {
        var $menu = $('#itemsPerPageMenuMain');
        $menu.toggle();
    });

    // Close dropdown when clicking outside
    $(document).on('click', function (e) {
        var $dropdown = $('.custom-dropdown');
        if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
            $('#itemsPerPageMenuMain').hide();
        }
    });

    // Handle page size selection
    $(document).on('click', '.page-size-option', function (e) {
        e.preventDefault();
        var newSize = parseInt($(this).data('size'));
        if (newSize !== rowsPerPageMain) {
            rowsPerPageMain = newSize;
            $('#itemsPerPageBtnMain').html(`${rowsPerPageMain} <span><i class="bi bi-chevron-down"></i></span>`);
            currentPageMain = 1;
            fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
        }
        $('#itemsPerPageMenuMain').hide();
    });

    // Fetch modifier groups
    function fetchModifierGroups() {
        $.ajax({
            url: '/Menu/GetModifierGroupsAside',
            type: 'GET',
            success: function (data) {
                // console.log(data.modifierGroups);
                updateModifierGroupList(data.modifierGroups);
                fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
                updateModifierFilterList(data.modifierGroups);
                UpdateModifierFilterListEdit(data.modifierGroups);
            },
            error: function () {
                toastr.error("Error loading sections.", "Error", { timeOut: 3000 });
            },
        });
    }

    fetchModifierGroups();

    function updateModifierFilterList(ModifierGroups) {
        var $sectionList = $("#modifierGroupContainerAdd");
        $sectionList.empty();

        if (ModifierGroups.length === 0) {
            $sectionList.append('<div id="no-modifierGroup">No Modifier Groups</div>');
            return;
        }

        ModifierGroups.forEach(function (s) {
            var sectionHtml = `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input modifier-checkbox" name="modifiersViewModel.Modifiergroupid" value="${s.modifiergroupid}" id="modifiergroup_${s.modifiergroupid}" data-group-name="${s.modifiergroupname}">
                    <label class="form-check-label" for="modifiergroup_${s.modifiergroupid}">${s.modifiergroupname}</label>
                </div>`;
            $sectionList.append(sectionHtml);
        });
    }

    function UpdateModifierFilterListEdit(modifierGroups) {
        var $sectionList = $("#modifierGroupContainerEdit");
        $sectionList.empty();

        if (modifierGroups.length === 0) {
            $sectionList.append('<div id="no-modifierGroup">No Modifier Groups</div>');
            return;
        }

        modifierGroups.forEach(function (s) {
            var sectionHtml = `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" name="modifiersViewModel.Modifiergroupid" value="${s.modifiergroupid}" id="modifiergroup_${s.modifiergroupid}" data-group-name="${s.modifiergroupname}">
                    <label class="form-check-label" for="modifiergroup_${s.modifiergroupid}">${s.modifiergroupname}</label>
                </div>`;
            $sectionList.append(sectionHtml);
        });
    }

    function updateModifierGroupList(ModifierGroups) {
        var $sectionList = $("#modifierGroup-list");
        var $noSections = $("#no-modifierGroup");
        $sectionList.empty();

        if (ModifierGroups.length === 0) {
            $noSections.show();
            return;
        }

        $noSections.hide();

        if (!selectedCategoryMain && ModifierGroups.length > 0) {
            selectedCategoryMain = ModifierGroups[0].modifiergroupid;
            var modifierGroupName = ModifierGroups[0].modifierGroupName;
            selectedModifierGroups = [{ id: selectedCategoryMain, name: modifierGroupName }]; // Single selection
            $('#toggleModifiersGroupAdd').text(modifierGroupName);

        }
        console.log("seeeeee: ", selectedCategoryMain);

        ModifierGroups.forEach(function (s) {
            var isActive = s.modifiergroupid === selectedCategoryMain ? "active" : "";
            var textColor = s.modifiergroupid === selectedCategoryMain ? "text-white" : "text-dark";
            var pensVisible = s.modifiergroupid === selectedCategoryMain ? "" : "d-none";

            var activePensHtml = "";
            if (canEdit) {
                activePensHtml += `
                    <a href="#" class="text-primary edit-modifier-group" data-modifier-group-id="${s.modifiergroupid}"
                        data-modifier-group-name="${s.modifiergroupname}" data-modifier-group-description="${s.modifiergroupdescription}">
                        <i class="bi bi-pen mx-1"></i>
                    </a>`;
            }
            if (canDelete) {
                activePensHtml += `
                    <a href="#" class="text-primary delete-modifier-group-btn" data-modifier-group-id="${s.modifiergroupid}">
                        <i class="bi bi-trash"></i>
                    </a>`;
            }
            activePensHtml = `<div class="activePens ${pensVisible}">${activePensHtml}</div>`;

            var sectionHtml = `
                <li class="nav-link links modifierGroup-link d-flex justify-content-between align-items-center gap-2 ${isActive}"
                    id="v-pills-${s.modifiergroupid}-tab-modifier-group" data-modifier-group-id="${s.modifiergroupid}"
                    data-modifier-group-name="${s.modifiergroupname}" data-modifier-group-description="${s.modifiergroupdescription}">
                    <a class="text-decoration-none ${textColor}">
                        <i class="bi bi-grid-3x2-gap-fill me-2"></i>${s.modifiergroupname}
                    </a>
                    ${activePensHtml}
                </li>`;
            $sectionList.append(sectionHtml);
        });
    }

    // AJAX fetch function for modifiers
    function fetchModifiersMain(modifierGroupId, searchTerm = '', page, pageSize) {
        console.log("Fetching modifiers for group:", modifierGroupId);
        $.ajax({
            url: filterModifiersUrl,
            type: 'GET',
            data: { modifierGroupId: modifierGroupId, searchTerm: searchTerm, pageNumber: page, pageSize: pageSize },
            success: function (data) {
                $('#collapse2').html(data);
                totalItemsMain = parseInt($('#ModifiersContainer').attr('data-total-modifiers')) || 0;
                updatePaginationMain();
                restoreCheckboxSelectionsMain();
                updateDeleteButtonStateMain();
            },
            error: function () {
                console.error('Error loading modifiers in main view.');
                toastr.error('Error loading modifiers.', 'Error', { timeOut: 3000 });
            }
        });
    }

    // Pagination update for main view
    function updatePaginationMain() {
        var totalPages = Math.ceil(totalItemsMain / rowsPerPageMain);
        var startItem = (currentPageMain - 1) * rowsPerPageMain + 1;
        var endItem = Math.min(currentPageMain * rowsPerPageMain, totalItemsMain);
        $("#pagination-info-main").text(`Showing ${startItem}-${endItem} of ${totalItemsMain}`);
        $("#prevPageMain").toggleClass("disabled", currentPageMain === 1);
        $("#nextPageMain").toggleClass("disabled", currentPageMain >= totalPages);
    }

    // Restore checkbox selections for main view
    function restoreCheckboxSelectionsMain() {
        $('#collapse2 .main-item-checkbox').each(function () {
            var modifierId = $(this).val();
            var isSelected = selectedModifierIdsMain.includes(modifierId);
            $(this).prop('checked', isSelected);
        });
        var $checkboxes = $('#collapse2 .main-item-checkbox');
        $('#selectAllModifiersCheckbox').prop('checked', $checkboxes.length > 0 && $checkboxes.length === $checkboxes.filter(':checked').length);
    }

    // Update delete button state for main view
    function updateDeleteButtonStateMain() {
        $('#deleteModifiers').prop('disabled', selectedModifierIdsMain.length === 0);
    }

    // Pagination controls for main view
    $(document).on('click', '#prevPageMain', function (e) {
        e.preventDefault();
        if (currentPageMain > 1) {
            currentPageMain--;
            fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
        }
    });

    $(document).on('click', '#nextPageMain', function (e) {
        e.preventDefault();
        if (currentPageMain * rowsPerPageMain < totalItemsMain) {
            currentPageMain++;
            fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
        }
    });

    // Modifier group link click handler
    $(document).on('click', '.modifierGroup-link', function (e) {
        e.preventDefault();
        var $this = $(this);
        $(".modifierGroup-link").removeClass('active').find('a').removeClass('text-white').addClass('text-dark');
        $('.activePens').addClass('d-none');
        $this.addClass('active').find('a').removeClass('text-dark').addClass('text-white');
        $this.find('.activePens').removeClass('d-none');

        var modifierGroupId = $this.data('modifier-group-id');
        var modifierGroupName = $this.data('modifier-group-name');

        // Update selected state
        if ($this.hasClass('active')) {
            selectedModifierGroups = [{ id: modifierGroupId, name: modifierGroupName }]; // Single selection
            selectedCategoryMain = modifierGroupId;
            $('#toggleModifiersGroupAdd').text(modifierGroupName);
        } else {
            selectedModifierGroups = selectedModifierGroups.filter(group => group.id !== modifierGroupId);
            selectedCategoryMain = null;
            $('#toggleModifiersGroupAdd').text('Modifier Groups');
        }

        // Reset search and pagination
        $("#searchInputModifierGroup").val('');
        currentPageMain = 1;
        searchTermMain = "";
        fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
    });

    // Search input handler
    $(document).on('input', '#searchInputModifierGroup', function () {
        searchTermMain = $(this).val().trim();
        currentPageMain = 1;
        fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
    });

    // Show all modifiers (no specific group)
    $(document).on('click', '.modifierGroupPartialAll', function (e) {
        e.preventDefault();
        $(".modifierGroup-link").removeClass("active").find('a').removeClass('text-white').addClass('text-dark');
        $('.activePens').addClass('d-none');
        selectedModifierGroups = [];
        $('#toggleModifiersGroupAdd').text('Modifier Groups');
        currentPageMain = 1;
        selectedCategoryMain = null;
        fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
    });

    // Update modifier group dropdown text for add
    function updateAddModifierGroupDropdown() {
        const selectedGroupNames = $('#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]:checked').map(function () {
            return $(this).data('group-name');
        }).get();
        const displayText = selectedGroupNames.length > 0 ? selectedGroupNames.join(', ') : 'Modifier Groups';
        $('#toggleModifiersGroupAdd').text(displayText);
        // Update selectedModifierGroups based on checked checkboxes
        selectedModifierGroups = $('#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]:checked').map(function () {
            return { id: $(this).val(), name: $(this).data('group-name') };
        }).get();
    }

    // Toggle modifier group container
    $("#modifierGroupContainerAdd").hide();
    $("#toggleModifiersGroupAdd").click(function () {
        $("#modifierGroupContainerAdd").slideToggle("fast");
    });

    // Update display text when checkboxes are toggled in Add Modal
    $(document).off('change', '#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]').on('change', '#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]', function () {
        updateAddModifierGroupDropdown();
    });

    // Modal cleanup for Add Modifier
    $('#addModifiers').on('hidden.bs.modal', function () {
        console.log('Add Modifier modal hidden, resetting state');
        $('#addModifierForm')[0].reset();
        $('#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]').prop('checked', false);
        $('#toggleModifiersGroupAdd').text('Modifier Groups');
        $('#modifierGroupContainerAdd').hide();
        $('body').removeClass('modal-open').css('overflow', '');
        removeBackdrop();
    });

    // Select/Deselect all checkboxes
    $(document).on('change', '#collapse2 #selectAllModifiersCheckbox', function () {
        var isChecked = $(this).is(':checked');
        $('#collapse2 .main-item-checkbox').prop('checked', isChecked);
        var currentPageIds = $('#collapse2 .main-item-checkbox').map(function () {
            return $(this).val();
        }).get();

        if (isChecked) {
            currentPageIds.forEach(function (id) {
                if (!selectedModifierIdsMain.includes(id)) {
                    selectedModifierIdsMain.push(id);
                }
            });
        } else {
            selectedModifierIdsMain = selectedModifierIdsMain.filter(id => !currentPageIds.includes(id));
        }
        updateDeleteButtonStateMain();
    });

    // Individual checkbox selection
    $(document).on('change', '#collapse2 .main-item-checkbox', function () {
        var modifierId = $(this).val();
        if ($(this).is(':checked')) {
            if (!selectedModifierIdsMain.includes(modifierId)) {
                selectedModifierIdsMain.push(modifierId);
            }
        } else {
            selectedModifierIdsMain = selectedModifierIdsMain.filter(id => id !== modifierId);
        }
        var $checkboxes = $('#collapse2 .main-item-checkbox');
        $('#selectAllModifiersCheckbox').prop('checked', $checkboxes.length > 0 && $checkboxes.length === $checkboxes.filter(':checked').length);
        updateDeleteButtonStateMain();
    });

    // Delete button click handler
    $(document).on('click', '#deleteModifiers', function (e) {
        e.preventDefault();
        if (!$(this).prop('disabled') && selectedModifierIdsMain.length > 0) {
            $('#selectedModifierIds').val(selectedModifierIdsMain.join(','));
            $('#exampleModal4').modal('show');
        }
    });

    // Populate selected modifier IDs in the modal before showing
    $('#exampleModal4').on('show.bs.modal', function () {
        $('#selectedModifierIds').val(selectedModifierIdsMain.join(','));
    });

    // Handle multiple delete form submission
    $('#deleteMultipleModifiersForm').on('submit', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Menu/DeleteMultipleModifiers',
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                console.log('Delete Response:', response);
                if (response && response.success) {
                    $('#exampleModal4').modal('hide');
                    selectedModifierIdsMain = [];
                    fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
                    removeBackdrop();
                    toastr.success(response.message || 'Modifiers deleted successfully.', 'Success', { timeOut: 3000 });
                } else {
                    toastr.error(response?.message || 'Error deleting modifiers.', 'Error', { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error('Error deleting modifiers:', status, error);
                toastr.error('Error submitting delete request.', 'Error', { timeOut: 3000 });
            }
        });
    });

    // Delete single modifier group button handler
    $(document).on('click', '.delete-modifier-group-btn', function () {
        let modifierId = $(this).data("modifier-group-id");
        $("#deleteModifierGroupId").val(modifierId);
        openDeleteModifierGroupModal.show();
    });

    // Open Add Modifier modal and preselect modifier groups
    $(document).on('click', '#addModifiersModal', function (e) {
        e.preventDefault();
        $('#addModifiers').on('shown.bs.modal', function () {
            console.log("Add Modifier modal fully shown, updating checkboxes");
            $('#modifierGroupContainerAdd input[name="modifiersViewModel.Modifiergroupid"]').each(function () {
                var groupId = $(this).val();
                $(this).prop('checked', selectedModifierGroups.some(group => group.id == groupId));
            });
            updateAddModifierGroupDropdown();
        });
        openAddModifierModal.show();
    });

    // Handle Add Modifier form submission
    $(document).off('submit', '#addModifierForm').on('submit', '#addModifierForm', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        if (selectedModifierGroups.length == 0) {
            $(".modifierNotSelected").removeClass("d-none");
            $(".modifierNotSelected").addClass("d-block");
            return;
        } else {
            $(".modifierNotSelected").addClass("d-none");
            $(".modifierNotSelected").removeClass("d-block");
            $.ajax({
                url: '/Menu/AddModifier',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response && response.success) {
                        openAddModifierModal.hide();
                        $('#addModifierForm')[0].reset();
                        removeBackdrop();
                        toastr.success(response.message || 'Modifier added successfully.', 'Success', { timeOut: 3000 });
                        fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
                    } else {
                        toastr.error(response?.message || 'Error adding modifier.', 'Error', { timeOut: 3000 });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error adding modifier:', status, error);
                    toastr.error('Error submitting form.', 'Error', { timeOut: 3000 });
                }
            });
        }

    });

    // Delete single modifier link handler
    $(document).off('click', '.delete-modifier-link').on('click', '.delete-modifier-link', function (e) {
        e.preventDefault();
        var modifierId = $(this).data('modifier-id');
        if (!modifierId || modifierId === "0") {
            console.error("Invalid Modifier ID:", modifierId);
            return;
        }
        console.log("Modifier ID clicked:", modifierId);
        $('#deleteModifierId').val(modifierId);
        openDeleteModifierModal.show();
    });

    // Delete single modifier form submission
    $(document).on('submit', '#deleteSingleModifierForm', function (e) {
        e.preventDefault();
        var modifierId = $('#deleteModifierId').val();
        console.log("Deleting Modifier ID:", modifierId);
        if (!modifierId || modifierId === "0") {
            console.error("Invalid Modifier ID:", modifierId);
            return;
        }
        $.ajax({
            url: '/Menu/DeleteModifier',
            type: 'POST',
            data: { modifierid: modifierId },
            success: function (response) {
                if (response && response.success) {
                    openDeleteModifierModal.hide();
                    $('#deleteModifierId').val('');
                    toastr.success(response.message || 'Modifier deleted successfully.', 'Success', { timeOut: 3000 });
                    fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
                    removeBackdrop();
                } else {
                    toastr.error(response?.message || 'Error deleting modifier.', 'Error', { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error('Error deleting modifier:', status, error);
                toastr.error('Error submitting delete request.', 'Error', { timeOut: 3000 });
            }
        });
    });

    // Edit modifier submit
    $(document).off('submit', '#editModifierForm').on('submit', '#editModifierForm', function (e) {
        e.preventDefault();
        var form = $(this);
        var formData = new FormData(this);

        // Log selected modifier groups
        var selectedGroups = $('#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]:checked').map(function () {
            return $(this).val();
        }).get();
        console.log("Selected Modifier Groups:", selectedGroups);

        if (selectedGroups.length == 0) {
            $(".modifierNotSelectedAtEdit").removeClass("d-none");
            $(".modifierNotSelectedAtEdit").addClass("d-block");
            return;
        } else {
            $(".modifierNotSelectedAtEdit").addClass("d-none");
            $(".modifierNotSelectedAtEdit").removeClass("d-block");

            $.ajax({
                url: '/Menu/EditModifier',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    console.log("Edit modifier response:", response);
                    if (response.success) {
                        openUpdateModifierModal.hide();
                        $("#modifierGroupContainerEdit").hide();
                        $('#editModifierForm')[0].reset();
                        $('#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]').prop('checked', false);
                        removeBackdrop();
                        toastr.success(response.message || "Modifier edited successfully", "Success", { timeOut: 3000 });
                        fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
                    } else {
                        toastr.error(response.message || "Failed to edit modifier", "Error", { timeOut: 3000 });
                    }
                },
                error: function (xhr, status, error) {
                    console.error("AJAX error - Status:", status, "Error:", error);
                    toastr.error("Error submitting edit form: " + status, "Error", { timeOut: 3000 });
                }
            });
        }
    });

    // Ensure modifier group container is hidden initially
    $("#modifierGroupContainerEdit").hide();

    // Toggle modifier group container
    $(document).off('click', '#toggleModifiersGroupEdit').on('click', '#toggleModifiersGroupEdit', function (e) {
        e.preventDefault();
        console.log("Toggling #modifierGroupContainerEdit");
        const $container = $("#modifierGroupContainerEdit");
        if ($container.length) {
            $container.slideToggle("fast", function () {
                console.log("Container visibility:", $container.is(":visible") ? "visible" : "hidden");
            });
        } else {
            console.error("Container #modifierGroupContainerEdit not found in DOM");
        }
    });

    // Ensure container is hidden when modal opens
    $('#editModal-Modifier').on('show.bs.modal', function () {
        // console.log("Edit Modifier modal opening, hiding #modifierGroupContainerEdit");
        $("#modifierGroupContainerEdit").hide();
    });

    // Modal cleanup
    $('#editModal-Modifier').on('hidden.bs.modal', function () {
        $('body').removeClass('modal-open').css('overflow', '');
    });

    function updateEditModifierGroupDropdown() {
        const selectedGroupNames = $('#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]:checked').map(function () {
            return $(this).data('group-name');
        }).get();
        $('#selectedModifierGroupsText').text(
            selectedGroupNames.length > 0 ? selectedGroupNames.join(', ') : 'Modifier Groups'
        );
    }

    // Edit modifier link handler
    $(document).off('click', '.edit-modifier-link').on('click', '.edit-modifier-link', function (e) {
        e.preventDefault();
        var modifierId = $(this).data('modifier-id');
        if (!modifierId || modifierId <= 0) {
            console.error("Invalid Modifier ID:", modifierId);
            return;
        }
        console.log("Fetching modifier ID for edit:", modifierId);

        // Reset form and checkboxes
        $('#editModifierForm')[0].reset();
        $('#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]').prop('checked', false);
        $('#selectedModifierGroupsText').text('Modifier Groups');

        // Fetch modifier data
        $.ajax({
            url: '/Menu/GetModifierData',
            type: 'GET',
            data: { modifierId: modifierId },
            dataType: 'json',
            success: function (data) {
                console.log("Modifier data received:", data);
                if (data && data.modifierid) {
                    // Populate form fields
                    $('#editModifierId').val(data.modifierid);
                    $('#editModifierName').val(data.modifiername);
                    $('#editModifierRate').val(data.modifierrate);
                    $('#editModifierQuantity').val(data.modifierquantity);
                    $('#editModifierUnit').val(data.modifierunit);
                    $('#editModifierDescription').val(data.modifierdescription);

                    // Store modifier group IDs
                    var modifierGroupIds = data.modifiergroupid && Array.isArray(data.modifiergroupid) ? data.modifiergroupid : [];
                    console.log("Modifier group IDs to check:", modifierGroupIds);

                    // Open modal
                    openUpdateModifierModal.show();

                    // Handle checkbox selection and display after modal is shown
                    $('#editModal-Modifier').off('shown.bs.modal').on('shown.bs.modal', function () {
                        console.log("Edit Modifier modal shown");

                        // Log available checkboxes
                        const availableCheckboxes = $('#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]').map(function () {
                            return $(this).attr('id');
                        }).get();
                        console.log("Available checkbox IDs:", availableCheckboxes);

                        // Collect selected group names
                        let selectedGroupNames = [];

                        // Check checkboxes and collect names
                        modifierGroupIds.forEach(function (groupId) {
                            const selector = '#modifiergroup_' + groupId;
                            const $checkbox = $(selector);
                            console.log(`Checking checkbox: ${selector}, Exists: ${$checkbox.length > 0}`);
                            if ($checkbox.length > 0) {
                                $checkbox.prop('checked', true);
                                const groupName = $checkbox.data('group-name');
                                if (groupName) {
                                    selectedGroupNames.push(groupName);
                                }
                                console.log(`Checkbox ${selector} checked`);
                            } else {
                                console.warn(`Checkbox ${selector} not found in DOM. Fetching group data...`);
                                // Dynamically add missing checkbox
                                $.ajax({
                                    url: '/Menu/GetModifierGroupById',
                                    type: 'GET',
                                    data: { modifierGroupId: groupId },
                                    success: function (groupData) {
                                        if (groupData && groupData.modifiergroupid && groupData.modifiergroupname) {
                                            console.log(`Adding checkbox for group: ${groupData.modifiergroupid} - ${groupData.modifiergroupname}`);
                                            $("#modifierGroupContainerEdit").append(`
                                                <div class="form-check">
                                                    <input type="checkbox" class="form-check-input"
                                                           name="modifiersViewModel.Modifiergroupid"
                                                           value="${groupData.modifiergroupid}"
                                                           id="modifiergroup_${groupData.modifiergroupid}"
                                                           data-group-name="${groupData.modifiergroupname}"
                                                           checked />
                                                    <label class="form-check-label" for="modifiergroup_${groupData.modifiergroupid}">
                                                        ${groupData.modifiergroupname}
                                                    </label>
                                                </div>
                                            `);
                                            selectedGroupNames.push(groupData.modifiergroupname);
                                            // Update display text after adding
                                            updateEditModifierGroupDropdown();
                                        }
                                    },
                                    error: function (xhr, status, error) {
                                        console.error(`Error fetching modifier group ${groupId}:`, status, error);
                                    }
                                });
                            }
                        });

                        // Update display text with selected group names
                        updateEditModifierGroupDropdown();
                    });
                } else {
                    console.error("Invalid modifier data received:", data);
                    toastr.error("Failed to load modifier details.", "Error", { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error - Status:", status, "Error:", error);
                toastr.error("Error loading modifier details: " + status, "Error", { timeOut: 3000 });
            }
        });
    });

    // Update display text when checkboxes are toggled in Edit Modal
    $(document).off('change', '#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]').on('change', '#modifierGroupContainerEdit input[name="modifiersViewModel.Modifiergroupid"]', function () {
        updateEditModifierGroupDropdown();
    });

    // Delete single modifier group form submission
    $(document).on('submit', '#DeleteModifierGroupForm', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            url: '/Menu/DeleteModifierGroup',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response && response.success) {
                    openDeleteModifierGroupModal.hide();
                    removeBackdrop();
                    selectedCategoryMain = null;
                    fetchModifierGroups();
                    toastr.success(response.message || 'Modifier group deleted successfully.', 'Success', { timeOut: 3000 });
                } else {
                    toastr.error(response?.message || 'Error deleting modifier group.', 'Error', { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error('Error deleting modifier group:', status, error);
                toastr.error('Error submitting delete request.', 'Error', { timeOut: 3000 });
            }
        });
    });

    // Add Modifier Group Modal Handling
    toastr.options.closeButton = true;
    var rowsPerPageAdd = 5;
    var currentPageAdd = 1;
    var totalItemsAdd = 0;
    var selectedCategoryAdd = null;
    var searchTermAdd = '';
    var selectedModifierIdsAdd = [];

    // Initial state
    $("#addModifierGroupForm").show();
    $(".addExistingModifiersAdd").hide();

    // Toggle custom dropdown visibility
    $('#itemsPerPageBtnAdd').on('click', function () {
        var $menu = $('#itemsPerPageMenuAdd');
        $menu.toggle();
    });

    // Close dropdown when clicking outside
    $(document).on('click', function (e) {
        var $dropdown = $('.custom-dropdown');
        if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
            $('#itemsPerPageMenuAdd').hide();
        }
    });

    // Handle page size selection
    $('#exampleModal1').on('click', '.page-size-option', function (e) {
        e.preventDefault();
        var newSize = parseInt($(this).data('size'));
        if (newSize !== rowsPerPageAdd) {
            rowsPerPageAdd = newSize;
            $('#itemsPerPageBtnAdd').html(`${rowsPerPageAdd} <span><i class="bi bi-chevron-down"></i></span>`);
            currentPageAdd = 1;
            fetchModifiersAdd(null, searchTermAdd, currentPageAdd, rowsPerPageAdd);
        }
        $('#itemsPerPageMenuAdd').hide();
    });

    // Fetch modifiers via AJAX
    function fetchModifiersAdd(modifierGroupId = null, searchTerm = '', page, pageSize) {
        $.ajax({
            url: '/Menu/FilterModifiersAtAddCategory',
            type: 'GET',
            data: { searchTerm: searchTerm, pageNumber: page, pageSize: pageSize },
            success: function (data) {
                $('#collapseAdd').html(data);
                totalItemsAdd = parseInt($('#ModifiersContainer2').attr('data-total-modifiers')) || 0;
                updatePaginationAdd();
                restoreCheckboxSelectionsAdd();
            },
            error: function () {
                alert('Error loading modifiers.');
            }
        });
    }

    // Update pagination UI
    function updatePaginationAdd() {
        var totalPages = Math.ceil(totalItemsAdd / rowsPerPageAdd);
        var startItem = (currentPageAdd - 1) * rowsPerPageAdd + 1;
        var endItem = Math.min(currentPageAdd * rowsPerPageAdd, totalItemsAdd);
        $("#pagination-info-add").text(`Showing ${startItem}-${endItem} of ${totalItemsAdd}`);
        $("#prevPageAdd").toggleClass("disabled", currentPageAdd === 1);
        $("#nextPageAdd").toggleClass("disabled", currentPageAdd >= totalPages);
    }

    // Restore checkbox states
    function restoreCheckboxSelectionsAdd() {
        $("#collapseAdd .item-checkbox").each(function () {
            var modifierId = $(this).val();
            var isSelected = selectedModifierIdsAdd.some((m) => m.id === modifierId);
            $(this).prop("checked", isSelected);
        });
    }

    // Pagination controls
    $('#exampleModal1').on('click', '#prevPageAdd', function (e) {
        e.preventDefault();
        if (currentPageAdd > 1) {
            currentPageAdd--;
            fetchModifiersAdd(null, searchTermAdd, currentPageAdd, rowsPerPageAdd);
        }
    });

    $('#exampleModal1').on('click', '#nextPageAdd', function (e) {
        e.preventDefault();
        if (currentPageAdd * rowsPerPageAdd < totalItemsAdd) {
            currentPageAdd++;
            fetchModifiersAdd(null, searchTermAdd, currentPageAdd, rowsPerPageAdd);
        }
    });

    $('#exampleModal1').on('input', '#searchInputAdd', function () {
        searchTermAdd = $(this).val().trim();
        currentPageAdd = 1;
        fetchModifiersAdd(null, searchTermAdd, currentPageAdd, rowsPerPageAdd);
    });

    // Show/hide modifier section
    $('#exampleModal1').on('click', '.link-to-modifiers-add', function (e) {
        e.preventDefault();
        $("#addModifierGroupForm").hide();
        $(".addExistingModifiersAdd").show();
        fetchModifiersAdd(null, searchTermAdd, currentPageAdd, rowsPerPageAdd);
    });

    $('#exampleModal1').on('click', '.close-modifiers-add', function (e) {
        e.preventDefault();
        $(".addExistingModifiersAdd").hide();
        $("#addModifierGroupForm").show();
        restoreCheckboxSelectionsAdd();
    });

    // Checkbox change handler
    $('#exampleModal1').on('change', '#collapseAdd .item-checkbox', function () {
        var modifierId = $(this).val();
        var modifierName = $(this).data('name') || `Modifier ${modifierId}`;
        var isChecked = $(this).is(':checked');
        if (isChecked && !selectedModifierIdsAdd.some((m) => m.id === modifierId)) {
            selectedModifierIdsAdd.push({ id: modifierId, name: modifierName });
        } else if (!isChecked) {
            selectedModifierIdsAdd = selectedModifierIdsAdd.filter((m) => m.id !== modifierId);
        }
    });

    // Reset on modal close
    $('#exampleModal1').on('hidden.bs.modal', function () {
        $(".addExistingModifiersAdd").hide();
        $("#addModifierGroupForm").show();
        $('#collapseAdd .item-checkbox').prop('checked', false);
        selectedModifierIdsAdd = [];
        $('#selectedIdsAdd').val('');
        $('#selectedIdsAddHidden').val('');
        $('#selectedModifiersContainerAdd .d-flex').empty();
    });

    // Handle modifier form submission
    $('#modifierFormAdd').on('submit', function (e) {
        e.preventDefault();
        $('#collapseAdd .item-checkbox:checked').each(function () {
            var id = $(this).val();
            var name = $(this).data('name') || `Modifier ${id}`;
            if (!selectedModifierIdsAdd.some((m) => m.id === id)) {
                selectedModifierIdsAdd.push({ id: id, name: name });
            }
        });
        $('#selectedIdsAddHidden').val(selectedModifierIdsAdd.map((m) => m.id).join(','));
        $('#selectedIdsAdd').val(selectedModifierIdsAdd.map((m) => m.id).join(','));
        $.ajax({
            url: '/Menu/AddModifierGroupDetails',
            type: 'POST',
            data: $(this).serialize(),
            dataType: 'json',
            success: function (response) {
                if (response && response.success && response.modifiers) {
                    response.modifiers.forEach(function (modifier) {
                        if ($(`#selectedModifiersContainerAdd div[data-id="${modifier.modifierid}"]`).length === 0) {
                            $('#selectedModifiersContainerAdd .d-flex').append(
                                `<div class="border border-2 px-2 text-primary rounded-pill border-primary me-2 mb-2" data-id="${modifier.modifierid}">
                                    ${modifier.modifiername} <span class="text-dark remove-modifier-add" style="cursor: pointer;" data-id="${modifier.modifierid}">x</span>
                                </div>`
                            );
                        }
                    });
                    $(".addExistingModifiersAdd").hide();
                    $("#addModifierGroupForm").show();
                    $('#collapseAdd .item-checkbox').prop('checked', false);
                } else {
                    alert(response?.message || 'Error adding modifiers.');
                }
            },
            error: function () {
                alert('Error submitting form.');
            }
        });
    });

    // Remove modifier
    $('#exampleModal1').on('click', '.remove-modifier-add', function () {
        var id = $(this).data('id').toString();
        selectedModifierIdsAdd = selectedModifierIdsAdd.filter((m) => m.id !== id);
        $(this).parent().remove();
        $('#selectedIdsAdd').val(selectedModifierIdsAdd.map((m) => m.id).join(','));
        $('#selectedIdsAddHidden').val(selectedModifierIdsAdd.map((m) => m.id).join(','));
    });

    // Open Add Modifier Group modal
    $(document).on('click', "#AddmodifierGroupModal", function (e) {
        e.preventDefault();
        $('#collapseAdd .item-checkbox').prop('checked', false);
        selectedModifierIdsAdd = [];
        $('#selectedIdsAdd').val('');
        $('#selectedIdsAddHidden').val('');
        $('#selectedModifiersContainerAdd .d-flex').empty();
        openAddModifierGroupModal.show();
    });

    // Add Modifier Group form submission
    $("#AddModifierGroupFormSubmit").on('submit', function (e) {
        e.preventDefault();
        var form = $(this);

        console.log("Form valid:", form.valid()); // Debug validation
        console.log("Form data:", form.serialize()); // Debug serialized data
        if (form.valid()) {
            var formData = $(this).serialize();
            $.ajax({
                url: '/Menu/AddModifierGroup',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response && response.success) {
                        $('#AddModifierGroupFormSubmit')[0].reset();
                        $('#collapseAdd .item-checkbox').prop('checked', false);
                        selectedModifierIdsAdd = [];
                        toastr.success(response.message || 'Modifier group added successfully.', 'Success', { timeOut: 3000 });
                        fetchModifierGroups();
                        openAddModifierGroupModal.hide();
                        removeBackdrop();
                    } else {
                        toastr.error(response?.message || 'Error adding modifier group.', 'Error', { timeOut: 3000 });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error adding modifier group:', status, error);
                    toastr.error('Error submitting form.', 'Error', { timeOut: 3000 });
                }
            });
        }
    });

    // Edit Modifier Group Modal Handling
    var rowsPerPageEdit = 5;
    var currentPageEdit = 1;
    var totalItemsEdit = 0;
    var existingModifierIdsEdit = [];
    var pendingModifierIdsEdit = [];
    var searchTermEdit = "";
    var currentModifierGroupIdEdit = null;

    // Toggle custom dropdown visibility
    $("#itemsPerPageBtnEdit").on("click", function () {
        var $menu = $("#itemsPerPageMenuEdit");
        $menu.toggle();
    });

    // Close dropdown when clicking outside
    $(document).on("click", function (e) {
        var $dropdown = $(".custom-dropdown");
        if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
            $("#itemsPerPageMenuEdit").hide();
        }
    });

    // Handle page size selection
    $("#editModal").on("click", ".page-size-option", function (e) {
        e.preventDefault();
        var newSize = parseInt($(this).data("size"));
        if (newSize !== rowsPerPageEdit) {
            rowsPerPageEdit = newSize;
            $("#itemsPerPageBtnEdit").html(
                `${rowsPerPageEdit} <span><i class="bi bi-chevron-down"></i></span>`
            );
            currentPageEdit = 1;
            fetchModifiersEdit(
                null,
                searchTermEdit,
                currentPageEdit,
                rowsPerPageEdit
            );
        }
        $("#itemsPerPageMenuEdit").hide();
    });

    // Update UI with existing modifiers
    function updateSelectedModifiersUIEdit() {
        const $container = $("#selectedModifiersContainerEdit .d-flex");
        $container.empty();
        existingModifierIdsEdit.forEach(function (modifier) {
            $container.append(
                `<div class="border border-2 px-2 text-primary rounded-pill border-primary me-2 mb-2" data-id="${modifier.id}">
                    ${modifier.name} <span class="text-dark remove-modifier-edit" style="cursor: pointer;" data-id="${modifier.id}">x</span>
                </div>`
            );
        });
        $("#selectedIdsEdit").val(
            existingModifierIdsEdit.map((m) => m.id).join(",")
        );
        $("#selectedIdsEditHidden").val(
            existingModifierIdsEdit.map((m) => m.id).join(",")
        );
    }

    // Load modifier group data
    function loadModifierGroupEdit(modifierGroupId) {
        $.ajax({
            url: "/Menu/GetModifierGroup",
            type: "GET",
            data: { id: modifierGroupId },
            success: function (data) {
                $("#editModal #floatingInputEdit").val(data.modifiergroupname);
                $("#editModal #floatingTextareaEdit").val(
                    data.modifiergroupdescription
                );
                $("#editModal #modifierGroupIdEdit").val(data.modifiergroupid);
                existingModifierIdsEdit = data.selectedModifiers
                    ? data.selectedModifiers.map((m) => ({
                        id: m.modifierId.toString(),
                        name: m.modifierName,
                    }))
                    : [];
                pendingModifierIdsEdit = [];
                currentModifierGroupIdEdit = modifierGroupId;
                updateSelectedModifiersUIEdit();
                $("#editModal").modal("show");
                $("#editModifierGroupForm").show();
                $(".addExistingModifiersEdit").hide();
                fetchModifiersEdit(
                    null,
                    searchTermEdit,
                    currentPageEdit,
                    rowsPerPageEdit
                );
            },
            error: function () {
                alert("Error loading modifier group data.");
            },
        });
    }

    // Fetch modifiers for the edit modal
    function fetchModifiersEdit(
        modifierGroupId = null,
        searchTerm = "",
        page,
        pageSize
    ) {
        $.ajax({
            url: "/Menu/FilterModifiersAtEditModifierGroup",
            type: "GET",
            data: { searchTerm: searchTerm, pageNumber: page, pageSize: pageSize },
            success: function (data) {
                $("#collapseEdit").html(data);
                totalItemsEdit =
                    parseInt($("#ModifiersContainer4").attr("data-total-modifiers")) || 0;
                updatePaginationEdit();
                restoreCheckboxSelectionsEdit();
            },
            error: function () {
                alert("Error loading modifiers.");
            },
        });
    }

    // Update pagination
    function updatePaginationEdit() {
        var totalPagesEdit = Math.ceil(totalItemsEdit / rowsPerPageEdit);
        var startItemEdit = (currentPageEdit - 1) * rowsPerPageEdit + 1;
        var endItemEdit = Math.min(
            currentPageEdit * rowsPerPageEdit,
            totalItemsEdit
        );
        $("#pagination-info-edit").text(
            `Showing ${startItemEdit}-${endItemEdit} of ${totalItemsEdit}`
        );
        $("#prevPageEdit").toggleClass("disabled", currentPageEdit === 1);
        $("#nextPageEdit").toggleClass(
            "disabled",
            currentPageEdit >= totalPagesEdit
        );
    }

    // Restore checkbox states
    function restoreCheckboxSelectionsEdit() {
        $("#collapseEdit .item-checkbox").each(function () {
            var modifierId = $(this).val();
            var isSelected =
                existingModifierIdsEdit.some((m) => m.id === modifierId) ||
                pendingModifierIdsEdit.some((m) => m.id === modifierId);
            $(this).prop("checked", isSelected);
        });
    }

    // Pagination controls
    $("#editModal").on("click", "#prevPageEdit", function (e) {
        e.preventDefault();
        if (currentPageEdit > 1) {
            currentPageEdit--;
            fetchModifiersEdit(
                null,
                searchTermEdit,
                currentPageEdit,
                rowsPerPageEdit
            );
        }
    });

    $("#editModal").on("click", "#nextPageEdit", function (e) {
        e.preventDefault();
        if (currentPageEdit * rowsPerPageEdit < totalItemsEdit) {
            currentPageEdit++;
            fetchModifiersEdit(
                null,
                searchTermEdit,
                currentPageEdit,
                rowsPerPageEdit
            );
        }
    });

    $("#editModal").on("input", "#searchInputEdit", function () {
        searchTermEdit = $(this).val().trim();
        currentPageEdit = 1;
        fetchModifiersEdit(
            null,
            searchTermEdit,
            currentPageEdit,
            rowsPerPageEdit
        );
    });

    // Checkbox change handler
    $("#editModal").on("change", ".item-checkbox", function () {
        var modifierId = $(this).val();
        var modifierName = $(this).data("name") || `Modifier ${modifierId}`;
        var isChecked = $(this).is(":checked");
        if (
            isChecked &&
            !existingModifierIdsEdit.some((m) => m.id === modifierId) &&
            !pendingModifierIdsEdit.some((m) => m.id === modifierId)
        ) {
            pendingModifierIdsEdit.push({ id: modifierId, name: modifierName });
        } else if (!isChecked) {
            pendingModifierIdsEdit = pendingModifierIdsEdit.filter(
                (m) => m.id !== modifierId
            );
        }
    });

    // Handle modifier form submission
    $("#modifierFormEdit").on("submit", function (e) {
        e.preventDefault();
        pendingModifierIdsEdit.forEach(function (modifier) {
            if (!existingModifierIdsEdit.some((m) => m.id === modifier.id)) {
                existingModifierIdsEdit.push({ id: modifier.id, name: modifier.name });
            }
        });
        pendingModifierIdsEdit = [];
        updateSelectedModifiersUIEdit();
        $(".addExistingModifiersEdit").hide();
        $("#editModifierGroupForm").show();
    });

    // Handle main form submission
    $("#editModifierGroupForm form").on("submit", function (e) {
        $("#selectedIdsEdit").val(
            existingModifierIdsEdit.map((m) => m.id).join(",")
        );
    });

    // Remove modifier
    $("#editModal").on("click", ".remove-modifier-edit", function () {
        var modifierId = $(this).data("id").toString();
        existingModifierIdsEdit = existingModifierIdsEdit.filter(
            (m) => m.id !== modifierId
        );
        pendingModifierIdsEdit = pendingModifierIdsEdit.filter(
            (m) => m.id !== modifierId
        );
        updateSelectedModifiersUIEdit();
        restoreCheckboxSelectionsEdit();
    });

    // Show/hide modifier section
    $("#editModal").on("click", ".link-to-modifiers-edit", function (e) {
        e.preventDefault();
        $("#editModifierGroupForm").hide();
        $(".addExistingModifiersEdit").show();
        fetchModifiersEdit(
            null,
            searchTermEdit,
            currentPageEdit,
            rowsPerPageEdit
        );
    });

    $("#editModal").on("click", ".close-modifiers-edit", function (e) {
        e.preventDefault();
        $(".addExistingModifiersEdit").hide();
        $("#editModifierGroupForm").show();
        pendingModifierIdsEdit = [];
        restoreCheckboxSelectionsEdit();
    });

    // Load modifier group on edit click
    $(document).on("click", ".edit-modifier-group", function (e) {
        e.preventDefault();
        var modifierGroupId = $(this).data("modifier-group-id");
        loadModifierGroupEdit(modifierGroupId);
    });

    // Reset on modal close
    $("#editModal").on("hidden.bs.modal", function () {
        $(".addExistingModifiersEdit").hide();
        $("#editModifierGroupForm").show();
        existingModifierIdsEdit = [];
        pendingModifierIdsEdit = [];
        $("#selectedIdsEdit").val("");
        $("#selectedIdsEditHidden").val("");
        $("#selectedModifiersContainerEdit .d-flex").empty();
    });

    // Edit Modifier Group form submission
    $(document).on('submit', '#EditModifierGroupFormSubmit', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.ajax({
            url: '/Menu/EditModifierGroup',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response && response.success) {
                    $('#EditModifierGroupFormSubmit')[0].reset();
                    $('#collapseEdit .item-checkbox').prop('checked', false);
                    existingModifierIdsEdit = [];
                    pendingModifierIdsEdit = [];
                    toastr.success(response.message || 'Modifier group edited successfully.', 'Success', { timeOut: 3000 });
                    openUpdateModifierGroupModal.hide();
                    removeBackdrop();
                    fetchModifierGroups();
                } else {
                    toastr.error(response?.message || 'Error editing modifier group.', 'Error', { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error('Error editing modifier group:', status, error);
                toastr.error('Error submitting form.', 'Error', { timeOut: 3000 });
            }
        });
    });

    // Backdrop remove helper
    function removeBackdrop() {
        setTimeout(() => {
            document.body.classList.remove("modal-open");
            $(".modal-backdrop").remove();
            document.body.style.overflow = "auto";
        }, 300);
    }

    // Initial fetch
    fetchModifiersMain(selectedCategoryMain, searchTermMain, currentPageMain, rowsPerPageMain);
});