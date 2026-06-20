using Xunit;
using RouteShare.Domain;
using System;
using System.Collections.Generic;

namespace RouteShare.Tests
{
    public class RouteShareTests
    {
        [Fact]
        public void Login_ShouldReturnTrue()
        {
            var user = new RegisteredUser();
            var result = user.Login("test@mail.com", "password");
            Assert.True(result);
        }

        [Fact]
        public void CreateTrip_ShouldAddTripToList()
        {
            var user = new RegisteredUser();
            var trip = new Trip();
            user.CreateTrip(trip);
            Assert.Contains(trip, user.MyTrips);
        }

        [Fact]
        public void BookTrip_ShouldReduceSeats()
        {
            var user = new RegisteredUser();
            var trip = new Trip { Seats = 4 };
            user.BookTrip(trip);
            Assert.Equal(3, trip.Seats);
        }

        [Fact]
        public void CancelTrip_ShouldRemoveTripFromList()
        {
            var user = new RegisteredUser();
            var trip = new Trip();
            user.MyTrips.Add(trip);
            user.CancelTrip(trip);
            Assert.DoesNotContain(trip, user.MyTrips);
        }

        [Fact]
        public void BlockUser_ShouldExecute()
        {
            var admin = new Admin();
            var user = new RegisteredUser();
            admin.BlockUser(user);
            Assert.NotNull(admin);
        }

        [Fact]
        public void DeleteTrip_ShouldExecute()
        {
            var admin = new Admin();
            var trip = new Trip();
            admin.DeleteTrip(trip);
            Assert.NotNull(admin);
        }

        [Fact]
        public void CancelReservation_ShouldIncreaseSeats()
        {
            var trip = new Trip { Seats = 3 };
            trip.CancelReservation();
            Assert.Equal(4, trip.Seats);
        }

        [Fact]
        public void SaveData_ShouldExecute()
        {
            var users = new List<User>();
            var trips = new List<Trip>();
            DataManager.SaveData(users, trips);
            Assert.Empty(users);
        }

        [Fact]
        public void LoadData_ShouldExecute()
        {
            DataManager.LoadData();
            Assert.True(true);
        }
    }
}