using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Persistence.Repositories;
using Xunit;

namespace RoomBookingApp.Persistence.Tests
{
  public class RoomBookingServiceTest
  {
    [Fact]
    public void Should_Return_Available_Rooms()
    {
      //Arrange
      var date = new DateOnly(2026, 10, 02);

      var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
        .UseInMemoryDatabase(databaseName: "AvailableRoomTest")
        .Options;

      using var context = new RoomBookingAppDbContext(dbOptions);

      context.Add(new Room { Id = 1, Name = "Room 1" });
      context.Add(new Room { Id = 2, Name = "Room 2" });
      context.Add(new Room { Id = 3, Name = "Room 3" });

      context.Add(new RoomBooking { RoomId = 1, Date = date, Email = "test@test.com", FullName = "Test User" });
      context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1), Email = "test@test.com", FullName = "Test User" });

      context.SaveChanges();

      var roomBookingService = new RoomBookingService(context);

      //Act
      var availableRooms = roomBookingService.GetAvailableRooms(date);

      //Assert
      Assert.Equal(2, availableRooms.Count());
      Assert.Contains(availableRooms, q => q.Id == 2);
      Assert.Contains(availableRooms, q => q.Id == 3);
      Assert.DoesNotContain(availableRooms, q => q.Id == 1);

    }

    [Fact]
    public void Should_Save_Room_Booking()
    {
      var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
       .UseInMemoryDatabase(databaseName: "ShouldSaveTest")
       .Options;

      var roomBooking = new RoomBooking { RoomId = 1, Date = new DateOnly(2021, 06, 09) };

      using var context = new RoomBookingAppDbContext(dbOptions);
      var roomBookingService = new RoomBookingService(context);
      roomBookingService.Save(roomBooking);

      var bookings = context.RoomBookings.ToList();
      var booking = Assert.Single(bookings);

      Assert.Equal(roomBooking.RoomId, booking.RoomId);
      Assert.Equal(roomBooking.Date, booking.Date);
    }
  }
}
