using MachineMonitoringRepository.Models.DDD;
using Microsoft.AspNetCore.Mvc;

namespace MachineMonitoring.WebAPI.Controllers
{
    public class OrderApiController
    {
        [HttpPost("orders/{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            /*
            var order = await _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.OrderId == id);

            // ? Business rules scattered in controller
            if (order.Status != "Pending")
                return BadRequest("Order must be pending");

            if (!order.Lines.Any())
                return BadRequest("Order must have lines");

            if (order.Lines.Sum(l => l.Quantity * l.UnitPrice) <= 0)
                return BadRequest("Total must be positive");

            // ? Direct database manipulation
            order.Status = "Confirmed";
            order.ConfirmedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // ? Side effects mixed with business logic
            await _emailService.SendOrderConfirmation(order.CustomerEmail);

            return Ok();
            */
            return null;
        }
    }
}
