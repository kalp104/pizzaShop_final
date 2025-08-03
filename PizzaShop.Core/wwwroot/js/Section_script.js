
        toastr.options.closeButton = true;
        $(document).ready(function () {
            var rowsPerPage = 5;
            var currentPage = 1;
            var totalTables = window.TotalTables;
            var canEdit = window.canEdit;
            var canDelete = window.canDelete;
            var selectedSection = null;
            var selectedSectionName = '';
            var searchTerm = '';
            var selectedTableIds = new Set();

            // Initialize modals once
            var addSectionmodal = new bootstrap.Modal('#SectionAddModal', { backdrop: 'static', keyboard: false });
            var addTableModal = new bootstrap.Modal('#TableAddModal', { backdrop: 'static', keyboard: false });
            var editTableModal = new bootstrap.Modal('#editTableModal', { backdrop: 'static', keyboard: false });
            var deleteTableModal = new bootstrap.Modal('#deleteTableModal', { backdrop: 'static', keyboard: false });
            
            // Fetch tables via AJAX
            function fetchTables(sectionId, searchTerm = '', page, pageSize) {
                $.ajax({
                    url: '/Section/FilterTables',
                    type: 'GET',
                    data: { sectionId: sectionId, searchTable: searchTerm, pageNumber: page, pageSize: pageSize },
                    success: function (data) {
                        $('#collapse5').html(data);
                        totalTables = parseInt($('#TableContainer').attr('data-total-tables')) || 0;
                        restoreCheckboxStates();
                        updatePagination();
                    },
                    error: function () {
                        toastr.error('Error loading tables.', 'Error', { timeOut: 3000 });
                    }
                });
            }

            // Restore checkbox states after table refresh
            function restoreCheckboxStates() {
                $('.Table-checkbox').each(function () {
                    var tableId = $(this).data('table-id');
                    $(this).prop('checked', selectedTableIds.has(tableId));
                });
                updateSelectAllCheckbox();
            }

            // Update pagination info and buttons
            function updatePagination() {
                var totalPages = Math.ceil(totalTables / rowsPerPage);
                var startItem = (currentPage - 1) * rowsPerPage + 1;
                var endItem = Math.min(currentPage * rowsPerPage, totalTables);
                $("#pagination-info").text(`Showing ${startItem}-${endItem} of ${totalTables}`);
                $("#prevPage4").toggleClass("disabled", currentPage === 1);
                $("#nextPage4").toggleClass("disabled", currentPage >= totalPages);
                $('#deleteMultipleTablesBtn').prop('disabled', selectedTableIds.size === 0);
            }

            // Update select all checkbox state
            function updateSelectAllCheckbox() {
                var allChecked = $('.Table-checkbox:visible').length > 0 &&
                    $('.Table-checkbox:visible').length === $('.Table-checkbox:visible:checked').length;
                $('#selectAllCheckBoxTable').prop('checked', allChecked);
            }

            // Fetch sections via AJAX
            function fetchSections() {
                $.ajax({
                    url: '/Section/GetSections',
                    type: 'GET',
                    success: function (data) {
                        updateSectionList(data.sections);
                        fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                    },
                    error: function () {
                        toastr.error('Error loading sections.', 'Error', { timeOut: 3000 });
                    }
                });
            }
            
            // initial fatch data of sections
            fetchSections();


            // open add table modal
            $(document).on('click', '#TableAddModalID', function()
            {
                addTableModal.show();
            });
            //open add section modal
            $(document).on('click', '#AddSectionBtn', function()
            {
                addSectionmodal.show();
            });

            // Update section list dynamically
            function updateSectionList(sections) {
                var $sectionList = $('#section-list');
                var $noSections = $('#no-sections');
                $sectionList.empty();
                if (sections.length === 0) {
                    $noSections.show();
                    return;
                }
                $noSections.hide();

                if(!selectedSection && sections.length > 0)
                {
                    selectedSection = sections[0].sectionid;
                    selectedSectionName = sections[0].sectionname;
                }

                sections.forEach(function (s) {
                    var isActive = s.sectionid === selectedSection ? 'active' : '';
                    var textColor = s.sectionid === selectedSection ? 'text-white' : 'text-dark';
                    var pensVisible = s.sectionid === selectedSection ? '' : 'd-none';
                    var activePensHtml = '';
                    if (canEdit) {
                        activePensHtml += `
                            <a href="#" class="text-primary edit-section-button" data-sectionid="${s.sectionid}"
                               data-sectionname="${s.sectionname}" data-sectiondescription="${s.sectiondescription}">
                                <i class="bi bi-pen mx-1"></i>
                            </a>`;
                    }
                    if (canDelete) {
                        activePensHtml += `
                            <a href="#" class="text-primary delete-section-btn" data-sectionid="${s.sectionid}">
                                <i class="bi bi-trash"></i>
                            </a>`;
                    }
                    activePensHtml = `<div class="activePens ${pensVisible}">${activePensHtml}</div>`;
                    var sectionHtml = `
                        <li class="nav-link links section-link d-flex justify-content-between align-items-center gap-2 ${isActive}"
                            id="v-pills-${s.sectionid}-tab-section" data-sectionid="${s.sectionid}"
                            data-sectionname="${s.sectionname}" data-sectiondescription="${s.sectiondescription}">
                            <a class="text-decoration-none ${textColor}">
                                <i class="bi bi-grid-3x2-gap-fill me-2"></i>${s.sectionname}
                            </a>
                            ${activePensHtml}
                        </li>`;
                    $sectionList.append(sectionHtml);
                });
            }

            // Pagination: Previous page
            $(document).on('click', '#prevPage4', function (e) {
                e.preventDefault();
                if (currentPage > 1) {
                    currentPage--;
                    fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                }
            });

            // Pagination: Next page
            $(document).on('click', '#nextPage4', function (e) {
                e.preventDefault();
                if (currentPage * rowsPerPage < totalTables) {
                    currentPage++;
                    fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                }
            });

            // Pagination: Change page size
            $(document).on('click', '.page-size-option4', function (e) {
                e.preventDefault();
                rowsPerPage = parseInt($(this).data("size"));
                $("#itemsPerPageBtn4").text(rowsPerPage);
                currentPage = 1;
                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
            });

            // Section link click
            $(document).on('click', '.section-link', function (e) {
                e.preventDefault();
                $('.section-link').removeClass('active').find('a').removeClass('text-white').addClass('text-dark');
                $('.activePens').addClass('d-none');
                $(this).addClass('active').find('a').removeClass('text-dark').addClass('text-white');
                $(this).find('.activePens').removeClass('d-none');
                selectedSection = $(this).data('sectionid');
                selectedSectionName = $(this).data('sectionname');
                $('#sectionIdAtAddTable').val(selectedSection);
                $('#sectionNameAtAddTable').val(selectedSectionName);
                currentPage = 1;
                selectedTableIds.clear();
                $('#searchInput').val("");
                searchTerm = '';
                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                $('#TableAddModalID').prop('disabled', false);
            });

            // Search input
            $(document).on('input', '#searchInput', function () {
                searchTerm = $(this).val().trim();
                currentPage = 1;
                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
            });

            // Select all checkboxes
            $(document).on('change', '#selectAllCheckBoxTable', function () {
                var isChecked = this.checked;
                $('.Table-checkbox:visible').each(function () {
                    var tableId = $(this).data('table-id');
                    $(this).prop('checked', isChecked);
                    if (isChecked) selectedTableIds.add(tableId);
                    else selectedTableIds.delete(tableId);
                });
                updatePagination();
                updateSelectAllCheckbox();
            });

            // Individual table checkbox
            $(document).on('change', '.Table-checkbox', function () {
                var tableId = $(this).data('table-id');
                if ($(this).is(':checked')) selectedTableIds.add(tableId);
                else selectedTableIds.delete(tableId);
                updatePagination();
                updateSelectAllCheckbox();
            });

            // Delete multiple tables button
            $(document).on('click', '#deleteMultipleTablesBtn', function (e) {
                e.preventDefault();
                if (selectedTableIds.size > 0) {
                    $('#deleteSelectedTableIds').val(Array.from(selectedTableIds).join(','));
                    var deleteModal = new bootstrap.Modal(document.getElementById('deleteTablesModal'), { backdrop: 'static' });
                    deleteModal.show();
                }
            });

            // Add section form submission
            $('#addSectionForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                if ($form.valid()) {
                    $.ajax({
                        url: $form.attr('action'),
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (response) {
                            if (response.success) {
                                toastr.success(response.message, 'Success', { timeOut: 3000 });
                                fetchSections();
                                $form[0].reset();
                               
                                addSectionmodal.hide();
                                removeBackdrop();
                            } else {
                                toastr.error(response.message, 'Error', { timeOut: 3000 });
                            }
                        },
                        error: function () {
                            toastr.error('Error adding section.', 'Error', { timeOut: 3000 });
                        }
                    });
                }
            });

            // Edit section form submission
            $('#editSectionForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                if ($form.valid()) {
                    $.ajax({
                        url: $form.attr('action'),
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (response) {
                            if (response.success) {
                                toastr.success(response.message, 'Success', { timeOut: 3000 });
                                fetchSections();
                                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                                var modal = bootstrap.Modal.getInstance(document.getElementById('editSectionModal'));
                                modal.hide();
                                removeBackdrop();
                            } else {
                                toastr.error(response.message, 'Error', { timeOut: 3000 });
                            }
                        },
                        error: function () {
                            toastr.error('Error editing section.', 'Error', { timeOut: 3000 });
                        }
                    });
                }
            });

            // Delete section form submission
            $('#deleteSectionForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                $.ajax({
                    url: $form.attr('action'),
                    type: 'POST',
                    data: $form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            toastr.success(response.message, 'Success', { timeOut: 3000 });
                            selectedSection = null;
                            selectedSectionName = '';
                            $('#sectionIdAtAddTable').val('');
                            $('#sectionNameAtAddTable').val('');
                            // $('#TableAddModalID').prop('disabled', true);
                            fetchSections();
                            fetchTables(null, searchTerm, currentPage, rowsPerPage);
                            var modal = bootstrap.Modal.getInstance(document.getElementById('deleteSectionModal'));
                            modal.hide();
                            removeBackdrop();
                        } else {
                            toastr.error(response.message, 'Error', { timeOut: 3000 });
                        }
                    },
                    error: function () {
                        toastr.error('Error deleting section.', 'Error', { timeOut: 3000 });
                    }
                });
            });

            // Add table form submission
            $('#addTableForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                if ($form.valid()) {
                    $.ajax({
                        url: $form.attr('action'),
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (response) {
                            if (response.success) {
                                toastr.success(response.message, 'Success', { timeOut: 3000 });
                                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                                $form[0].reset();
                                addTableModal.hide();
                                removeBackdrop();
                            } else {
                                toastr.error(response.message, 'Error', { timeOut: 3000 });
                            }
                        },
                        error: function () {
                            toastr.error('Error adding table.', 'Error', { timeOut: 3000 });
                        }
                    });
                }
            });

            // Edit table form submission
            $('#editTableForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                if ($form.valid()) {
                    $.ajax({
                        url: $form.attr('action'),
                        type: 'POST',
                        data: $form.serialize(),
                        success: function (response) {
                            if (response.success) {
                                toastr.success(response.message, 'Success', { timeOut: 3000 });
                                fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                                editTableModal.hide();
                                removeBackdrop();
                            } else {
                                toastr.error(response.message, 'Error', { timeOut: 3000 });
                            }
                        },
                        error: function () {
                            toastr.error('Error editing table.', 'Error', { timeOut: 3000 });
                        }
                    });
                }
            });

            // Delete table form submission
            $('#deleteTableForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                $.ajax({
                    url: $form.attr('action'),
                    type: 'POST',
                    data: $form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            toastr.success(response.message, 'Success', { timeOut: 3000 });
                            fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                            deleteTableModal.hide();
                            removeBackdrop();
                        } else {
                            toastr.error(response.message, 'Error', { timeOut: 3000 });
                        }
                    },
                    error: function () {
                        toastr.error('Error deleting table.', 'Error', { timeOut: 3000 });
                    }
                });
            });

            // Delete multiple tables form submission
            $('#deleteTablesForm').on('submit', function (e) {
                e.preventDefault();
                var $form = $(this);
                $.ajax({
                    url: $form.attr('action'),
                    type: 'POST',
                    data: $form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            toastr.success(response.message, 'Success', { timeOut: 3000 });
                            selectedTableIds.clear();
                            fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);
                            var modal = bootstrap.Modal.getInstance(document.getElementById('deleteTablesModal'));
                            modal.hide();
                            removeBackdrop();
                        } else {
                            toastr.error(response.message, 'Error', { timeOut: 3000 });
                        }
                    },
                    error: function () {
                        toastr.error('Error deleting tables.', 'Error', { timeOut: 3000 });
                    }
                });
            });

            // Helper function to remove backdrop
            function removeBackdrop() {
                setTimeout(() => {
                    document.body.classList.remove('modal-open');
                    $('.modal-backdrop').remove();
                }, 300);
            }

            // Table add modal show
            $('#TableAddModal').on('show.bs.modal', function (e) {
                if (selectedSection && selectedSectionName) {
                    $('#sectionIdAtAddTable').val(selectedSection);
                    $('#sectionNameAtAddTable').val(selectedSectionName);
                } else {
                    $('#sectionNameAtAddTable').val('No section selected');
                    $('#sectionIdAtAddTable').val('');
                }
            });

            // Edit section button
            $(document).on('click', '.edit-section-button', function (e) {
                e.preventDefault();
                var sectionId = $(this).data("sectionid");
                var sectionElement = $(`[data-sectionid='${sectionId}']`);
                var sectionName = sectionElement.data("sectionname");
                var sectionDescription = sectionElement.data("sectiondescription");

                $("#editSectionId").val(sectionId);
                $("#editSectionName").val(sectionName);
                $("#editSectionDescription").val(sectionDescription);

                var editModal = new bootstrap.Modal(document.getElementById('editSectionModal'), { backdrop: 'static' });
                editModal.show();
            });

            // Delete section button
            $(document).on('click', '.delete-section-btn', function (e) {
                e.preventDefault();
                var sectionId = $(this).data("sectionid");
                $("#deleteSectionId").val(sectionId);

                var deleteModal = new bootstrap.Modal(document.getElementById('deleteSectionModal'), { backdrop: 'static' });
                deleteModal.show();
            });

            // Edit table link
            $(document).on('click', '.edit-table-link', function (e) {
                e.preventDefault();
                var tableId = $(this).data('table-id');
                var tableName = $(this).data('table-name');
                var tableCapacity = $(this).data('table-capacity');
                var tableStatus = $(this).data('table-status');
                var sectionId = $(this).data('section-id');
                var sectionName = $(this).data('section-name');

                $('#Tablename').val(tableName);
                $('#TableId').val(tableId);
                $('#Capacity').val(tableCapacity);
                $('#Status').val(tableStatus > 1 ? 2 : 1);
                $('#sectionIdAtEditTable').val(sectionId);
                $('#sectionNameAtEditTable').val(sectionName);

                editTableModal.show();
            });

            // Delete table link
            $(document).on('click', '.delete-table-link', function (e) {
                e.preventDefault();
                var tableId = $(this).data('table-id');
                $('#deleteTableId').val(tableId);
                deleteTableModal.show();
            });

            // Handle modal close (No or cross button)
            $('#editTableModal, #deleteTableModal').on('hidden.bs.modal', function () {
                removeBackdrop();
            });

            // Initialize
            fetchTables(selectedSection, searchTerm, currentPage, rowsPerPage);

            
            $('.section-link.active').find('.activePens').removeClass('d-none');
            // $('#TableAddModalID').prop('disabled', true);
            if ($('.section-link.active').length > 0) {
                selectedSectionName = $('.section-link.active').data('sectionname');
                selectedSection = $('.section-link.active').data('sectionid');
                $('#sectionIdAtAddTable').val(selectedSection);
                $('#sectionNameAtAddTable').val(selectedSectionName);
                $('#TableAddModalID').prop('disabled', false);
            }
        });
   