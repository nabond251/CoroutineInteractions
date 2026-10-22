using Microsoft.AspNetCore.Mvc;

namespace CoroutineInteractions.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController(
        ILogger<ReservationsController> logger) : ControllerBase
    {
        [HttpGet("Slots", Name = "GetSlots")]
        public IEnumerable<Slot> GetSlots(DateOnly slotDate)
        {
            return Enumerable
                .Range(1, Random.Shared.Next(0, 5))
                .Select(index => new Slot
                {
                    SlotDate = slotDate,
                    SeatsLeft = Random.Shared.Next(0, 55),
                })
                .ToArray();
        }

        [HttpPost(Name = "PostReservation")]
        public void Post(Reservation reservation)
        {
            logger.LogInformation(reservation.ToString());
        }
    }
}
