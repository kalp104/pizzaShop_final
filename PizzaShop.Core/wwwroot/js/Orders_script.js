$(document).ready(function () {
  toastr.options.closeButton = true;
  var rowsPerPage = 5;
  var currentPage = 1;
  var totalItems = 0;
  var searchTerm = "";
  var orderStatus = "";
  var dateRange = "";
  var fromDate = "";
  var toDate = "";

  // Sorting variables
  var sortBy = ""; // "order", "date", "name", "amount"
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

  // Real-time validation on input change
  $("#FromDateInput, #ToDateInput").on("change", function () {
    validateDates();
  });

  // Fetch orders function
  function fetchOrders(page, pageSize) {
    $.ajax({
      url: "/Order/FilterOrders",
      type: "GET",
      data: {
        searchTerm: searchTerm,
        pageNumber: page,
        pageSize: pageSize,
        status: orderStatus,
        dateRange: dateRange,
        fromDate: fromDate,
        toDate: toDate,
        sortBy: sortBy,
        sortDirection: sortDirection,
      },
      success: function (data) {
        $("#OrderContainer").html(data);
        totalItems =
          parseInt($("#TableContainer").attr("data-total-items")) || 0;
        updatePagination();
      },
      error: function () {
        console.log("Error loading orders.");
        toastr.error("Error loading orders.", "Error", { timeOut: 3000 });
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
      fetchOrders(currentPage, rowsPerPage);
    }
    $("#itemsPerPageMenu").hide();
  });

  // Toggle dropdown paging
  $("#itemsPerPageBtn").on("click", function () {
    $("#itemsPerPageMenu").toggle();
  });

  // Previous page
  $(document).on("click", "#prevPage", function (e) {
    e.preventDefault();
    if (currentPage > 1) {
      currentPage--;
      fetchOrders(currentPage, rowsPerPage);
    }
  });

  // Next page
  $(document).on("click", "#nextPage", function (e) {
    e.preventDefault();
    if (currentPage * rowsPerPage < totalItems) {
      currentPage++;
      fetchOrders(currentPage, rowsPerPage);
    }
  });

  // Search input
  $(document).on("input", "#search_tax", function () {
    searchTerm = $(this).val().trim();
    currentPage=1;
    fetchOrders(currentPage, rowsPerPage);
  });

  $("#orderStatusFilter").on('change', function(){
    orderStatus = $("#orderStatusFilter").val();
    currentPage = 1;
    fetchOrders(currentPage, rowsPerPage);
  })
  $("#dateRangeFilter").on('change', function(){
    dateRange = $("#dateRangeFilter").val();
    currentPage = 1;
    fetchOrders(currentPage, rowsPerPage);
  })

  // Search button with validation
  $("#searchBtn").on("click", function () {
    fromDate = $("#FromDateInput").val();
    toDate = $("#ToDateInput").val();
    orderStatus = $("#orderStatusFilter").val();
    dateRange = $("#dateRangeFilter").val();

    if (validateDates()) {
      currentPage = 1;
      fetchOrders(currentPage, rowsPerPage);
    } else {
      toastr.error(
        "Please correct the date errors before searching.",
        "Error",
        { timeOut: 3000 }
      );
    }
  });

  // Clear button
  $("#clearBtn").on("click", function () {
    searchTerm = "";
    orderStatus = "";
    dateRange = "";
    fromDate = "";
    toDate = "";
    $("#search_tax").val("");
    $("#orderStatusFilter").val("");
    $("#dateRangeFilter").val("");
    $("#FromDateInput").val("");
    $("#ToDateInput").val("");
    $("#fromDateError").text("");
    $("#toDateError").text("");
    currentPage = 1;
    fetchOrders(currentPage, rowsPerPage);
  });

  // Export button with loader
  $("#exportBtn").on("click", function (e) {
    e.preventDefault();
    if (validateDates()) {
      $("#exportLoader").removeClass("d-none");
      $("#exportBtn .bi-box-arrow-up-right").addClass("d-none");

      var url =
        "/Order/ExportOrders?" +
        $.param({
          searchTerm: searchTerm,
          status: orderStatus,
          dateRange: $("#dateRangeFilter").val(),
          fromDate: $("#FromDateInput").val(),
          toDate: $("#ToDateInput").val()
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
        toastr.success("pdf invoice file downloaded successfully.", "Success", {
          timeOut: 3000,
        });
      }, 2000);
    } else {
      toastr.error(
        "Please correct the date errors before exporting.",
        "Error",
        { timeOut: 3000 }
      );
    }
  });

  $(document).on("click", "#OrderDetailEye", function (e) {
    e.preventDefault();
    var orderId = $(this).data("order-id");
    console.log("clicked order id : ", orderId);
    window.location.href = "/Order/OrderDetail?orderId=" + orderId;
  });

  $(document).on("click", "#OrderDetailpdf", function (e) {
    e.preventDefault();
    var orderId = $(this).data("order-id");
    console.log("clicked order id : ", orderId);
    window.location.href = "/Order/GenerateInvoice?orderId=" + orderId;
    toastr.success("Pdf invoice downloaded successfully.", "Success", {
      timeOut: 3000,
    });
  });

  // Sorting handlers
  $(document).on("click", "#orderAscending", function (e) {
    e.preventDefault();
    sortBy = "order";
    sortDirection = "asc";
    fetchOrders(currentPage, rowsPerPage);
  });
  $(document).on("click", "#orderDescending", function (e) {
    e.preventDefault();
    sortBy = "order";
    sortDirection = "desc";
    fetchOrders(currentPage, rowsPerPage);
  });

  $(document).on("click", "#dateAscending", function (e) {
    e.preventDefault();
    sortBy = "date";
    sortDirection = "asc";
    fetchOrders(currentPage, rowsPerPage);
  });
  $(document).on("click", "#dateDescending", function (e) {
    e.preventDefault();
    sortBy = "date";
    sortDirection = "desc";
    fetchOrders(currentPage, rowsPerPage);
  });

  $(document).on("click", "#nameAscending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "asc";
    fetchOrders(currentPage, rowsPerPage);
  });
  $(document).on("click", "#nameDescending", function (e) {
    e.preventDefault();
    sortBy = "name";
    sortDirection = "desc";
    fetchOrders(currentPage, rowsPerPage);
  });

  $(document).on("click", "#amountAscending", function (e) {
    e.preventDefault();
    sortBy = "amount";
    sortDirection = "asc";
    fetchOrders(currentPage, rowsPerPage);
  });
  $(document).on("click", "#amountDescending", function (e) {
    e.preventDefault();
    sortBy = "amount";
    sortDirection = "desc";
    fetchOrders(currentPage, rowsPerPage);
  });

  // Initial fetch
  fetchOrders(currentPage, rowsPerPage);
});
