$(document).ready(function () {
  // console.log("hello");
  toastr.options.closeButton = true;

  // Pagination and filter variables
  var rowsPerPage = 5;
  var currentPage = 1;
  var totalItems = 0;
  var searchTerm = "";
  var dateRange = "";
  var fromDate = "";
  var toDate = "";

  // Sorting variables
  var sortBy = ""; // "name", "date", "total"
  var sortDirection = ""; // "asc", "desc"

  // Get current date (today) in YYYY-MM-DD format
  var today = new Date().toISOString().split("T")[0];

  // Set max attribute for date inputs
  $("#ToDateInput").attr("max", today);
  $("#FromDateInput").attr("max", today);

  // Validation function
  function validateDates() {
    var fromDateVal = $("#FromDateInput").val();
    var toDateVal = $("#ToDateInput").val();
    var isValid = true;

    $("#fromDateError").text("");
    $("#toDateError").text("");

    if (fromDateVal && toDateVal) {
      if (new Date(fromDateVal) > new Date(toDateVal)) {
        $("#fromDateError").text("Invalid Date");
        isValid = false;
      }
    }

    if (toDateVal && new Date(toDateVal) > new Date(today)) {
      $("#toDateError").text("To Date cannot be greater than today");
      isValid = false;
    }

    return isValid;
  }

  // Real-time validation on date input change
  $("#FromDateInput, #ToDateInput").on("change", function () {
    validateDates();
  });

  // Fetch customers with optimized sorting parameters
  function fetchCustomers(page, pageSize) {
    // console.log(fromDate,toDate,dateRange);
    $.ajax({
      url: "/Customer/FilterCustomers",
      type: "GET",
      data: {
        searchTerm: searchTerm,
        pageNumber: page,
        pageSize: pageSize,
        dateRange: dateRange,
        fromDate: fromDate,
        toDate: toDate,
        sortBy: sortBy,
        sortDirection: sortDirection,
      },
      success: function (data) {
        $("#CustomerContainer").html(data);
        totalItems =
          parseInt($("#TableContainer").attr("data-total-items")) || 0;
        updatePagination();
      },
      error: function () {
        console.log("Error loading customer details");
        toastr.error("Error loading customer details.", "Error", {
          timeOut: 3000,
        });
      },
    });
  }

  // Update pagination info
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

  // Page size change
  $(document).on("click", ".page-size-option", function (e) {
    e.preventDefault();
    var newSize = parseInt($(this).data("size"));
    if (newSize !== rowsPerPage) {
      rowsPerPage = newSize;
      $("#itemsPerPageBtn").html(
        `${rowsPerPage} <span><i class="bi bi-chevron-down"></i></span>`
      );
      currentPage = 1;
      fetchCustomers(currentPage, rowsPerPage);
    }
    $("#itemsPerPageMenu").hide();
  });

  // Previous page
  $(document).on("click", "#prevPage", function (e) {
    e.preventDefault();
    if (currentPage > 1) {
      currentPage--;
      fetchCustomers(currentPage, rowsPerPage);
    }
  });

  // Next page
  $(document).on("click", "#nextPage", function (e) {
    e.preventDefault();
    if (currentPage * rowsPerPage < totalItems) {
      currentPage++;
      fetchCustomers(currentPage, rowsPerPage);
    }
  });

  // Search input
  $(document).on("input", "#search_tax", function () {
    searchTerm = $(this).val().trim();
    fetchCustomers(1, rowsPerPage);
  });
  
  $(document).on("click", ".dateRangeFilter", function (e) {
    e.preventDefault();
    dateRange = $(this).data("value");
    var text = $(this).text();
    $('#btn-content').text($(this).text());
    toDate = "";
    fromDate = "";
    // console.log("date range: ", dateRange);

    if (dateRange == 5) {
        $("#CustomDates").modal("show");
        
    } else {
        fetchCustomers(1, rowsPerPage);
    }
  });

  

  // Submitting custom date
  $(document).on("click", "#customdateSubmit", function (e) {
    e.preventDefault();
    if (validateDates()) {
      fromDate = $("#FromDateInput").val();
      toDate = $("#ToDateInput").val();
      dateRange = 5;
      // console.log(fromDate,toDate);
      if(!fromDate) {
        $("#fromDateError").text("From date is required");
      }
      if(!toDate) {
        $("#toDateError").text("To date is required");
      }
      if(fromDate && toDate) {
        fetchCustomers(1, rowsPerPage);
        $("#CustomDates").modal("hide");
      }
        
      
      
    } else {
      toastr.error("Invalid date selection", "Error", { timeOut: 3000 });
    }
  });

  $(".cloasing").on("click", function () {
    $("#dateRangeFilter").val(1);
    $("#FromDateInput").val("");
    $("#ToDateInput").val("");
    fromDate = "";
    toDate = "";
  });

  // Toggle dropdown paging
  $("#itemsPerPageBtn").on("click", function () {
    $("#itemsPerPageMenu").toggle();
  });

  // Sorting handlers
  $(document).on("click", "#nameAscending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "asc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  $(document).on("click", "#nameDescending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "desc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  $(document).on("click", "#dateAscending", function (e) {
    e.preventDefault();
    sortBy = "date";
    sortDirection = "asc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  $(document).on("click", "#dateDescending", function (e) {
    e.preventDefault();
    sortBy = "date";
    sortDirection = "desc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  $(document).on("click", "#totalAscending", function (e) {
    e.preventDefault();
    sortBy = "total";
    sortDirection = "asc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  $(document).on("click", "#totalDescending", function (e) {
    e.preventDefault();
    sortBy = "total";
    sortDirection = "desc";
    fetchCustomers(currentPage, rowsPerPage);
  });

  // Export button with loader
  $("#exportBtn").on("click", function (e) {
    e.preventDefault();

    $("#exportLoader").removeClass("d-none");
    $("#exportBtn .bi-box-arrow-up-right").addClass("d-none");

    var url =
      "/Customer/ExportCustomers?" +
      $.param({
        searchTerm: searchTerm,
        dateRange: dateRange,
        fromDate: fromDate,
        toDate: toDate,
        sortBy: sortBy,
        sortDirection: sortDirection,
      });

    var link = document.createElement("a");
    link.href = url;
    link.download = "OrdersExport.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    setTimeout(function () {
      $("#exportLoader").addClass("d-none");
      $("#exportBtn .bi-box-arrow-up-right").removeClass("d-none");
      toastr.success("Excel file downloaded successfully.", "Success", {
        timeOut: 2000,
      });
    }, 400);
  });

  // Customer details modal
  $(document).on("click", ".customerDetailsClass", function (e) {
    // console.log("modal clicked");
    var id = $(this).data("id");
    // console.log("the id is: ", id);
    $.ajax({
      url: "/Customer/customerDetails",
      type: "GET",
      data: { customerId: id },
      success: function (data) {
        // console.log(data);
        $("#customerHistoryBody").html(data);
      },
      error: function () {
        console.log("Error loading customer history");
        toastr.error("Error loading customer history.", "Error", {
          timeOut: 3000,
        });
      },
    });
  });

  // Initial fetch
  fetchCustomers(currentPage, rowsPerPage);
});
