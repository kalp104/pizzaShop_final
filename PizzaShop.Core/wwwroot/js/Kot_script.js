$(document).ready(function () {
  toastr.options.closeButton = true;
  var category;
  var status = 1;

  var connection = new signalR.HubConnectionBuilder()
    .withUrl("/OrderAppHub")
    .build();
  // SignalR connection for real-time updates
  connection
    .start()
    .then(function () {
      console.log("SignalR connection established for KOT updates.");
    })
    .catch(function (err) {
      return console.error(
        "Error establishing SignalR connection:",
        err.toString()
      );
    });

  connection.on("ReceiveOrderApp", function () {
    console.log("UpdateKOT received:");
    fetchCards(null, 1).then(() => {
      console.log("Cards fetched successfully.");
    }).catch((err) => {
      console.error("Error fetching cards:", err);
    });
  });



  function fetchCards(category, status) {
    $.ajax({
      url: "/OrderApp/GetCardDetails",
      method: "GET",
      data: { category: category, status: status, IsModal: false },
      success: function (data) {
        $("#CardKotContainer").html(data);
      },
      error: function (e) {
        console.error("Error loading Cards:", e);
        toastr.error("Error in fetching cards at KOT");
      },
    });
  }

  $(".KOTLink").on("click", function () {
    $(".KOTLink").removeClass("active");
    $(this).addClass("active");
    var categoryName = $(this).attr("data-category-name");
    category = $(this).attr("data-category-id");
    $(".categoryName").text(categoryName);
    fetchCards(category, status);
  });

  $(".prgressLink").on("click", function () {
    $(".prgressLink").removeClass("active");
    $(this).addClass("active");
    status = $(this).data("status");
    fetchCards(category, status);
  });

  $("#CardKotContainer").on("click", ".cardContainerKOT", function () {
    $("#KOTModal").modal("show");

    var orderid = $(this).find(".text-primary").text().replace("# ", "");
    $.ajax({
      url: "/OrderApp/GetModalDetails",
      method: "GET",
      data: { orderid: orderid, status: status, IsModal: true },
      success: function (data) {
        $("#KOTModal .modal-content").html(data);
        if (status == 1) {
          $(".mark-prepared-btn").text("Mark as Ready");
          $("#StatusNameLabel").text("Mark for Reday");
        } else {
          $(".mark-prepared-btn").text("Mark in process");
          $("#StatusNameLabel").text("Mark in process");
        }
      },
      error: function (e) {
        console.error("Error loading modal:", e);
        toastr.error("Error in fetching modal details");
      },
    });
  });

  // Quantity controls in modal
  $("#KOTModal").on("click", ".decrease-btn", function () {
    var orderItemMappingId = $(this).data("order-item-mapping-id");
    var $quantity = $(
      `.quantity-unit[data-order-item-mapping-id="${orderItemMappingId}"]`
    );
    var current = parseInt($quantity.text());
    if (current > 0) {
      $quantity.text(current - 1);
      $(
        `.quantity-hidden[data-order-item-mapping-id="${orderItemMappingId}"]`
      ).val(current - 1);
    }
  });

  $("#KOTModal").on("click", ".increase-btn", function () {
    var orderItemMappingId = $(this).data("order-item-mapping-id");
    var status = $(this).data("status");
    var max = 0;
    if (status == 1) {
      max = parseInt($(this).data("max"));
    } else {
      max = parseInt($(this).data("ready-quantity"));
    }
    var $quantity = $(
      `.quantity-unit[data-order-item-mapping-id="${orderItemMappingId}"]`
    );
    var current = parseInt($quantity.text());
    if (current < max) {
      $quantity.text(current + 1);
      $(
        `.quantity-hidden[data-order-item-mapping-id="${orderItemMappingId}"]`
      ).val(current + 1);
    }
  });

  $("#KOTModal").on("change", ".item-check", function () {
    var orderItemMappingId = $(this).data("order-item-mapping-id");
    var Status = $(this).data("status");
    var total = 0;
    if (Status == 1) {
      total = parseInt($(this).data("total"));
    } else {
      total = parseInt($(this).data("ready-quantity"));
    }

    console.log(total);
    var $quantity = $(
      `.quantity-unit[data-order-item-mapping-id="${orderItemMappingId}"]`
    );
    if ($(this).is(":checked")) {
      $quantity.text(total);
      $(
        `.quantity-hidden[data-order-item-mapping-id="${orderItemMappingId}"]`
      ).val(total);
    } else {
      $quantity.text(0);
      $(
        `.quantity-hidden[data-order-item-mapping-id="${orderItemMappingId}"]`
      ).val(0);
    }
  });

  $("#KOTModal").on("click", ".mark-prepared-btn", function () {
    var orderId = $(this).data("order-id");
    var formData = $("#kotForm").serializeArray();
    var status = $(this).data("modal-status");
    console.log(
      "__________status_______________",
      status,
      $(this).data("modal-status")
    );
    console.log("__________status_______________", formData);
    var updates = [];
    $(".quantity-unit").each(function () {
      var orderItemMappingId = $(this).data("order-item-mapping-id");
      var readyQuantity = parseInt($(this).text()) || 0;
      updates.push({
        orderItemMappingId: parseInt(orderItemMappingId),
        readyQuantity: readyQuantity,
      });
    });

    $.ajax({
      url: "/OrderApp/UpdateReadyQuantities",
      method: "POST",
      data: { orderId: orderId, updates: updates, Status: status },
      success: function () {
        $("#KOTModal").modal("hide");
        $(".modal-backdrop").remove();
        fetchCards(category, status);
        toastr.success("Items marked as prepared", "success", {
          setTimeout: 2000,
        });
      },
      error: function (e) {
        console.error("Error updating quantities:", e);
        toastr.error("Error updating quantities");
      },
    });
  });

  $("#LeftScroll").on("click", function () {
    KOTLoadData("left");
  });
  $("#RightScroll").on("click", function () {
    KOTLoadData("right");
  });
  function KOTLoadData(direction) {
    const data = document.querySelector("#CardKotContainer");
    const scroll = 650;
    if (direction === "left") {
      data.scrollBy({ left: -scroll, behavior: "smooth" });
    } else {
      data.scrollBy({ left: scroll, behavior: "smooth" });
    }
  }

  fetchCards(null, 1);
});
