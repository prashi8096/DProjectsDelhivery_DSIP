/* ============================================
   DSIP - Delhivery Shipment Intelligence Platform
   app.js
============================================ */

const API = "https://localhost:7113/api/Shipment";

$(document).ready(function () {

    loadShipments();
    loadStats();

    $("#btnBook").click(bookShipment);
    $("#btnTrack").click(trackShipment);

    $("#btnRefresh").click(function () {
        loadShipments();
        loadStats();
    });

    $("#filterStatus").change(filterShipments);

});


function showLoader() {
    $("#loader").addClass("active");
}

function hideLoader() {
    $("#loader").removeClass("active");
}

function showMessage(message) {

    $("#toast")
        .text(message)
        .addClass("show");

    setTimeout(function () {
        $("#toast").removeClass("show");
    }, 2500);

}


/* ============================================
        LOAD ALL SHIPMENTS
============================================ */

function loadShipments() {

    showLoader();

    $.ajax({

        url: API,
        type: "GET",

        success: function (shipments) {

            hideLoader();

            $("#shipmentBody").empty();

            if (shipments.length == 0) {

                $("#shipmentBody").append(
                    "<tr><td colspan='7'>No Shipments Found</td></tr>"
                );

                return;
            }

            $.each(shipments, function (index, shipment) {

                var row = `

<tr data-status="${shipment.status}">

<td>${shipment.awbNumber}</td>

<td>${shipment.senderName}</td>

<td>${shipment.receiverName}</td>

<td>${shipment.origin} → ${shipment.destination}</td>

<td>${shipment.weightKg} Kg</td>

<td>

<select onchange="updateStatus('${shipment.awbNumber}',this.value)">

<option value="Booked"
${shipment.status=="Booked"?"selected":""}>
Booked
</option>

<option value="InTransit"
${shipment.status=="InTransit"?"selected":""}>
In Transit
</option>

<option value="OutForDelivery"
${shipment.status=="OutForDelivery"?"selected":""}>
Out For Delivery
</option>

<option value="Delivered"
${shipment.status=="Delivered"?"selected":""}>
Delivered
</option>

<option value="RTO"
${shipment.status=="RTO"?"selected":""}>
RTO
</option>

</select>

</td>

<td>

<button
class="btn btn-danger"
onclick="cancelShipment(${shipment.shipmentId})">

Cancel

</button>

</td>

</tr>

`;

                $("#shipmentBody").append(row);

            });

            filterShipments();

        },

        error: function () {

            hideLoader();

            alert("Unable to load shipments.");

        }

    });

}


/* ============================================
        LOAD STATS
============================================ */

function loadStats() {

    $.ajax({

        url: API + "/stats",

        type: "GET",

        success: function (stats) {

            $("#bookedCount").text(stats.booked);

            $("#inTransitCount").text(stats.inTransit);

            $("#outForDeliveryCount").text(stats.outForDelivery);

            $("#deliveredCount").text(stats.delivered);

            $("#rtoCount").text(stats.rto);

        }

    });

}


/* ============================================
        VALIDATE BOOKING FORM
        (returns true if valid, false if not)
============================================ */

function validateBookingForm(shipment) {

    // AWB Number - required, only letters/numbers, min 5 chars
    if (shipment.awbNumber == "") {
        alert("AWB Number is required.");
        return false;
    }

    var awbPattern = /^[A-Za-z0-9]{5,20}$/;
    if (!awbPattern.test(shipment.awbNumber)) {
        alert("AWB Number must be 5-20 letters/numbers only (no spaces or symbols).");
        return false;
    }

    // Sender Name - required, letters and spaces only
    if (shipment.senderName == "") {
        alert("Sender Name is required.");
        return false;
    }

    var namePattern = /^[A-Za-z ]{2,50}$/;
    if (!namePattern.test(shipment.senderName)) {
        alert("Sender Name must contain only letters and spaces (2-50 characters).");
        return false;
    }

    // Receiver Name - required, letters and spaces only
    if (shipment.receiverName == "") {
        alert("Receiver Name is required.");
        return false;
    }

    if (!namePattern.test(shipment.receiverName)) {
        alert("Receiver Name must contain only letters and spaces (2-50 characters).");
        return false;
    }

    // Origin - required
    if (shipment.origin == "") {
        alert("Origin is required.");
        return false;
    }

    // Destination - required
    if (shipment.destination == "") {
        alert("Destination is required.");
        return false;
    }

    // Origin and Destination cannot be the same
    if (shipment.origin.trim().toLowerCase() == shipment.destination.trim().toLowerCase()) {
        alert("Origin and Destination cannot be the same.");
        return false;
    }

    // Weight - required, must be a valid number within range
    if (isNaN(shipment.weightKg) || shipment.weightKg <= 0) {
        alert("Weight must be a number greater than 0.");
        return false;
    }

    if (shipment.weightKg > 1000) {
        alert("Weight cannot be more than 1000 Kg.");
        return false;
    }

    // All checks passed
    return true;

}


/* ============================================
        BOOK SHIPMENT
============================================ */

function bookShipment() {

    var shipment = {

        awbNumber: $("#awb").val().trim(),

        senderName: $("#sender").val().trim(),

        receiverName: $("#receiver").val().trim(),

        origin: $("#origin").val().trim(),

        destination: $("#destination").val().trim(),

        weightKg: parseFloat($("#weight").val())

    };

    // Run all validations before calling the API
    if (!validateBookingForm(shipment)) {
        return;
    }

    showLoader();

    $.ajax({

        url: API,

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(shipment),

        success: function () {

            hideLoader();

            showMessage("Shipment Booked Successfully");

            $("#awb").val("");
            $("#sender").val("");
            $("#receiver").val("");
            $("#origin").val("");
            $("#destination").val("");
            $("#weight").val("");

            loadShipments();

            loadStats();

        },

        error: function (xhr) {

            hideLoader();

            alert(xhr.responseText);

        }

    });

}


/* ============================================
        UPDATE STATUS
============================================ */

function updateStatus(awb, status) {

    showLoader();

    $.ajax({

        url: API + "/" + awb,

        type: "PUT",

        contentType: "application/json",

        data: JSON.stringify(status),

        success: function () {

            hideLoader();

            showMessage("Status Updated");

            loadShipments();

            loadStats();

        },

        error: function (xhr) {

            hideLoader();

            alert(xhr.responseText);

            loadShipments();

        }

    });

}


/* ============================================
        CANCEL SHIPMENT
============================================ */

function cancelShipment(id) {

    if (!confirm("Are you sure?"))
        return;

    showLoader();

    $.ajax({

        url: API + "/" + id,

        type: "DELETE",

        success: function () {

            hideLoader();

            showMessage("Shipment Cancelled");

            loadShipments();

            loadStats();

        },

        error: function (xhr) {

            hideLoader();

            alert(xhr.responseText);

        }

    });

}


/* ============================================
        FILTER SHIPMENTS
============================================ */

function filterShipments() {

    var status = $("#filterStatus").val();

    $("#shipmentBody tr").each(function () {

        if (
            status == "" ||
            $(this).attr("data-status") == status
        ) {

            $(this).show();

        }
        else {

            $(this).hide();

        }

    });

}


/* ============================================
        TRACK SHIPMENT
============================================ */

function trackShipment() {

    var awb = $("#trackAwb").val().trim();

    // Validate AWB before calling the API
    if (awb == "") {

        alert("Enter AWB Number");

        return;

    }

    var awbPattern = /^[A-Za-z0-9]{5,20}$/;
    if (!awbPattern.test(awb)) {

        alert("Enter a valid AWB Number (5-20 letters/numbers only).");

        return;

    }

    showLoader();

    $.ajax({

        url: API + "/" + awb,

        type: "GET",

        success: function (shipment) {

            hideLoader();

            $("#trackResult")
                .removeClass("hidden")
                .html(`

<h3>Shipment Details</h3>

<p><b>AWB :</b> ${shipment.awbNumber}</p>

<p><b>Sender :</b> ${shipment.senderName}</p>

<p><b>Receiver :</b> ${shipment.receiverName}</p>

<p><b>Origin :</b> ${shipment.origin}</p>

<p><b>Destination :</b> ${shipment.destination}</p>

<p><b>Weight :</b> ${shipment.weightKg} Kg</p>

<p><b>Status :</b> ${shipment.status}</p>

`);

        },

        error: function () {

            hideLoader();

            $("#trackResult")
                .removeClass("hidden")
                .html("<h3>Shipment Not Found</h3>");

        }

    });

}