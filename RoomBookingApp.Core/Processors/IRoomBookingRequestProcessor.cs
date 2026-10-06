using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Core.Processors
{
  public interface IRoomBookingRequestProcessor
  {
    RoomBookingResult BookRoom(RoomBookingRequest bookingRequest);
  }
}