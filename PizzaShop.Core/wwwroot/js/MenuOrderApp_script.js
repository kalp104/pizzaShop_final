$(document).ready(function () {
    toastr.options.closeButton = true;
    var tempItems = {};
    var selectedItems = {};
    var itemCounter = 0;
    var OrderWiseComment = "";
    var orderComment = false;
    var orderid = 0;
    var tableid = 0;
    var tableStatus = 0;
    var customerid = 0;
    var paymentMode = 1;
    var totalAmount = 0;
    var category;
    var feedbackRatings = {
        food: 0,
        service: 0,
        ambience: 0
    };

    var searchingterm = "";


    tableid = $("#pageloading").data('tableid');
    console.log('tableid:', tableid);
    if (tableid) {
        $.ajax({
            url: "/OrderApp/GetOrderPageDetails",
            method: "GET",
            data: { tableId: tableid },
            success: function (data) {
                $("#OrderPagePartial").html(data);
                orderid = $('.OrderPage').data('order-id') || 0;
                tableid = $('.OrderPage').data('table-id') || 0;
                tableStatus = $('.OrderPage').data('table-status') || 0;
                customerid = $('.OrderPage').data('customer-id') || 0;

                if (tableStatus == 3) {
                    FetchItemsDetails(orderid);
                }
                console.log('status id :',tableStatus, 'orderid:', orderid, 'tableid:', tableid, 'customerid:', customerid); 
            },
            error: function (xhr, status, error) {
                console.error("Error loading order page details:", error, xhr.responseText); 
            }
        });
    } else {
        console.warn("No tableId found in #pageloading. AJAX call skipped."); 
    }



    function FetchItemsDetails(orderid) {
        $.ajax({
            url: "/OrderApp/GetOrderDataIfExists",
            method: "GET",
            data: { orderId: orderid },
            success: function (data) {
                if (data.success && data.data) {
                    //console.log("Order data loaded:", JSON.stringify(data.data, null, 2));
                    $("#CompleteOrder").prop("disabled", false);
                    $("#OrderDetailpdf").prop("disabled", false);
                    // Clear existing selectedItems to avoid duplicates
                    selectedItems = {};
                    itemCounter = 0;

                    // Map server items to selectedItems
                    if (data.data.items && data.data.items.length > 0) {
                        data.data.items.forEach(function (item) {
                            var uniqueKey = `item_${itemCounter++}`;
                            selectedItems[uniqueKey] = {
                                itemId: item.itemId,
                                itemName: item.itemName,
                                itemRate: parseFloat(item.itemRate),
                                totalItems: item.totalItems,
                                comment: item.comment || "",
                                modifiers: item.modifiers ? item.modifiers.map(function (m) {
                                    return {
                                        modifierId: m.modifierId,
                                        modifierName: m.modifierName,
                                        modifierRate: parseFloat(m.modifierRate)
                                    };
                                }) : [],
                                modifierGroups: {}
                            };
                        });
                    }

                    // Update global variables
                    OrderWiseComment = data.data.orderWiseComment || OrderWiseComment;
                    totalAmount = parseFloat(data.data.totalAmount) || totalAmount;
                    paymentMode = data.data.paymentMode || paymentMode;

                    // Log tax rows
                    console.log(`Found ${$("#taxSection .tax-row").length} tax rows`);

                    // Save checkbox states to restore after updateOrderList
                    var checkboxStates = {};
                    $("#taxSection .tax-row").each(function () {
                        var taxId = parseInt($(this).data("tax-id"));
                        checkboxStates[taxId] = $(this).find(".tax-checkbox").is(":checked");
                    });

                    // Update taxes in the UI
                    if (data.data.taxes && data.data.taxes.length > 0) {
                        $("#taxSection .tax-row").each(function () {
                            var $row = $(this);
                            var taxId = parseInt($row.data("tax-id"));
                            var tax = data.data.taxes.find(t => t.taxId === taxId);
                            if (tax) {
                                console.log(`Tax row: taxId=${taxId}, data=${JSON.stringify(tax)}`);
                                var $checkbox = $row.find(".tax-checkbox");
                                if ($checkbox.length) {
                                    // Check if tax is in OrderTaxMapping (present in data.taxes)
                                    const isChecked = tax.isChecked === true || tax.isChecked === null; // Fallback for current server
                                    $checkbox.prop("checked", isChecked);
                                    checkboxStates[taxId] = isChecked;
                                    console.log(`Setting checkbox for taxId=${taxId}, checked=${isChecked}`);
                                }
                                $row.find(".calculated-tax").text(`₹${tax.taxAmount.toFixed(2)}`);
                            } else {
                                console.warn(`No tax data for taxId=${taxId}`);
                                var $checkbox = $row.find(".tax-checkbox");
                                if ($checkbox.length) {
                                    $checkbox.prop("checked", false);
                                    checkboxStates[taxId] = false;
                                }
                            }
                        });
                    } else {
                        console.warn("No taxes in server response");
                        $("#taxSection .tax-row .tax-checkbox").prop("checked", false);
                        $("#taxSection .tax-row").each(function () {
                            var taxId = parseInt($(this).data("tax-id"));
                            checkboxStates[taxId] = false;
                        });
                    }

                    // Update payment mode radio buttons
                    if (paymentMode === 1) {
                        $("#Cash").prop("checked", true);
                    } else if (paymentMode === 2) {
                        $("#Card").prop("checked", true);
                    } else if (paymentMode === 3) {
                        $("#UPI").prop("checked", true);
                    }

                    // Update the UI and restore checkbox states
                    updateOrderList();
                    $("#taxSection .tax-row").each(function () {
                        var taxId = parseInt($(this).data("tax-id"));
                        var $checkbox = $(this).find(".tax-checkbox");
                        if ($checkbox.length && checkboxStates[taxId] !== undefined) {
                            $checkbox.prop("checked", checkboxStates[taxId]);
                            console.log(`Restored checkbox state: taxId=${taxId}, checked=${checkboxStates[taxId]}`);
                        }
                    });

                    console.log("Order data loaded successfully.");
                } else {
                    console.warn("No order data found:", data.message);
                    $("#taxSection .tax-row .tax-checkbox").prop("checked", false);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching order data:", error, xhr.responseText);
                toastr.error("Failed to load order data.", "Error", { timeOut: 3000 });
            }
        });
    }



    function fetchItems(category,searchingterm) {
        $("#loadingSpinner").show();
        $.ajax({
            url: "/OrderApp/GetItemsByCategoryId",
            method: "GET",
            data: { category: category,
                    searchTerm : searchingterm,
             },
            success: function (data) {
                $("#CardContainer").html(data);
                $("#loadingSpinner").hide();
            },
            error: function () {
                console.error("Error loading items.");
                $("#loadingSpinner").hide();
            },
        });
    }
    
    $(document).on("input","#searchInputHere", function(){
        var searchingterm = $(this).val();
        fetchItems(category,searchingterm);
    })

    $(document).on("change", "input[name='status']", function () {
        if ($(this).attr("id") === "Cash") {
            paymentMode = 1;
        } else if ($(this).attr("id") === "Card") {
            paymentMode = 2;
        } else if ($(this).attr("id") === "UPI") {
            paymentMode = 3;
        }
        // console.log("Payment Mode Updated:", paymentMode);
    });

    $(".categoryLink").on("click", function () {
        $(".categoryLink").removeClass("active");
        $(this).addClass("active");
        category = $(this).data("category-id");
        $("#searchInputHere").val("");
        fetchItems(category,"");
    });

    $(document).on("click", ".itemCards", function () {
        var itemId = $(this).data("item-id");
        var itemName = $(this).data("item-name");
        var itemRate = parseFloat(
            $(this).find("h6").text().replace("₹", "").trim()
        );
        var uniqueKey = `item_${itemCounter++}`;

        tempItems[uniqueKey] = {
            itemId: itemId,
            itemName: itemName,
            itemRate: itemRate,
            totalItems: 1,
            comment: "",
            modifierGroups: {} // Store modifiers by group
        };

        $("#ItemNameId").text(itemName);
        $.ajax({
            url: "/OrderApp/GetModifiers",
            method: "GET",
            data: { itemid: itemId },
            success: function (data) {
                $("#modifiersData").html(data);
                $("#modifiersAtMenu").data("current-item-key", uniqueKey);
                initializeModifiers(uniqueKey);
                var modal = new bootstrap.Modal(
                    document.getElementById("modifiersAtMenu")
                );
                modal.show();
            },
            error: function () {
                console.error("Error loading modifiers.");
            },
        });
    });

    // Modified initializeModifiers to ensure button state is set
    function initializeModifiers(uniqueKey) {
        var item = tempItems[uniqueKey];
        if (!item) {
            console.error("Item not found for uniqueKey:", uniqueKey);
            return;
        }
    
        $('.modifiersCard').each(function () {
            var $card = $(this);
            var $header = $card.closest('.cardContainer').prev('.MGheader');
            var modifierGroupName = $header.find('h5').text().replace(' Options', '');
            var modifierId = $card.data('modifier-id');
    
            if (item.modifierGroups[modifierGroupName] &&
                item.modifierGroups[modifierGroupName].modifiers.some(m => m.modifierId === modifierId)) {
                $card.addClass('active');
            } else {
                $card.removeClass('active');
            }
        });
    
        // Ensure Save button state is updated when modal opens
        updateSaveButtonState(uniqueKey);
    }

    
    // Modified modifiersCard click handler
    $(document).on("click", ".modifiersCard", function () {
        var $card = $(this);
        var uniqueKey = $("#modifiersAtMenu").data("current-item-key");
        var item = tempItems[uniqueKey];
        if (!item) {
            console.error("Item not found for uniqueKey:", uniqueKey);
            return;
        }
    
        var $header = $card.closest('.cardContainer').prev('.MGheader');
        var modifierGroupName = $header.find('h5').text().replace(' Options', '');
        var minValue = parseInt($header.data('min-value')) || 0;
        var maxValue = parseInt($header.data('max-value')) || Infinity;
        var modifierId = $card.data('modifier-id');
        var modifierName = $card.data('modifier-name');
        var modifierRate = parseFloat($card.data('modifier-rate'));
    
        // Initialize modifier group if not present
        if (!item.modifierGroups[modifierGroupName]) {
            item.modifierGroups[modifierGroupName] = {
                minValue: minValue,
                maxValue: maxValue,
                modifiers: []
            };
        }
    
        var group = item.modifierGroups[modifierGroupName];
        var selectedModifiers = group.modifiers;
        var isSelected = $card.hasClass('active');
    
        if (isSelected) {
            // Remove modifier
            group.modifiers = selectedModifiers.filter(m => m.modifierId !== modifierId);
            $card.removeClass('active');
        } else {
            // Check maxValue before adding
            if (selectedModifiers.length >= maxValue) {
                toastr.warning(`Maximum ${maxValue} modifiers allowed for ${modifierGroupName}`);
                return;
            }
            // Add modifier
            selectedModifiers.push({
                modifierId: modifierId,
                modifierName: modifierName,
                modifierRate: modifierRate
            });
            $card.addClass('active');
        }
    
        // Update Save button state
        updateSaveButtonState(uniqueKey);
    });

    $(document).on("click","#saveModifiersButton", function () {
        var uniqueKey = $("#modifiersAtMenu").data("current-item-key");
        var item = tempItems[uniqueKey];
        if (!item) return;

        // Validate minValue for each modifier group
        for (var groupName in item.modifierGroups) {
            var group = item.modifierGroups[groupName];
            if (group.modifiers.length < group.minValue) {
                toastr.error(`Please select at least ${group.minValue} modifiers for ${groupName}`);
                return;
            }
        }

        // Migrate modifiers to flat array for compatibility
        item.modifiers = [];
        for (var groupName in item.modifierGroups) {
            item.modifierGroups[groupName].modifiers.forEach(function (m) {
                item.modifiers.push(m);
            });
        }

        // Move to selectedItems and update UI
        selectedItems[uniqueKey] = item;
        delete tempItems[uniqueKey];
        updateOrderList();
        $("#modifiersAtMenu").modal("hide");
    });

    $(document).on("click",".customerDetailsBtn", function () {
        var modal = new bootstrap.Modal(document.getElementById("customerDetailsModal"));
        modal.show();
    });

    $(document).on("click",".QRCodeBtn", function () {
        var modal = new bootstrap.Modal(document.getElementById("QRCode"));
        modal.show();
    });

    $(document).on("click","#CompleteOrder", function () {
        // console.log("clicked");
        $.ajax({
            url: '/OrderApp/CheckStateOfItems',
            method: 'GET',
            contentType: 'application/json',
            data: {
                orderid: orderid,
            },
            success: function (response) {
                if(response.success)
                {
                    var modal = new bootstrap.Modal(document.getElementById("CompleteOrderModal"));
                    modal.show();
                }else{
                    toastr.error("Order items are not ready yet", "Error", { timeOut: 3000 });
                    
                }
            },
            error: function (xhr, status, error) {
                console.error("Error completing order:", error);
                // alert("Failed to complete order. Check console for details.");
            }
        });
       
    });

    $(document).on("click","#feedback", function () {
        // console.log("clicked");
        var modal = new bootstrap.Modal(document.getElementById("Feedbackmodal"));
        modal.show();
    });

    // Handle star click
    $(document).on("click", ".star-rating .star", function () {
        var $star = $(this);
        var value = parseInt($star.data("value"));
        var field = $star.closest(".star-rating").data("field");

        // Update rating for the field
        feedbackRatings[field] = value;

        // Update star visuals
        $star.closest(".star-rating").find(".star").each(function () {
            var starValue = parseInt($(this).data("value"));
            if (starValue <= value) {
                $(this).removeClass("bi-star").addClass("bi-star-fill").css("color", "#ffc107");
            } else {
                $(this).removeClass("bi-star-fill").addClass("bi-star").css("color", "");
            }
        });

        // console.log("Updated Ratings:", feedbackRatings);
    });

    $(document).on("click","#saveFeedbackButton", function () {
        var comment = $("#FeedbackComment").val().trim();
        var feedbackData = {
            foodRating: feedbackRatings.food,
            serviceRating: feedbackRatings.service,
            ambienceRating: feedbackRatings.ambience,
            comment: comment
        };

        // console.log("Feedback Data:", feedbackData);

        // Complete order with feedback
        $.ajax({
            url: '/OrderApp/CompleteOrder',
            method: 'GET',
            contentType: 'application/json',
            data: {
                orderid: orderid,
                feedback: JSON.stringify(feedbackData)
            },
            success: function (response) {
                if (response.status == 1) {
                    toastr.success("Order completed successfully!", "Success", { timeOut: 3000 });
                    window.location.href = "/OrderApp/Tables";
                } else {
                    toastr.error("Order items are not ready yet", "Error", { timeOut: 3000 });
                }
            },
            error: function (xhr, status, error) {
                console.error("Error completing order:", error);
                alert("Failed to complete order. Check console for details.");
            }
        });

        // Clear ratings and comment
        feedbackRatings.food = 0;
        feedbackRatings.service = 0;
        feedbackRatings.ambience = 0;
        $("#FeedbackComment").val("");
        $(".star-rating .star").removeClass("bi-star-fill").addClass("bi-star").css("color", "");

        $("#Feedbackmodal").modal("hide");
    });


    function updateOrderList() {
        var $list = $('#selectedItemsList');
        $list.empty();
        var subTotal = 0;
    
        for (var uniqueKey in selectedItems) {
            var item = selectedItems[uniqueKey];
            var itemTotal = item.itemRate * item.totalItems;
            var modifiersHtml = '';
            var modifiersTotal = 0;
    
            item.modifiers.forEach(function (m) {
                var modifierCost = m.modifierRate * item.totalItems;
                modifiersTotal += modifierCost;
                modifiersHtml += `<li class="small list-group-item border-0"><i class="bi bi-dot me-2"></i> ${m.modifierName} ₹${modifierCost.toFixed(2)}</li>`;
            });
    
            $list.append(`
                <li class="list-group-item" data-item-key="${uniqueKey}">
                    <div class="d-flex justify-content-between align-items-center">
                        <div class="d-flex gap-3" data-item-key="${uniqueKey}">
                            <button class="border-0 bg-transparent dropDownBtn" data-item-key="${uniqueKey}"><i class="bi bi-chevron-down"></i></button>
                            <div class="commentForItem" data-item-key="${uniqueKey}">${item.itemName}</div>
                        </div>
                        <div class="d-flex gap-3 align-items-center justify-content-center flex-column flex-md-row">
                            <div class="d-flex align-items-center justify-content-center">
                                <div class="border border-1 m-1 d-flex align-items-center justify-content-center">
                                    <button class="btn minus-btn" data-item-key="${uniqueKey}">-</button>
                                    <span class="mx-2 text-center">${item.totalItems}</span>
                                    <button class="btn plus-btn" data-item-key="${uniqueKey}">+</button>
                                </div>
                            </div>
                            <div class="d-flex flex-column">
                                <span class="ms-1">₹${(item.itemRate * item.totalItems).toFixed(2)}</span>
                                ${modifiersTotal > 0 ? `<span class="ms-2 text-muted">₹${modifiersTotal.toFixed(2)}</span>` : ''}
                            </div>
                            <div>
                                <button class="btn trash-btn" data-item-key="${uniqueKey}"><i class="bi bi-trash"></i></button>
                            </div>
                        </div>
                    </div>
                    <div>
                        <ul class="list-group list-group-flush ms-2 dropdownContainer" data-item-key="${uniqueKey}">
                            ${modifiersHtml}
                        </ul>
                    </div>
                </li>
            `);
            subTotal += itemTotal + modifiersTotal;
        }

        console.log(selectedItems);
    
        $('#subTotal').text('₹ ' + subTotal.toFixed(2));
    
        // Calculate total with enabled or checked taxes
        var total = subTotal;
        $("#taxSection .tax-row").each(function () {
            var $row = $(this);
            var taxAmount = parseFloat($row.data("tax-amount"));
            var taxType = parseInt($row.data("tax-type"));
            var isEnabledValue = $row.data("tax-enabled");
            console.log("isEnabledValue:", isEnabledValue);
            var isEnabled = isEnabledValue == 'True' ? true : false;
            var $checkbox = $row.find(".tax-checkbox");
            var isChecked = $checkbox.length ? $checkbox.is(":checked") : false;
    
            var taxValue = taxType === 1 ? (subTotal * taxAmount) / 100 : taxAmount;
    
            if (isEnabled || isChecked) {
                $row.find(".calculated-tax").text(`₹${taxValue.toFixed(2)}`);
            } else {
                $row.find(".calculated-tax").text(`₹0.00`);
            }
    
            if (taxType === 1 && (isEnabled || isChecked)) {
                total += taxValue;
            } else if (taxType === 2 && (isEnabled || isChecked)) {
                total += taxAmount;
            }
            totalAmount = total;
        });
    
        $('#total').text('₹ ' + total.toFixed(2));
    }


    $(document).on("click", ".plus-btn", function () {
        var uniqueKey = $(this).data("item-key");
        selectedItems[uniqueKey].totalItems += 1;
        updateOrderList();
    });

    $(document).on("click", ".minus-btn", function () {
        var uniqueKey = $(this).data("item-key");
        if (selectedItems[uniqueKey].totalItems > 1) {
            selectedItems[uniqueKey].totalItems -= 1;
        } else {
            delete selectedItems[uniqueKey];
        }
        updateOrderList();
    });

    $(document).on("click", ".trash-btn", function () {
        var uniqueKey = $(this).data("item-key");
        // console.log("clicked on trash, unique key is", uniqueKey);
        delete selectedItems[uniqueKey];
        updateOrderList();
    });

    $(document).on("click", ".dropDownBtn", function (e) {
        e.preventDefault();
        var uniqueKey = $(this).data("item-key");
        var $dropdown = $(this)
            .closest("li")
            .find('div > ul.dropdownContainer[data-item-key="' + uniqueKey + '"]');
        if ($dropdown.length) {
            $dropdown.slideToggle(300);
        } else {
            console.error("Dropdown not found for key:", uniqueKey);
        }
    });

    $(document).on("change", ".tax-checkbox", function () {
        updateOrderList();
    });

    
    // item vise comments
    $(document).on("click", ".commentForItem", function (e) {
        e.stopPropagation();
        var uniqueKey = $(this).data("item-key");
        var item = selectedItems[uniqueKey];
    
        $("#CommentHeader").text("Special Instruction");
        $("#itemComment").val(item.comment || "");
    
        $("#CommentForItemModal").data("current-item-key", uniqueKey);
    
        var modal = new bootstrap.Modal('#CommentForItemModal', {
            backdrop: 'static',
            keyboard: false
        });
        modal.show();
    });
    
    // Add order-wise instruction
    $(document).on('click', ".OrderWiseComment", function (e) {
        e.stopPropagation();
        $("#itemComment").val(OrderWiseComment);
        $("#CommentHeader").text("Order Wise Comment");
        orderComment = true;
    
        var modal = new bootstrap.Modal('#CommentForItemModal', {
            backdrop: 'static',
            keyboard: false
        });
        modal.show();
    });
    
    // Save comment from modal
    $("#saveCommentButton").on("click", function () {
        if (orderComment) {
            OrderWiseComment = $("#itemComment").val().trim();
            console.log("Order Wise Comment:", OrderWiseComment);
            updateOrderList();
            orderComment = false;
        } else {
            var uniqueKey = $("#CommentForItemModal").data("current-item-key");
            var comment = $("#itemComment").val().trim();
            $("#itemComment").val('');
            if (uniqueKey && selectedItems[uniqueKey]) {
                selectedItems[uniqueKey].comment = comment;
                updateOrderList();
            }
        }
    
        // Hide the modal
        var modal = bootstrap.Modal.getInstance('#CommentForItemModal');
        if (modal) {
            modal.hide();
        }
    });

    // Save Order Button Click Handler
    $(document).on('click','#SaveOrderBtn', function () {
        // console.log("save button clicked");
        var orderData = {
            OrderWiseComment: OrderWiseComment,
            TotalAmount: totalAmount,
            OrderId: orderid,
            CustomerId: customerid,
            TableId: tableid,
            PaymentMode: paymentMode,
            items: [],
            taxes: []
        };

        for (var uniqueKey in selectedItems) {
            var item = selectedItems[uniqueKey];
            orderData.items.push({
                itemId: item.itemId,
                itemName: item.itemName,
                itemRate: item.itemRate,
                totalItems: item.totalItems,
                comment: item.comment || "",
                modifiers: item.modifiers.map(m => ({
                    modifierId: m.modifierId,
                    modifierName: m.modifierName,
                    modifierRate: m.modifierRate
                }))
            });
        }

        var subTotal = parseFloat($('#subTotal').text().replace('₹', '').trim());
        $("#taxSection .tax-row").each(function () {
            var $row = $(this);
            var taxId = parseInt($row.data("tax-id"));
            var taxAmount = parseFloat($row.data("tax-amount"));
            var taxType = parseInt($row.data("tax-type"));
            var isEnabledValue = $row.data("tax-enabled");
            var isEnabled = isEnabledValue === 'True' || isEnabledValue === true;
            var $checkbox = $row.find(".tax-checkbox");
            var isChecked = $checkbox.length ? $checkbox.is(":checked") : true;

            var taxValue = taxType === 1 ? (subTotal * taxAmount) / 100 : taxAmount;

            if (isChecked) {
                orderData.taxes.push({
                    taxId: taxId,
                    Isenabled: isEnabled,
                    isChecked: isChecked,
                    taxAmount: taxAmount,
                    taxType: taxType,
                    taxValue: taxValue
                });
            }
        });

        // console.log("Order Data to Send:", orderData);

        $.ajax({
            url: '/OrderApp/SaveOrder',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(orderData),
            success: function (response) {
                if (response.success) {
                    $("#CompleteOrder").prop("disabled", false);
                    $("#OrderDetailpdf").prop("disabled", false);

                    toastr.success("Order saved successfully!", "Success", { timeOut: 3000 });
                }else{
                    
                    FetchItemsDetails(orderid);
                    
                    console.error("Error saving order:", response.message, orderid);
                    toastr.error(response.message, "Error", { timeOut: 3000 });  
                }
            },
            error: function (xhr, status, error) {
                console.error("Error saving order:", error);
            }
        });
    });

    // Cancel order button
    $(document).on("click",'#CancelOrderBtn', function(){
        var modal = new bootstrap.Modal(document.getElementById("CancelOrderModal"));
        modal.show();
    });

    // Generate PDF
    $(document).on("click", "#OrderDetailpdf", function (e) {
        e.preventDefault();
        // console.log("clicked order id:", orderid);
        window.location.href = "/Order/GenerateInvoice?orderId=" + orderid;
        toastr.success("pdf invoice file downloaded successfully.", "Success", {
            timeOut: 3000,
        });
    });

    // On clicking cancel in modal
    $(document).on("click",'#CancelOrder', function () {
        orderid = $('.OrderPage').data('order-id') || 0;

        $.ajax({
            url: '/OrderApp/CancelOrder',
            method: 'GET',
            contentType: 'application/json',
            data: { OrderId: orderid },
            success: function (response) {
                if (response.status == false) {
                    toastr.error("The order item is ready, cannot cancel the order", "Error", { timeOut: 3000 });
                } else {
                    window.location.href = "/OrderApp/Tables";
                }
            },
            error: function (xhr, status, error) {
                console.error("Error canceling order:", error);
                alert("Failed to cancel order. Check console for details.");
            }
        });
    });

    $(document).on("click", "#favouriteBtn", function(e) {
        e.stopPropagation();
        var itemid = $(this).data("item-id");
        $.ajax({
            url: '/OrderApp/AddFavourite',
            method: 'GET',
            contentType: 'application/json',
            data: { itemid: itemid },
            success: function (response) {
                if (response.status == false) {
                    toastr.error("Error adding to favourites", "Error", { timeOut: 3000 });
                } else {
                    $("#searchInputHere").val("");
                    fetchItems(category,"");
                    $(this).find("i").toggleClass("bi-heart bi-heart-fill");
                    toastr.success("Changes successfully done!", "Success", { timeOut: 3000 });
                }
            },
            error: function (error) {
                console.error("Error adding favourite:", error);
                alert("Failed to add favourite. Check console for details.");
            }
        });
    });



    $(document).on("submit", "#EditCustomerDetails", function(e) {
        e.preventDefault();
        var form = $(this);
        if(form.valid())
        {
            var formData = form.serialize();
            $.ajax({
                url: '/OrderApp/EditCustomer',
                method: 'GET',
                contentType: 'application/json',
                data: formData,
                success: function (response) {
                if (response.success == false) {
                    toastr.error(response.message, "Error", { timeOut: 3000 });
                } else {
                    var modal = bootstrap.Modal.getInstance("#customerDetailsModal");
                    modal.hide();
                    toastr.success("Changes successfully done!", "Success", { timeOut: 3000 });
                }
                },
                error: function (error) {
                    console.error("Error adding favourite:", error);
                    alert("Failed to add favourite. Check console for details.");
                }
            });
        }
    });



    function updateSaveButtonState(uniqueKey) {
        var item = tempItems[uniqueKey];
        if (!item) {
            $("#saveModifiersButton").prop("disabled", true);
            return;
        }
     
        // Count total selected modifiers across all modifier groups
        var totalModifiers = 0;
        for (var groupName in item.modifierGroups) {
            totalModifiers += item.modifierGroups[groupName].modifiers.length;
        }
     
        // Enable Save button if at least one modifier is selected
        $("#saveModifiersButton").prop("disabled", totalModifiers === 0);
        console.log(`Total modifiers selected: ${totalModifiers}, Save button disabled: ${totalModifiers === 0}`);
    }


    fetchItems(null,"");
});