// Prevent re-execution of script
if (!window.categoryScriptsInitialized) {
  window.categoryScriptsInitialized = true;

  $(document).ready(function () {
    toastr.options.closeButton = true;

    // Variables for category and item views
    var rowsPerPage = 5;
    var currentPage = 1;
    var totalItems = window.totalItems || 0;
    var selectedCategory = null;
    var selectedCategoryName = "";
    var searchTerm = "";
    var selectedItemIds = [];
    var canEdit = window.canEdit;
    var canDelete = window.canDelete;
    var filterItemsUrl = window.filterItemsUrl || "";
    var filterCategoriesUrl = window.filterCategoriesUrl || "";

    // Modal initializations
    var openAddCategoryModal = new bootstrap.Modal("#exampleModal", {
      backdrop: "static",
      keyboard: false,
    });
    var openEditCategoryModal = new bootstrap.Modal("#editCategoryModal", {
      backdrop: "static",
      keyboard: false,
    });
    var openDeleteCategoryModal = new bootstrap.Modal("#deleteCategoryModal", {
      backdrop: "static",
      keyboard: false,
    });
    var openAddItemModal = new bootstrap.Modal("#addItem", {
      backdrop: "static",
      keyboard: false,
    });
    var openDeleteItemModal = new bootstrap.Modal("#ItemDeleteModal", {
      backdrop: "static",
      keyboard: false,
    });
    var openEditItemModal = new bootstrap.Modal("#editItemModal", {
      backdrop: "static",
      keyboard: false,
    });

    // Flag to track AJAX request status
    var isAjaxRequestInProgress = false;
    var isSubmitting = false;

    // Modifier state for edit and add item modals
    var selectedGroupsEdit = {};
    var selectedGroupsAdd = {};

    // Fetch items
    function fetchItems(categoryId, searchTerm = "", page, pageSize) {
      $.ajax({
        url: filterItemsUrl,
        type: "GET",
        data: {
          categoryId: categoryId,
          searchTerm: searchTerm,
          pageNumber: page,
          pageSize: pageSize,
        },
        success: function (data) {
          $("#collapse1").html(data);
          totalItems =
            parseInt($("#itemsContainer").attr("data-total-items")) || 0;
          updatePagination();
          restoreCheckboxSelections();
          updateDeleteButtonState();
        },
        error: function (xhr, status, error) {
          console.error("Error loading items:", xhr, status, error);
          toastr.error("Failed to load items.", "Error", { timeOut: 3000 });
        },
      });
    }

    // ============================================XXXXXXX======================================//

    function fetchCategories() {
      $.ajax({
        url: "/Menu/GetCategories",
        type: "GET",
        success: function (data) {
          console.log("data", data.categories);
          updateCategoryList(data.categories);
          updateFilterList(data.categories);
          fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
        },
        error: function () {
          toastr.error("Error loading sections.", "Error", { timeOut: 3000 });
        },
      });
    }
    fetchCategories();

    function updateFilterList(categories) {
      $("#selectedCategoryForAddItem").empty();
      $("#selectedCategoryForAddItem").append(
        '<option value="">Select Category</option>'
      );
      categories.forEach(function (category) {
        var option = $("<option></option>")
          .attr("value", category.categoryid)
          .text(category.categoryname);
        $("#selectedCategoryForAddItem").append(option);
      });
    }

    function updateCategoryList(category) {
      var $sectionList = $("#Category-list");
      var $noSections = $("#no-categories");
      $sectionList.empty();

      // If no categories, show "no categories" message and return
      if (category.length === 0) {
        $noSections.show();
        return;
      }

      $noSections.hide();

      // If no category is selected and there are categories, select the first one
      if (!selectedCategory && category.length > 0) {
        selectedCategory = category[0].categoryid;
      }

      category.forEach(function (s) {
        // Determine if this category is active based on selectedCategory
        var isActive = s.categoryid === selectedCategory ? "active" : "";
        var textColor =
          s.categoryid === selectedCategory ? "text-white" : "text-dark";
        var pensVisible = s.categoryid === selectedCategory ? "" : "d-none";

        // Build edit/delete buttons based on permissions
        var activePensHtml = "";
        if (canEdit) {
          activePensHtml += `
            <a href="#" class="text-primary edit-category-button" data-category-id="${s.categoryid}"
               data-category-name="${s.categoryname}" data-category-description="${s.categorydescription}">
                <i class="bi bi-pen mx-1"></i>
            </a>`;
        }
        if (canDelete) {
          activePensHtml += `
            <a href="#" class="text-primary delete-category-btn" data-category-id="${s.categoryid}">
                <i class="bi bi-trash"></i>
            </a>`;
        }
        activePensHtml = `<div class="activePens ${pensVisible}">${activePensHtml}</div>`;

        // Build the category HTML
        var sectionHtml = `
          <li class="nav-link links category-link d-flex justify-content-between align-items-center gap-2 ${isActive}"
              id="v-pills-${s.categoryid}-tab-category" data-category-id="${s.categoryid}"
              data-category-name="${s.categoryname}" data-category-description="${s.categorydescription}">
              <a class="text-decoration-none ${textColor}">
                  <i class="bi bi-grid-3x2-gap-fill me-2"></i>${s.categoryname}
              </a>
              ${activePensHtml}
          </li>`;
        $sectionList.append(sectionHtml);
      });
    }

    // ============================================XXXXXXX======================================//

    // Update pagination
    function updatePagination() {
      var totalPages = Math.ceil(totalItems / rowsPerPage);
      var startItem = (currentPage - 1) * rowsPerPage + 1;
      var endItem = Math.min(currentPage * rowsPerPage, totalItems);
      $("#pagination-info").text(
        `Showing ${startItem}-${endItem} of ${totalItems}`
      );
      $("#prevPage").toggleClass("disabled", currentPage === 1);
      $("#nextPage").toggleClass("disabled", currentPage >= totalPages);
    }

    // Restore checkbox selections
    function restoreCheckboxSelections() {
      $("#collapse1 .item-checkbox").each(function () {
        var itemId = $(this).val();
        var isSelected = selectedItemIds.includes(itemId);
        $(this).prop("checked", isSelected);
      });
      var $checkboxes = $("#collapse1 .item-checkbox");
      $("#selectAllItemsCheckbox").prop(
        "checked",
        $checkboxes.length > 0 &&
          $checkboxes.length === $checkboxes.filter(":checked").length
      );
    }

    // Update delete button state
    function updateDeleteButtonState() {
      $("#deleteItems").prop("disabled", selectedItemIds.length === 0);
    }

    // Toggle custom dropdown visibility
    $("#itemsPerPageBtn").on("click", function () {
      var $menu = $("#itemsPerPageMenu");
      $menu.toggle();
    });

    // Close dropdown when clicking outside
    $(document).on("click", function (e) {
      var $dropdown = $(".custom-dropdown");
      if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
        $("#itemsPerPageMenu").hide();
      }
    });

    // Handle page size selection
    $(document).on("click", ".page-size-option", function (e) {
      e.preventDefault();
      var newSize = parseInt($(this).data("size"));
      if (newSize !== rowsPerPage) {
        rowsPerPage = newSize;
        $("#itemsPerPageBtn").html(
          `${rowsPerPage} <span><i class="bi bi-chevron-down"></i></span>`
        );
        currentPage = 1;
        fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
      }
      $("#itemsPerPageMenu").hide();
    });

    // Pagination controls
    $(document).on("click", "#prevPage", function (e) {
      e.preventDefault();
      if (currentPage > 1) {
        currentPage--;
        fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
      }
    });

    $(document).on("click", "#nextPage", function (e) {
      e.preventDefault();
      if (currentPage * rowsPerPage < totalItems) {
        currentPage++;
        fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
      }
    });

    // Category link click handler
    $(document).on("click", ".category-link", function (e) {
      e.preventDefault();
      $(".category-link")
        .removeClass("active")
        .find("a")
        .removeClass("text-white")
        .addClass("text-dark");
      $(".activePens").addClass("d-none");
      $(this)
        .addClass("active")
        .find("a")
        .removeClass("text-dark")
        .addClass("text-white");
      $(this).find(".activePens").removeClass("d-none");
      selectedCategory = $(this).data("category-id");
      // console.log("Selected Category ID:", selectedCategory);
      selectedCategoryName = $(this).data("category-name");
      $("#selectedCategoryForAddItem").val(selectedCategory);

      // console.log("Selected Category Name:", selectedCategoryName);
      // $('#sectionIdAtAddTable').val(selectedSection);
      // $('#sectionNameAtAddTable').val(selectedSectionName);
      searchTerm = "";
      currentPage = 1;
      fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
      selectedItemIds = [];
      $("#searchInput").val("");
    });

    // Search input handler
    $(document).on("input", "#searchInput", function () {
      searchTerm = $(this).val().trim();
      currentPage = 1;
      fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
    });

    // Select/Deselect all checkboxes
    $(document).on("change", "#collapse1 #selectAllItemsCheckbox", function () {
      var isChecked = $(this).is(":checked");
      $("#collapse1 .item-checkbox").prop("checked", isChecked);
      var currentPageIds = $("#collapse1 .item-checkbox")
        .map(function () {
          return $(this).val();
        })
        .get();

      if (isChecked) {
        currentPageIds.forEach(function (id) {
          if (!selectedItemIds.includes(id)) {
            selectedItemIds.push(id);
          }
        });
      } else {
        selectedItemIds = selectedItemIds.filter(
          (id) => !currentPageIds.includes(id)
        );
      }
      updateDeleteButtonState();
    });

    // Individual checkbox selection
    $(document).on("change", "#collapse1 .item-checkbox", function () {
      var itemId = $(this).val();
      if ($(this).is(":checked")) {
        if (!selectedItemIds.includes(itemId)) {
          selectedItemIds.push(itemId);
        }
      } else {
        selectedItemIds = selectedItemIds.filter((id) => id !== itemId);
      }
      var $checkboxes = $("#collapse1 .item-checkbox");
      $("#selectAllItemsCheckbox").prop(
        "checked",
        $checkboxes.length > 0 &&
          $checkboxes.length === $checkboxes.filter(":checked").length
      );
      updateDeleteButtonState();
    });

    // Add category modal
    $(document).on("click", "#openAddCategoryModal", function () {
      openAddCategoryModal.show();
    });

    // Add item modal
    $(document).on("click", "#openAddItemModal", function (e) {
      e.preventDefault();
      $("#selectedCategoryForAddItem").val(selectedCategory);
      console.log("Selected Category ID:", selectedCategory);
      openAddItemModal.show();
    });

    // Edit category button
    $(document).on("click", ".edit-category-button", function () {
      var categoryId = $(this).data("category-id");
      var categoryElement = $(`[data-category-id='${categoryId}']`);
      var categoryName = categoryElement.data("category-name");
      var categoryDescription = categoryElement.data("category-description");

      $("#editCategoryId").val(categoryId);
      $("#editCategoryName").val(categoryName);
      $("#editCategoryDescription").val(categoryDescription);
      $("#selectedCategoryForAddItem").val(categoryId);

      $("#selectedCategoryForAddItem").val(selectedCategory);
      console.log("Selected Category ID:", selectedCategory);
      openEditCategoryModal.show();
    });

    // Delete category button
    $(document).on("click", ".delete-category-btn", function () {
      let categoryId = $(this).data("category-id");
      $("#deleteCategoryId").val(categoryId);
      $("#deleteCategoryText").html(
        "Are you sure you want to delete this category?"
      );
      openDeleteCategoryModal.show();
    });

    // Multiple delete button
    $(document).on("click", "#deleteItems", function (e) {
      e.preventDefault();
      if (!$(this).prop("disabled") && selectedItemIds.length > 0) {
        $("#selectedItemIds").val(selectedItemIds.join(","));
        $("#exampleModal3").modal("show");
      }
    });

    // Populate selected item IDs in multiple delete modal
    $("#exampleModal3").on("show.bs.modal", function () {
      $("#selectedItemIds").val(selectedItemIds.join(","));
      console.log("Selected Item IDs for Multiple Delete:", selectedItemIds);
    });

    // Add category handler
    $("#AddCategoryForm").on("submit", function (e) {
      e.preventDefault();

      var form = $(this);
      if (form.valid()) {
        var formData = $(this).serialize();
        $.ajax({
          url: "/Menu/AddCategory",
          type: "POST",
          data: formData,
          dataType: "json",
          success: function (response) {
            if (response.success) {
              toastr.success(response.message, "Success", { timeOut: 3000 });
              $("#AddCategoryForm")[0].reset();
              fetchCategories();
              openAddCategoryModal.hide();
              removeBackdrop();
              // location.reload();
            } else {
              toastr.error(
                response.message || "Failed to add category.",
                "Error",
                { timeOut: 3000 }
              );
            }
          },
          error: function (xhr, status, error) {
            console.error("Error adding category:", xhr, status, error);
            toastr.error(
              "An error occurred while adding the category.",
              "Error",
              { timeOut: 3000 }
            );
          },
        });
      }
    });

    // Edit category handler
    $(document).on("submit", "#EditCategoryForm", function (e) {
      e.preventDefault();

      var formData = $(this).serialize();
      $.ajax({
        url: "/Menu/EditCategory",
        type: "POST",
        data: formData,
        success: function (response) {
          if (response.success) {
            $("#EditCategoryForm")[0].reset();
            toastr.success(response.message, "Success", { timeOut: 3000 });
            fetchCategories();
            openEditCategoryModal.hide();
            removeBackdrop();
            $("#selectedCategoryForAddItem").val(selectedCategory);
            console.log("Selected Category ID:", selectedCategory);

            // location.reload();
          } else {
            toastr.error(
              response.message || "Failed to edit category.",
              "Error",
              { timeOut: 3000 }
            );
          }
        },
        error: function (xhr, status, error) {
          console.error("Error editing category:", xhr, status, error);
          toastr.error(
            "An error occurred while editing the category.",
            "Error",
            { timeOut: 3000 }
          );
        },
      });
    });

    // Delete category handler
    $(document).on("submit", "#DeleteCategoryForm", function (e) {
      e.preventDefault();

      var formData = $(this).serialize();
      $.ajax({
        url: "/Menu/DeleteCategory",
        type: "POST",
        data: formData,
        success: function (response) {
          if (response.success) {
            toastr.success(response.message, "Success", { timeOut: 3000 });
            $("#DeleteCategoryForm")[0].reset();
            selectedCategory = null;
            selectedCategoryName = "";
            fetchCategories();
            fetchItems(null, searchTerm, currentPage, rowsPerPage);
            openDeleteCategoryModal.hide();
            removeBackdrop();
          } else {
            toastr.error(
              response.message || "Failed to delete category.",
              "Error",
              { timeOut: 3000 }
            );
          }
        },
        error: function (xhr, status, error) {
          console.error("Error deleting category:", xhr, status, error);
          toastr.error(
            "An error occurred while deleting the category.",
            "Error",
            { timeOut: 3000 }
          );
        },
      });
    });

    // Add item handler
    $(document).on("submit", "#addItemForm", function (e) {
      e.preventDefault();

      // Add selected modifier groups to form data
      $("#selectedModifierGroupsAdd").val(JSON.stringify(selectedGroupsAdd));
      var formData = new FormData(this);

      $.ajax({
        url: "/Menu/AddItem",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
          if (response.success) {
            // Close the modal
            openAddItemModal.hide();

            // Reset the form
            $("#addItemForm")[0].reset();

            // Clear selected modifier groups and reset container
            selectedGroupsAdd = {};
            renderModifiersAdd();

            // Clear image input and display
            $("#imageInputAdd").val("");
            $(".AddedImageURL").text("");

            // Refresh the item list
            fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);

            // Show success notification
            toastr.success(response.message, "Success", { timeOut: 3000 });

            // Remove backdrop
            removeBackdrop();
          } else {
            toastr.error(response.message || "Failed to add item.", "Error", {
              timeOut: 3000,
            });
          }
        },
        error: function (xhr, status, error) {
          console.error("Error adding item:", xhr, status, error);
          toastr.error("An error occurred while adding item.", "Error", {
            timeOut: 3000,
          });
        },
      });
    });

    // Toggle modifier container visibility for add item
    $(document).on("click", "#toggleModifiersAdd", function () {
      $("#modifierContainerAdd").slideToggle("fast");
    });

    // Modifier group selection for add item
    $(document).on("click", ".modifier-group-item", function (e) {
      e.preventDefault();
      var $this = $(this);
      var modifierGroupId = $this
        .find(".modifier-checkbox-add")
        .data("modifiergroup-id");
      var groupName = $this.find("span").text().trim();
      var $checkbox = $this.find(".modifier-checkbox-add");

      $checkbox.prop("checked", !$checkbox.prop("checked"));

      if (!selectedGroupsAdd[modifierGroupId]) {
        $.ajax({
          url: "/Menu/GetModifiersByGroup",
          type: "GET",
          data: { modifierGroupId: modifierGroupId },
          success: function (response) {
            if (response && response.length > 0) {
              selectedGroupsAdd[modifierGroupId] = {
                name: groupName,
                minValue: 0,
                maxValue: 0,
                modifiers: response.map((modifier) => ({
                  modifierId: modifier.modifierid,
                  modifiername: modifier.modifiername || "Unnamed Modifier",
                  modifierrate: modifier.modifierrate,
                })),
              };
              renderModifiersAdd();
            }
          },
          error: function (xhr, status, error) {
            $("#modifiers-container-add").html(`
                            <div class="p-2 m-2 alert alert-danger">
                                Error loading modifiers for ${groupName}: ${error}
                            </div>
                        `);
          },
        });
      } else {
        delete selectedGroupsAdd[modifierGroupId];
        renderModifiersAdd();
      }
    });

    // Render modifiers for add item
    function renderModifiersAdd() {
      $("#modifiers-container-add").empty();
      if (Object.keys(selectedGroupsAdd).length > 0) {
        let html = "";
        for (let groupId in selectedGroupsAdd) {
          let group = selectedGroupsAdd[groupId];
          // Ensure minValue and maxValue default to 1 if undefined or invalid
          group.minValue = group.minValue >= 1 ? group.minValue : 1;
          group.maxValue = group.maxValue >= 1 ? group.maxValue : 1;

          html += `
                <div class="mb-3">
                    <div class="px-3 d-flex justify-content-between">
                        <div style="font-size:20px">${group.name}</div>
                        <div class="trash-icon-add" style="font-size:20px; cursor:pointer" data-group-id="${groupId}">
                            <i class="bi bi-trash-fill"></i>
                        </div>
                    </div>
                    <div class="px-3 pb-1 d-flex justify-content-between mt-1">
                        <select class="form-select min-value-add" data-group-id="${groupId}" style="width: 80px;">
                            <option value="1" ${
                              group.minValue === 1 ? "selected" : ""
                            }>1</option>
                            <option value="2" ${
                              group.minValue === 2 ? "selected" : ""
                            }>2</option>
                            <option value="3" ${
                              group.minValue === 3 ? "selected" : ""
                            }>3</option>
                        </select>
                        <select class="form-select max-value-add" data-group-id="${groupId}" style="width: 80px;">
                            <option value="1" ${
                              group.maxValue === 1 ? "selected" : ""
                            }>1</option>
                            <option value="2" ${
                              group.maxValue === 2 ? "selected" : ""
                            }>2</option>
                            <option value="3" ${
                              group.maxValue === 3 ? "selected" : ""
                            }>3</option>
                        </select>
                    </div>
                    <ul>`;
          group.modifiers.forEach(function (modifier) {
            html += `
                    <li class="px-3 d-flex justify-content-between" style="font-size:14px" data-modifier-id="${modifier.modifierId}">
                        <span>${modifier.modifiername}</span>
                        <span>${modifier.modifierrate}</span>
                    </li>`;
          });
          html += `</ul></div>`;
        }
        $("#modifiers-container-add").html(html);

        $("#addItemForm")
          .off("click", ".trash-icon-add")
          .on("click", ".trash-icon-add", function () {
            const groupId = $(this).data("group-id");
            delete selectedGroupsAdd[groupId];
            renderModifiersAdd();
          });

        $("#addItemForm")
          .off("change", ".min-value-add")
          .on("change", ".min-value-add", function () {
            const groupId = $(this).data("group-id");
            selectedGroupsAdd[groupId].minValue = parseInt($(this).val());
          });

        $("#addItemForm")
          .off("change", ".max-value-add")
          .on("change", ".max-value-add", function () {
            const groupId = $(this).data("group-id");
            selectedGroupsAdd[groupId].maxValue = parseInt($(this).val());
          });
      } else {
        $("#modifiers-container-add").html(`
            <div class="p-2 m-2 alert alert-info">No modifier groups selected</div>
        `);
      }

      $("#addItemForm .modifier-checkbox-add").each(function () {
        var groupId = $(this).data("modifiergroup-id");
        $(this).prop("checked", !!selectedGroupsAdd[groupId]);
      });
    }

    // Image input handler for add item
    $(document).on("change", "#imageInputAdd", function () {
      var file = this.files[0];
      if (file) {
        $(".AddedImageURL").text("Selected File: " + file.name);
      } else {
        $(".AddedImageURL").text("");
      }
    });

    // Reset on modal close for add item
    $(document).on("hidden.bs.modal", "#addItem", function () {
      selectedGroupsAdd = {};
      renderModifiersAdd();
      $("#addItemForm")[0].reset();
      $(".AddedImageURL").text("");
    });

    // Delete item handler
    $(document).on("click", ".delete-item-link", function (e) {
      e.preventDefault();
      var itemId = $(this).attr("data-item-id");
      console.log("Delete item clicked, Item ID:", itemId);
      $("#deleteItemId").val(itemId);
      openDeleteItemModal.show();
    });

    $(document).on("submit", "#deleteForm", function (e) {
      e.preventDefault();
      var formData = $(this).serialize();
      $.ajax({
        url: "/Menu/DeleteItem",
        type: "POST",
        data: formData,
        success: function (response) {
          if (response.success) {
            toastr.success(response.message, "Success", { timeOut: 3000 });
            $("#deleteForm")[0].reset();
            openDeleteItemModal.hide();
            removeBackdrop();
            fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
          } else {
            toastr.error(
              response.message || "Failed to delete item.",
              "Error",
              { timeOut: 3000 }
            );
          }
        },
        error: function (xhr, status, error) {
          console.error("Error deleting item:", xhr, status, error);
          toastr.error("An error occurred while deleting item.", "Error", {
            timeOut: 3000,
          });
        },
      });
    });

    // Edit item handler
    $(document).on("click", ".edit-item-link", function (e) {
      e.preventDefault();
      var itemId = $(this).data("item-id");
      console.log("Edit item clicked, Item ID:", itemId);

      if (isAjaxRequestInProgress) {
        console.log("AJAX request in progress, ignoring click");
        return;
      }

      isAjaxRequestInProgress = true;

      $.ajax({
        url: "/Menu/EditItemPartial",
        type: "GET",
        data: { id: itemId },
        success: function (data) {
          console.log("Edit partial loaded successfully");
          $("#editItemModalContent").html(data);
          openEditItemModal.show();
          loadExistingModifiers(itemId);
        },
        error: function (xhr, status, error) {
          console.error("Error loading edit form:", xhr, status, error);
          toastr.error("Error loading edit form.", "Error", { timeOut: 3000 });
        },
        complete: function () {
          isAjaxRequestInProgress = false;
        },
      });
    });

    // Edit item modal handlers
    $(document).on("change", "#imageInputEdit", function () {
      var file = this.files[0];
      if (file) {
        $("#fileExisitDisplayEditITem").text("");
        $("#fileNameDisplayEditITem").text("Selected File: " + file.name);
      } else {
        $("#fileExisitDisplayEditITem").text("");
      }
    });

    // Toggle modifier container visibility for edit item
    $(document).on("click", "#toggleModifiersEdit", function () {
      $("#modifierContainerEdit").slideToggle("fast");
    });

    // Fetch existing modifier groups for edit item
    function loadExistingModifiers(itemId) {
      $.ajax({
        url: "/Menu/GetModifiersByItemId",
        type: "GET",
        data: { itemId: itemId },
        dataType: "json",
        success: function (response) {
          if (response && Array.isArray(response) && response.length > 0) {
            selectedGroupsEdit = {};
            response.forEach(function (group) {
              selectedGroupsEdit[group.modifierGroupId] = {
                modifierGroupName: group.modifierGroupName,
                minValue: group.minValue || 0,
                maxValue: group.maxValue || 0,
                modifiers: group.modifiers.map((modifier) => ({
                  modifierId: modifier.modifierId,
                  modifierName: modifier.modifierName,
                  modifierRate: modifier.modifierRate,
                })),
              };
            });
            renderModifiersEdit();
          } else {
            $("#modifiers-container-edit").html(
              '<div class="alert alert-info">No modifier groups found</div>'
            );
          }
        },
        error: function (xhr, status, error) {
          $("#modifiers-container-edit").html(
            '<div class="alert alert-danger">Error loading existing modifiers.</div>'
          );
        },
      });
    }

    // Modifier group selection for edit item
    $(document).on("click", ".modifier-group-item", function (e) {
      e.preventDefault();
      var $this = $(this);
      var modifierGroupId = $this
        .find(".modifier-checkbox-edit")
        .data("modifiergroup-id");
      var groupName = $this.find("span").text().trim();
      var $checkbox = $this.find(".modifier-checkbox-edit");

      $checkbox.prop("checked", !$checkbox.prop("checked"));

      if (!selectedGroupsEdit[modifierGroupId]) {
        $.ajax({
          url: "/Menu/GetModifiersByGroup",
          type: "GET",
          data: { modifierGroupId: modifierGroupId },
          dataType: "json",
          success: function (response) {
            if (response && response.length > 0) {
              selectedGroupsEdit[modifierGroupId] = {
                modifierGroupName: groupName,
                minValue: 0,
                maxValue: 0,
                modifiers: response.map((modifier) => ({
                  modifierId: modifier.modifierid,
                  modifierName: modifier.modifiername,
                  modifierRate: modifier.modifierrate,
                })),
              };
              renderModifiersEdit();
            }
          },
          error: function (xhr, status, error) {
            $("#modifiers-container-edit").html(
              `<div class="alert alert-danger">Error loading modifiers for ${groupName}</div>`
            );
          },
        });
      } else {
        delete selectedGroupsEdit[modifierGroupId];
        renderModifiersEdit();
      }
    });

    // Render modifiers for edit item
    function renderModifiersEdit() {
      $("#modifiers-container-edit").empty();
      if (Object.keys(selectedGroupsEdit).length > 0) {
        let html = "";
        for (let groupId in selectedGroupsEdit) {
          let group = selectedGroupsEdit[groupId];
          if (!group || !group.modifierGroupName) continue;
          // Ensure minValue and maxValue default to 1 if undefined or invalid
          group.minValue = group.minValue >= 1 ? group.minValue : 1;
          group.maxValue = group.maxValue >= 1 ? group.maxValue : 1;

          html += `
                <div class="mb-3">
                    <div class="px-3 d-flex justify-content-between">
                        <div style="font-size:20px">${
                          group.modifierGroupName
                        }</div>
                        <div class="trash-icon-edit" style="font-size:20px; cursor:pointer" data-group-id="${groupId}">
                            <i class="bi bi-trash-fill"></i>
                        </div>
                    </div>
                    <div class="px-3 pb-1 d-flex justify-content-between mt-1">
                        <select class="form-select min-value-edit" data-group-id="${groupId}" style="width: 80px;">
                            <option value="1" ${
                              group.minValue === 1 ? "selected" : ""
                            }>1</option>
                            <option value="2" ${
                              group.minValue === 2 ? "selected" : ""
                            }>2</option>
                            <option value="3" ${
                              group.minValue === 3 ? "selected" : ""
                            }>3</option>
                        </select>
                        <select class="form-select max-value-edit" data-group-id="${groupId}" style="width: 80px;">
                            <option value="1" ${
                              group.maxValue === 1 ? "selected" : ""
                            }>1</option>
                            <option value="2" ${
                              group.maxValue === 2 ? "selected" : ""
                            }>2</option>
                            <option value="3" ${
                              group.maxValue === 3 ? "selected" : ""
                            }>3</option>
                        </select>
                    </div>
                    <ul>`;
          if (group.modifiers && Array.isArray(group.modifiers)) {
            group.modifiers.forEach(function (modifier) {
              html += `
                        <li class="px-3 d-flex justify-content-between" style="font-size:14px" data-modifier-id="${modifier.modifierId}">
                            <span>${modifier.modifierName}</span>
                            <span>${modifier.modifierRate}</span>
                        </li>`;
            });
          }
          html += `</ul></div>`;
        }
        $("#modifiers-container-edit").html(html);

        $("#editItemForm")
          .off("click", ".trash-icon-edit")
          .on("click", ".trash-icon-edit", function (e) {
            e.preventDefault();
            const groupId = $(this).data("group-id");
            delete selectedGroupsEdit[groupId];
            renderModifiersEdit();
          });

        $("#editItemForm")
          .off("change", ".min-value-edit")
          .on("change", ".min-value-edit", function () {
            const groupId = $(this).data("group-id");
            selectedGroupsEdit[groupId].minValue = parseInt($(this).val());
          });

        $("#editItemForm")
          .off("change", ".max-value-edit")
          .on("change", ".max-value-edit", function () {
            const groupId = $(this).data("group-id");
            selectedGroupsEdit[groupId].maxValue = parseInt($(this).val());
          });
      } else {
        $("#modifiers-container-edit").html(
          '<div class="alert alert-info">No modifier groups selected</div>'
        );
      }

      $("#editItemForm .modifier-checkbox-edit").each(function () {
        var groupId = $(this).data("modifiergroup-id");
        $(this).prop("checked", !!selectedGroupsEdit[groupId]);
      });
    }

    // Edit item form submission
    $(document).on("submit", "#editItemForm", function (e) {
      e.preventDefault();

      if (isSubmitting) {
        console.log("Submission blocked: Already in progress");
        return;
      }

      isSubmitting = true;
      var $form = $(this);
      var $saveButton = $("#saveItemButton");
      $saveButton.prop("disabled", true);

      $("#selectedModifierGroupsEdit").val(JSON.stringify(selectedGroupsEdit));
      var formData = new FormData($form[0]);
      console.log("Edit form data:", Array.from(formData.entries()));

      $.ajax({
        url: "/Menu/EditItem",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
          console.log("Edit response:", response);
          if (response.success) {
            $("#editItemForm")[0].reset();
            selectedGroupsEdit = {};
            renderModifiersEdit();
            fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
            toastr.success(
              response.message || "Item saved successfully.",
              "Success",
              { timeOut: 3000 }
            );
            openEditItemModal.hide();
            removeBackdrop();
          } else {
            toastr.error(
              response.message || "Failed to update item.",
              "Error",
              { timeOut: 3000 }
            );
          }
        },
        error: function (xhr, status, error) {
          console.error("Error updating item:", {
            status: status,
            error: error,
            responseText: xhr.responseText,
            statusCode: xhr.status,
          });
          toastr.error("An error occurred while updating item.", "Error", {
            timeOut: 3000,
          });
        },
        complete: function () {
          isSubmitting = false;
          $saveButton.prop("disabled", false);
        },
      });
    });

    // Reset on modal close for edit item
    $(document).on("hidden.bs.modal", "#editItemModal", function () {
      selectedGroupsEdit = {};
      renderModifiersEdit();
      isSubmitting = false;
      $("#saveItemButton").prop("disabled", false);
      console.log("Edit item modal closed, form reset");
    });

    // Multiple delete handler
    $(document).on("submit", "#deleteMultipleForm", function (e) {
      e.preventDefault();
      $.ajax({
        url: "/Menu/DeleteMultipleItems",
        type: "POST",
        data: $(this).serialize(),
        success: function (response) {
          if (response && response.success) {
            $("#exampleModal3").modal("hide");
            selectedItemIds = [];
            fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
            toastr.success("Items deleted successfully.", "Success", {
              timeOut: 3000,
            });
          } else {
            toastr.error(
              response?.message || "Error deleting items.",
              "Error",
              { timeOut: 3000 }
            );
          }
        },
        error: function (xhr, status, error) {
          console.error("Error deleting items:", xhr, status, error);
          toastr.error("Error submitting delete request.", "Error", {
            timeOut: 3000,
          });
        },
      });
    });

    // Handle "No" button for multiple delete
    $("#cancelMultipleDelete").on("click", function () {
      $("#exampleModal3").modal("hide");
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
    fetchItems(selectedCategory, searchTerm, currentPage, rowsPerPage);
  });
}
