using Microsoft.AspNetCore.Mvc;
using Moq;
using RoomBookingApp.Api.Controllers;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain.BaseModels;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomBookingApp.Api.Tests
{
  public class RoomBookingControllerTests
  {
    private Mock<IRoomBookingRequestProcessor> _roomBookingProcessor;
    private RoomBookingController _controller;
    private RoomBookingRequest _request;
    private RoomBookingResult _result;
    public RoomBookingControllerTests()
    {
      _roomBookingProcessor = new Mock<IRoomBookingRequestProcessor>();
      _controller = new RoomBookingController(_roomBookingProcessor.Object);
      _request = new RoomBookingRequest();
      _result = new RoomBookingResult();
    
      _roomBookingProcessor.Setup(x => x.BookRoom(_request)).Returns(_result);
    }

    [Theory]
    [InlineData(1, true, typeof(OkObjectResult), BookingResultFlag.Success)]
    [InlineData(0, false, typeof(BadRequestObjectResult), BookingResultFlag.Failure)]
    public async Task SHould_Call_Booking_Method_When_Valid(int expectedMethodCalls, bool isModelValid, 
      Type expectedActionResultType, BookingResultFlag? bookingResultFlag)
    {
      // Arrange
      if (!isModelValid)
      {
        _controller.ModelState.AddModelError("Key", "ErrorMessage");
      }

      // Act
      var result = await _controller.BookRoom(_request);

      // Assert
      result.ShouldBeOfType(expectedActionResultType);
      _roomBookingProcessor.Verify(x => x.BookRoom(_request), Times.Exactly(expectedMethodCalls));

    }
  }
}
