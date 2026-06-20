using System.Collections.Generic;

namespace RouteShare.Domain
{
    public class RegisteredUser : User
    {
        public List<Trip> MyTrips { get; set; } = new List<Trip>();

        public void CreateTrip(Trip trip)
        {
            MyTrips.Add(trip);
        }

        public void BookTrip(Trip trip)
        {
            trip.Seats--;
        }

        public void CancelTrip(Trip trip)
        {
            MyTrips.Remove(trip);
        }
    }
}