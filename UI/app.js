/* =========================================================
   DSIP — Delhivery Shipment Intelligence Platform
   app.js  |  jQuery $.ajax() only — no fetch()
   ========================================================= */

const API = "https://localhost:7113/api/Shipment";

// Must match the C# ShipmentStatus enum names exactly
const VALID_STATUSES = ["Booked", "InTransit", "OutForDelivery", "Delivered", "RTO"];

// Human-readable labels for display
const STATUS_LABELS = {
    "Booked":          "Booked",
    "InTransit":       "In Transit",
    "OutForDelivery":  "Out for Delivery",
    "Delivered":       "Delivered",
    "RTO":             "RTO"
};

/* ── Loader & Toast helpers ──────────────────────────────── */

let _loaderCount = 0;
function showLoader() {
    _loaderCount++;
    $("#loader").addClass("active");
}
function hideLoader() {
    _loaderCount = Math.max(0, _loaderCount - 1);
    if (_loaderCount === 0) $("#loader").removeClass("active");
}

let _toastTimer = null;
function showToast(msg, type /* "success" | "error" */) {
    const $t = $("#toast");
    $t.text(msg).removeClass("success error").addClass(type).addClass("show");
    clearTimeout(_toastTimer);
    _toastTimer = setTimeout(() => $t.removeClass("show success error"), 3200);
}

/* ── DOM Ready ───────────────────────────────────────────── */

$(document).ready(function () {
    loadShipments();
    loadStats();

    $("#btnBook").on("click", bookShipment);
    $("#btnTrack").on("click", trackShipment);
    $("#btnRefresh").on("click", function () { loadShipments(); loadStats(); });
    $("#filterStatus").on("change", applyFilter);

    // Track on Enter key
    $("#trackAwb").on("keydown", function (e) {
        if (e.key === "Enter") trackShipment();
    });
});

/* ── Load All Shipments ──────────────────────────────────── */

function loadShipments() {
    showLoader();
    $.ajax({
        url: API,
        type: "GET",
        success: function (shipments) {
            renderTable(shipments);
            hideLoader();
        },
        error: function () {
            hideLoader();
            showToast("Could not load shipments. Is the API running?", "error");
            $("#shipmentBody").html(
                '<tr class="empty-row"><td colspan="7">Failed to load shipments.</td></tr>'
            );
        }
    });
}

/* ── Load Stats ──────────────────────────────────────────── */

function loadStats() {
    showLoader();
    $.ajax({
        url: API + "/stats",
        type: "GET",
        success: function (s) {
            hideLoader();
            $("#bookedCount").text(s.booked ?? 0);
            $("#inTransitCount").text(s.inTransit ?? 0);
            $("#outForDeliveryCount").text(s.outForDelivery ?? 0);
            $("#deliveredCount").text(s.delivered ?? 0);
            $("#rtoCount").text(s.rto ?? 0);
        },
        error: function () {
            hideLoader();
            // silently fail stats — main table failure is the primary signal
        }
    });
}

/* ── Render Table ────────────────────────────────────────── */

function renderTable(shipments) {
    const $body = $("#shipmentBody").empty();

    if (!shipments || shipments.length === 0) {
        $body.html('<tr class="empty-row"><td colspan="7">No shipments found.</td></tr>');
        return;
    }

    $.each(shipments, function (_, s) {
        // s.status comes back as the enum name e.g. "InTransit"
        const statusKey   = s.status || "Booked";
        const statusLabel = STATUS_LABELS[statusKey] || statusKey;
        const badgeClass  = "badge-" + statusKey;

        const $row = $(`
            <tr data-status="${escHtml(statusKey)}" data-id="${s.shipmentId}">
                <td><strong>${escHtml(s.awbNumber)}</strong></td>
                <td>${escHtml(s.senderName)}</td>
                <td>${escHtml(s.receiverName)}</td>
                <td class="route-cell">
                    ${escHtml(s.origin)}
                    <span class="route-arrow">→</span>
                    ${escHtml(s.destination)}
                </td>
                <td>${parseFloat(s.weightKg).toFixed(2)} kg</td>
                <td><span class="badge ${badgeClass}">${escHtml(statusLabel)}</span></td>
                <td></td>
            </tr>
        `);

        // Inline status dropdown — values are enum names, labels are human-readable
        const $select = $('<select class="status-select" aria-label="Update status"></select>');
        VALID_STATUSES.forEach(function (st) {
            const $opt = $("<option>").val(st).text(STATUS_LABELS[st] || st);
            if (st === statusKey) $opt.prop("selected", true);
            $select.append($opt);
        });
        $select.on("change", function () {
            updateStatus(s.awbNumber, $(this).val(), $row);
        });

        // Cancel button
        const $cancel = $('<button class="btn btn-danger" title="Cancel shipment">Cancel</button>');
        $cancel.on("click", function () {
            cancelShipment(s.shipmentId, $row);
        });

        const $actions = $('<div style="display:flex;gap:8px;align-items:center;"></div>')
            .append($select)
            .append($cancel);
        $row.find("td:last-child").append($actions);

        $body.append($row);
    });

    applyFilter();
}

/* ── Client-side Filter ──────────────────────────────────── */

function applyFilter() {
    const val = $("#filterStatus").val();
    $("#shipmentBody tr").each(function () {
        if (!val || $(this).data("status") === val) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

/* ── Book Shipment ───────────────────────────────────────── */

function bookShipment() {
    const awb    = $.trim($("#awb").val());
    const sender = $.trim($("#sender").val());
    const recv   = $.trim($("#receiver").val());
    const orig   = $.trim($("#origin").val());
    const dest   = $.trim($("#destination").val());
    const wt     = parseFloat($("#weight").val());

    if (!awb || !sender || !recv || !orig || !dest || isNaN(wt) || wt <= 0) {
        showToast("Please fill all fields. Weight must be greater than 0.", "error");
        return;
    }

    const payload = {
        awbNumber:    awb,
        senderName:   sender,
        receiverName: recv,
        origin:       orig,
        destination:  dest,
        weightKg:     wt
    };

    showLoader();
    $.ajax({
        url: API,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function () {
            hideLoader();
            showToast("Shipment " + awb + " booked successfully.", "success");
            $("#awb, #sender, #receiver, #origin, #destination, #weight").val("");
            loadShipments();
            loadStats();
        },
        error: function (xhr) {
            hideLoader();
            showToast(xhr.responseText || "Booking failed. Check inputs.", "error");
        }
    });
}

/* ── Update Status (inline) ──────────────────────────────── */

function updateStatus(awb, newStatus, $row) {
    showLoader();
    $.ajax({
        // Controller: [HttpPut("{awb}")] — no /status suffix
        url: API + "/" + encodeURIComponent(awb),
        type: "PUT",
        contentType: "application/json",
        // Controller: [FromBody] string status — must send a plain JSON string
        data: JSON.stringify(newStatus),
        success: function () {
            hideLoader();
            const label = STATUS_LABELS[newStatus] || newStatus;
            showToast("Status updated to \"" + label + "\".", "success");
            // Update badge live without reloading the whole table
            const badgeClass = "badge-" + newStatus;
            $row.attr("data-status", newStatus);
            $row.find(".badge")
                .attr("class", "badge " + badgeClass)
                .text(label);
            loadStats();
        },
        error: function (xhr) {
            hideLoader();
            showToast(xhr.responseText || "Status update failed.", "error");
            // Revert dropdown to current value
            loadShipments();
        }
    });
}

/* ── Cancel Shipment ─────────────────────────────────────── */

function cancelShipment(shipmentId, $row) {
    const awb = $row.find("td:first-child strong").text();
    if (!confirm("Cancel shipment " + awb + "? This cannot be undone.")) return;

    showLoader();
    $.ajax({
        url: API + "/" + shipmentId,
        type: "DELETE",
        success: function () {
            hideLoader();
            showToast("Shipment " + awb + " cancelled.", "success");
            $row.fadeOut(300, function () { $(this).remove(); });
            loadStats();
        },
        error: function (xhr) {
            hideLoader();
            showToast(xhr.responseText || "Cancellation failed.", "error");
        }
    });
}

/* ── Track by AWB ────────────────────────────────────────── */

function trackShipment() {
    const awb = $.trim($("#trackAwb").val());
    if (!awb) { showToast("Enter an AWB number to track.", "error"); return; }

    showLoader();
    $.ajax({
        url: API + "/" + encodeURIComponent(awb),
        type: "GET",
        success: function (s) {
            hideLoader();
            const statusKey   = s.status || "Booked";
            const statusLabel = STATUS_LABELS[statusKey] || statusKey;
            const badgeClass  = "badge-" + statusKey;
            $("#trackResult")
                .removeClass("hidden not-found")
                .html(`
                    <div class="track-grid">
                        <span class="tk-label">AWB</span>
                        <span class="tk-value">${escHtml(s.awbNumber)}</span>
                        <span class="tk-label">Sender</span>
                        <span class="tk-value">${escHtml(s.senderName)}</span>
                        <span class="tk-label">Receiver</span>
                        <span class="tk-value">${escHtml(s.receiverName)}</span>
                        <span class="tk-label">Route</span>
                        <span class="tk-value">${escHtml(s.origin)} → ${escHtml(s.destination)}</span>
                        <span class="tk-label">Weight</span>
                        <span class="tk-value">${parseFloat(s.weightKg).toFixed(2)} kg</span>
                        <span class="tk-label">Status</span>
                        <span class="tk-value">
                            <span class="badge ${badgeClass}">${escHtml(statusLabel)}</span>
                        </span>
                    </div>
                `);
        },
        error: function (xhr) {
            hideLoader();
            if (xhr.status === 404) {
                $("#trackResult")
                    .removeClass("hidden")
                    .addClass("not-found")
                    .html("Shipment not found for AWB: <strong>" + escHtml(awb) + "</strong>");
            } else {
                showToast("Track request failed.", "error");
            }
        }
    });
}

/* ── Helpers ─────────────────────────────────────────────── */

function escHtml(str) {
    if (str == null) return "";
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;");
}