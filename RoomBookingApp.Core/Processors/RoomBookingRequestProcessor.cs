using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Core.Processors
{
  public class RoomBookingRequestProcessor : IRoomBookingRequestProcessor
  {
    private readonly IRoomBookingService _roombookingService;

    public RoomBookingRequestProcessor(IRoomBookingService roomBookingService)
    {
      this._roombookingService = roomBookingService;
    }

    public RoomBookingResult BookRoom(RoomBookingRequest bookingRequest)
    {
      if (bookingRequest == null)
      {
        throw new ArgumentNullException(nameof(bookingRequest));
      }

      var availableRooms = _roombookingService.GetAvailableRooms(bookingRequest.Date);
      var result = CreateRoomBookingObject<RoomBookingResult>(bookingRequest);

      if (availableRooms.Any())
      {
        var room = availableRooms.First();
        var roomBooking = CreateRoomBookingObject<RoomBooking>(bookingRequest);
        roomBooking.RoomId = room.Id;
        _roombookingService.Save(roomBooking);

        result.RoomBookingId = roomBooking.Id;
        result.Flag = Enums.BookingResultFlag.Success;

      }
      else
      {
        result.Flag = Enums.BookingResultFlag.Failure;
      }

      return result;
    }

    private TRoomBooking CreateRoomBookingObject<TRoomBooking>(RoomBookingRequest bookingRequest) where TRoomBooking
      : RoomBookingBase, new()
    {
      return new TRoomBooking
      {
        FullName = bookingRequest.FullName,
        Email = bookingRequest.Email,
        Date = bookingRequest.Date
      };

    }
  }
}