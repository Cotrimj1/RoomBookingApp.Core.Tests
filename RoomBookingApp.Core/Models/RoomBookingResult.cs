using RoomBookingApp.Core.Enums;

namespace RoomBookingApp.Domain.BaseModels
{
  public class RoomBookingResult : RoomBookingBase
  {
    public BookingResultFlag Flag { get; set; }
    public object RoomBookingId { get; set; }
  }
}