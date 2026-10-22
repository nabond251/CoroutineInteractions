using Microsoft.AspNetCore.Mvc;

namespace CoroutineInteractions.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        [HttpGet("Slots", Name = "GetSlots")]
        public IEnumerable<Slot> GetSlotsAsync(DateOnly slotDate)
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
    }
}
