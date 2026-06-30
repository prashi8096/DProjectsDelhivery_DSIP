//using Delhivery.ConsoleAPP.Services;
using Delhivery.Data.Services;
using Delhivery.Domain.Entities;
using Delhivery.Domain.Enums;
using Delhivery.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Delhivery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly ShipmentService _shipmentService;

        public ShipmentController(ShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }
        [HttpGet]
        public IActionResult GetAllShipments()
        {
            var shipments = _shipmentService.ListShipments();
            return Ok(shipments);
        }
        [HttpGet("{awb}")]
        public IActionResult GetShipmentByAwb(string awb)
        {
            try
            {
                var shipment = _shipmentService.GetShipmentByAWB(awb);
                return Ok(shipment);
            }
            catch (ShipmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
       
        public IActionResult BookShipment([FromBody] Shipment shipment)
        {
            try
            {
                _shipmentService.BookShipment(shipment);
                return CreatedAtAction(
      nameof(GetShipmentByAwb),
      new { awb = shipment.AWBNumber },
      shipment);
            }
            catch (DuplicateAwbException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidShipmentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{awb}")]
        public IActionResult UpdateShipmentStatus(string awb, [FromBody] string status)
        {
            try
            {
                if (!Enum.TryParse<ShipmentStatus>(status, true, out ShipmentStatus shipmentStatus))
                {
                    return BadRequest("Valid Status values are: Booked, InTransit, OutForDelivery, Delivered, RTO.");
                }

                _shipmentService.UpdateStatus(awb, shipmentStatus);

                var shipment = _shipmentService.GetShipmentByAWB(awb);

                return Ok(shipment);
            }
            catch (ShipmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidShipmentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult CancelShipment(int id)
        {
            try
            {
                _shipmentService.CancelShipment(id);
                return NoContent();   // 204
            }
            catch (ShipmentNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("stats")]
        
        public IActionResult GetShipmentStats()
        {
            var stats = _shipmentService.GetShipmentStats();
            return Ok(stats);
        }
    }
}