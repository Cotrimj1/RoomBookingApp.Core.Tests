using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RoomBookingController : ControllerBase
  {
    private IRoomBookingRequestProcessor _roomBookingProcessor;

    public RoomBookingController(IRoomBookingRequestProcessor roomBookingProcessor)
    {
      this._roomBookingProcessor = roomBookingProcessor;
    }

    [HttpPost]
    public async Task<IActionResult> BookRoom(RoomBookingRequest request)
    {
      if (ModelState.IsValid)
      {
        var result = _roomBookingProcessor.BookRoom(request);
        if (result.Flag == Core.Enums.BookingResultFlag.Success)
        {
          return Ok(result);
        }

        ModelState.AddModelError(nameof(RoomBookingRequest.Date), "No rooms Available for given date.");
      }
      return BadRequest(ModelState);
    }
  }
}
