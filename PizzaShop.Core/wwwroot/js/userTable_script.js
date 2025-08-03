$(document).ready(function () {
  toastr.options.closeButton = true;
  let currentPage = 1;
  let pageSize = 5;
  let currentSearchTerm = ""; // Add variable to store the search term

  var sortBy = ""; // "name" "Role"
  var sortDirection = ""; // "asc", "desc"

  //modal instances
  const deleteUserModal = new bootstrap.Modal(document.getElementById("DeleteUserModal"), {keyboard: false,backdrop: "static"});
  
  // Delete user
  $(document).on("click", ".delete-user", function (e) {
    e.preventDefault();
    const userId = $(this).data("id");
    $('#deleteUserId').val(userId);

    // console.log($('#deleteUserId').val());
    deleteUserModal.show();
  });

  $(document).on("click", "#DeleteUserForm", function (e) {
    e.preventDefault();
    const userId = $('#deleteUserId').val();
    $.ajax({
      url: `/UserTable/deleteUserById`,
      type: "POST",
      data: { id: userId },
      success: function (response) {
        if (response.success) {
          toastr.success(response.message);
          fetchUsers(currentPage, pageSize, currentSearchTerm); 
          deleteUserModal.hide();
          removeBackdrop();
        } else {
          toastr.error(response.message);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error deleting user:", error);
        toastr.error("An error occurred while deleting the user.");
      },
    });
  });

  function removeBackdrop() {
    setTimeout(() => {
      document.body.classList.remove("modal-open");
      $(".modal-backdrop").remove();
      document.body.style.overflow = "auto";
    }, 300);
  }
    

  function fetchUsers(page, size, searchTerm = "") {
    $.ajax({
      url: `/UserTable/GetUsers`,
      type: "GET",
      data: { page: page, 
              pageSize: size, 
              searchTerm: searchTerm,
              sortBy: sortBy,
              sortDirection: sortDirection,
      },
      dataType: "json",
      success: function (data) {
        updateTable(data.data, data.canedit, data.candelete);
        updatePagination(page, data.totalUsers, size);
      },
      error: function (xhr, status, error) {
        console.error("Error fetching users:", error);
      },
    });
  }

  // Update the search term and fetch filtered data
  $(".search_bar").on("input", function () {
    currentSearchTerm = $(this).val().trim().toLowerCase(); // Store the search term
    fetchUsers(1, pageSize, currentSearchTerm); // Reset to page 1 with search term
  });

  function updateTable(users, canedit, candelete) {
    const tbody = $("tbody");
    tbody.empty(); // Clear existing rows
    $.each(users, function (index, user) {
      let spanTag = "";
      if (user.status == 1) spanTag = `<span class="Active">Active</span>`;
      if (user.status == 2) spanTag = `<span class="Inactive">Inactive</span>`;
      if (user.status == 3) spanTag = `<span class="pending">Pending</span>`;
      let roleTag = "";
      // console.log("user roleid" , user.role);
      if (user.role == 3) roleTag = `Admin`;
      if (user.role == 2) roleTag = `Chef`;
      if (user.role == 1) roleTag = `AccountManager`;
      var image = "~/images/Default_pfp.svg.png";
      if (user.image != "default") image = user.image;

      var edit = `<form method="get" action="/UserTable/EditUserById" class="d-inline">
                                <input type="hidden" name="id" value="${user.id}">
                                <button type="submit" class="btn btn-link p-0 mx-1"><i class="bi bi-pen"></i></button>
                            </form>`;
      var del = `<a href="#" class="delete-user" data-id="${user.id}">
                                <i class="bi bi-trash"></i>
                            </a>`;
      const row = `<tr>
                    <td>
                      <img class="m-1 rounded-circle" src="${image}" height="25px" width="25px" alt="">${user.firstname} ${user.lastname}
                    </td>
                    <td class="text-center">${user.email}</td>
                    <td class="text-center">${user.phone}</td>
                    <td class="text-center">${roleTag}</td>
                    <td class="text-center">${spanTag}</td>
                    <td class="text-center">
                        ${canedit ? edit : ""}
                        ${candelete ? del : ""}
                    </td>
                  </tr>`;
      tbody.append(row);
    });
  }

  function updatePagination(currentPage, totalUsers, pageSize) {
    const totalPages = Math.ceil(totalUsers / pageSize);

    $("#pagination-info").text(
      `Showing ${(currentPage - 1) * pageSize + 1}-${Math.min(
        currentPage * pageSize,
        totalUsers
      )} of ${totalUsers}`
    );

    $(".prev-page").toggleClass("disabled", currentPage <= 1);
    $(".next-page").toggleClass("disabled", currentPage >= totalPages);

    // Pass the current search term to pagination clicks
    $(".prev-page")
      .off("click")
      .on("click", function (e) {
        e.preventDefault();
        if (currentPage > 1) {
          currentPage = --currentPage;
          fetchUsers(currentPage, pageSize, currentSearchTerm);
        }
      });

    $(".next-page")
      .off("click")
      .on("click", function (e) {
        e.preventDefault();
        if (currentPage < totalPages) {
          currentPage = ++currentPage;
          fetchUsers(currentPage, pageSize, currentSearchTerm);
        }
      });
  }

  $(".page-size-option").click(function (e) {
    e.preventDefault();
    pageSize = $(this).data("size");
    $("#itemsPerPageBtn").text(pageSize);
    currentPage = 1; // Reset to first page
    fetchUsers(currentPage, pageSize, currentSearchTerm); // Use current search term
  });




  // sorting handler
  $(document).on("click", "#nameAscending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "asc";
    fetchUsers(currentPage, pageSize);
  });
  $(document).on("click", "#nameDescending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "desc";
    fetchUsers(currentPage, pageSize);
  });
  $(document).on("click", "#roleAscending", function (e) {
    e.preventDefault();
    sortBy = "role";
    sortDirection = "asc";
    fetchUsers(currentPage, pageSize);
  });
  $(document).on("click", "#roleDescending", function (e) {
    e.preventDefault();
    sortBy = "role";
    sortDirection = "desc";
    fetchUsers(currentPage, pageSize);
  });

  // Load first page initially
  fetchUsers(currentPage, pageSize);
});
