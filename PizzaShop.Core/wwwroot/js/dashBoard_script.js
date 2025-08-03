$(document).ready(function () {
  toastr.options.closeButton = true;

  var selector; 
  var openCustomeDateModal = new bootstrap.Modal(document.getElementById('CustomDates'), {backdrop: 'static', keyboard: false});

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

  // Submitting custom date
  $(document).on("click", "#customdateSubmit", function (e) {
    e.preventDefault();
    if (validateDates()) {
      var fromDate = $("#FromDateInput").val();
      var toDate = $("#ToDateInput").val();
      if(!fromDate) {
        $("#fromDateError").text("From date is required");
      }
      if(!toDate) {
        $("#toDateError").text("To date is required");
      }
      if(fromDate && toDate) {
        // console.log(fromDate, toDate);
        $('#CustomDateValues').text(`From : ${fromDate}   To : ${toDate}`);
        getData(selector, fromDate, toDate);
        openCustomeDateModal.hide();
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

  // Real-time validation on date input change
  $("#FromDateInput, #ToDateInput").on("change", function () {
    validateDates();
  });

  // function for fetching data from the server
  function getData(selector, fromDate, toDate) {
    $.ajax({
      url: "/Users/UserDashboardData",
      type: "GET",
      data: {
        selector : selector,
        fromDate : fromDate,
        toDate : toDate,
      },
      success: function (data) {
        if (data.success) {
          console.log(data.data);
          setData(data.data);
        } else {
          toastr.error("Error fetching data:", data.message);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error fetching data:", error);
      },
    });
  }

  // function for setting data in the dashboard
  function setData(data) {
    console.log(data);
    $("#totalOrders").text(data.totalOrders);
    $("#totalSales").text("₹ " + data.totalSales);
    $("#avgOrderValue").text("₹ " + data.avgOrderValue);
    $("#waitingListCount").text(data.totalWaittingList);
    $("#avgWaitingTime").text(data.avgWaitingTime);
    $("#newCustomerCount").text(data.totalNewCustomer);
    if (data.lastItems) {
      fetchItems(data.lastItems, "#lastItems");
    }
    if (data.topItems) {
      fetchItems(data.topItems, "#topItems");
    }
    if (data.graphDataRevenue) {
      generateRevenueChart(data.graphDataRevenue);
    }
    if(data.graphDataCustomer){
        generateCustomerChart(data.graphDataCustomer);
    }
  }

  // function for fetching items and displaying them in the dashboard
  function fetchItems(items, elementId) {
    var itemsHtml = "";
    var count = 1;
    itemsHtml += "<ul>";
    $.each(items, function (index, item) {
      itemsHtml += `
            <li class="d-flex flex-row py-2">
                <div class="d-flex flex-row justify-content-start align-items-center w-100">
                    <p class="me-3">${count}</p>
                    <img class="border border-1 rounded-circle my-auto ms-2" src="${item.image}" height="70" width="70" alt="${item.itemName}">
                    <div class="pt-2 d-flex flex-column justify-content-start align-items-start ms-3">
                        <h5 class="mb-1">${item.itemName}</h5>
                        <h5 class="mb-0"><i class="bi bi-cake-fill"></i> ${item.count}</h5>
                    </div>
                </div>
            </li>
        `;

      count++;
    });
    itemsHtml += "</ul>";
    $(elementId).html(itemsHtml);
  }

  // function for generating revenue chart
  function generateRevenueChart(data) {
    const dateNumbers = data.map((item) => item.dateNumber);
    const revenues = data.map((item) => item.revenue);
    chartGenerator("RevenueChart",dateNumbers,revenues,"Revenue");
  }

  // function for generating customer chart
  function generateCustomerChart(data) {
    const dateNumbers = data.map((item) => item.month);
    const customers = data.map((item) => item.numberOfCustomer);
    chartGenerator("CustomerChart",dateNumbers,customers,"Customers");
  }

  // function for generating charts
  function chartGenerator(chartFor,dateNumbers,revenues,labels) {
    new Chart(chartFor, {
      type: "line",
      data: {
        labels: dateNumbers,
        datasets: [
          {
            label: labels,
            borderColor: "rgb(75, 192, 192)",
            data: revenues,
            fill: true,
            backgroundColor: "rgba(75, 192, 192, 0.2)",
          },
        ],
      },
      options: {
        legend: { display: true },
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true,
              },
            },
          ],
        },
      },
    });
  }
  

  // Event handler for the selector change
  $(document).on('click','.selector' ,function () {
    selector = $(this).attr('data-value');
    var selectedText = $(this).text();
    $('#selectorText').text(selectedText);
    if(selector == 5)
    {
      openCustomeDateModal.show();
    }else{
      $('#CustomDateValues').text('');
      getData(selector,null,null);
    }
  });


  // Call getData when the page loads
  getData(null,null,null);
});
