import requests
import csv
import sys
from datetime import datetime

API_BASE_URL = "https://localhost:7113/api/Shipment"


def get_shipments():
    try:
        response = requests.get(
            API_BASE_URL,
            verify=False,
            timeout=5
        )

        response.raise_for_status()

        return response.json()

    except requests.exceptions.RequestException:

        print("ERROR: DSIP API is offline.")

        sys.exit(1)


def get_stats():
    try:
        response = requests.get(
            API_BASE_URL + "/stats",
            verify=False,
            timeout=5
        )

        response.raise_for_status()

        return response.json()

    except requests.exceptions.RequestException:

        print("ERROR: DSIP API is offline.")

        sys.exit(1)

def export_csv(shipments):

    file_name = "delhivery_report_" + datetime.now().strftime("%Y%m%d") + ".csv"

    with open(file_name, "w", newline="", encoding="utf-8") as file:

        writer = csv.writer(file)

        writer.writerow([
            "ShipmentId",
            "AWB",
            "Sender",
            "Receiver",
            "Origin",
            "Destination",
            "WeightKg",
            "Status",
            "BookedAt",
            "DeliveredAt"
        ])

        for shipment in shipments:

            writer.writerow([
                shipment["shipmentId"],
                shipment["awbNumber"],
                shipment["senderName"],
                shipment["receiverName"],
                shipment["origin"],
                shipment["destination"],
                shipment["weightKg"],
                shipment["status"],
                shipment["bookedAt"],
                shipment["deliveredAt"]
            ])

    print("\nCSV exported successfully.")
    print("File :", file_name)

if __name__ == "__main__":

    shipments = get_shipments()
    stats = get_stats()

    total_shipments = len(shipments)

    total_weight = 0

    heaviest_weight = 0
    heaviest_awb = ""

    for shipment in shipments:

        total_weight += shipment["weightKg"]

        if shipment["weightKg"] > heaviest_weight:
            heaviest_weight = shipment["weightKg"]
            heaviest_awb = shipment["awbNumber"]

    if total_shipments > 0:
        average_weight = total_weight / total_shipments
    else:
        average_weight = 0
    print("================================================")
    print(" DELHIVERY - END OF DAY SHIPMENT REPORT")
    print(" Date            :", datetime.now().strftime("%Y-%m-%d"))
    print("================================================")

    print(" Total Shipments :", total_shipments)
    print(" Booked          :", stats["booked"])
    print(" In Transit      :", stats["inTransit"])
    print(" Out For Delivery:", stats["outForDelivery"])
    print(" Delivered       :", stats["delivered"])
    print(" RTO             :", stats["rto"])

    print("------------------------------------------------")

    print(" Avg Weight      : {:.2f} kg".format(average_weight))
    print(" Heaviest        : {} ({:.2f} kg)".format(heaviest_awb, heaviest_weight))

    print("================================================")
    if "--export" in sys.argv:
     export_csv(shipments)


