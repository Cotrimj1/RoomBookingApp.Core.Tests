using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain.BaseModels;
using Shouldly;

namespace RoomBookingApp.Core
{
  public class RoomBookingRequestProcessorTest
  {
    private RoomBookingRequestProcessor _processor;
    private RoomBookingRequest _request;
    private Mock<IRoomBookingService> _roomBookingServiceMock;
    private List<Room> _availableRooms;

    public RoomBookingRequestProcessorTest()
    {
      //Arrange
      _request = new RoomBookingRequest
      {
        FullName = "Test Name",
        Email = "test@request.com",
        Date = new DateOnly(2026, 10, 1)
      };
      _availableRooms = new List<Room> { new Room()
      {
        Id =1,
      }
      };

      _roomBookingServiceMock = new Mock<IRoomBookingService>();
      _roomBookingServiceMock.Setup(x => x.GetAvailableRooms(_request.Date))
        .Returns(_availableRooms);
      _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);

    }

    [Fact]
    public void Should_Return_Room_Booking_Response_With_Request_Values()
    {
      //Arrange



      //Act - Call the method
      RoomBookingResult result = _processor.BookRoom(_request);

      //Assert
      Assert.NotNull(result);
      Assert.Equal(_request.FullName, result.FullName);
      Assert.Equal(_request.Email, result.Email);
      Assert.Equal(_request.Date, result.Date);

      //result.ShouldNotBeNull();
      //result.FullName.ShouldBe(request.FullName); 
      //result.Email.ShouldBe(request.Email);
      //result.Date.ShouldBe(request.Date);
    }

    [Fact]
    public void Should_Throw_Exception_For_Null_request()
    {
      var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookRoom(null));
      exception.ParamName.ShouldBe("bookingRequest");

    }

    [Fact]
    public void Should_Save_Room_Booking_request()
    {
      RoomBooking savedBooking = null;
      _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
        .Callback<RoomBooking>(x => savedBooking = x);

      _processor.BookRoom(_request);

      _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once);

      savedBooking.ShouldNotBeNull();
      savedBooking.FullName.ShouldBe(_request.FullName);
      savedBooking.Email.ShouldBe(_request.Email);
      savedBooking.Date.ShouldBe(_request.Date);
      savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
    }

    [Fact]
    public void Should_Not_Save_Room_Booking_Request_If_None_Available()
    {
      _availableRooms.Clear(); // Clear the available rooms to simulate no availability
      _processor.BookRoom(_request);
      _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);

    }

    [Theory]
    [InlineData(BookingResultFlag.Failure, false)]
    [InlineData(BookingResultFlag.Success, true)]
    public void Should_return_Success_Or_Failure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
    {
      if (!isAvailable)
      {
        _availableRooms.Clear();
      }

      var result = _processor.BookRoom(_request);
      bookingSuccessFlag.ShouldBe(result.Flag);

    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(null, false)]
    public void Should_Return_RoomBooking_Id_In_Result(int? roomBookingId, bool isAvailable)
    {
      if (!isAvailable)
      {
        _availableRooms.Clear();
      }
      else
      {
        _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
        .Callback<RoomBooking>(booking => booking.Id = roomBookingId.Value);
      }

      var result = _processor.BookRoom(_request);
      result.RoomBookingId.ShouldBe(roomBookingId);
    }
  }
}
