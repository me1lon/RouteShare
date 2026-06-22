using System.Collections.Generic;

namespace RouteShare.Domain
{
    public class RegisteredUser : User
    {
        public List<Trip> MyTrips { get; set; } = new List<Trip>();

        // Словник, який зберігає "ID поїздки" -> "Кількість заброньованих місць"
        public Dictionary<string, int> BookedSeatsMap { get; set; } = new Dictionary<string, int>();

        public void CreateTrip(Trip trip)
        {
            MyTrips.Add(trip);
        }

        public void BookTrip(Trip trip, int seats = 1)
        {
            trip.Seats -= seats;
            if (!MyTrips.Contains(trip))
            {
                MyTrips.Add(trip);
            }

            if (BookedSeatsMap.ContainsKey(trip.Id))
                BookedSeatsMap[trip.Id] += seats;
            else
                BookedSeatsMap[trip.Id] = seats;
        }

        public void CancelTrip(Trip trip)
        {
            MyTrips.Remove(trip);
            if (BookedSeatsMap.ContainsKey(trip.Id))
            {
                BookedSeatsMap.Remove(trip.Id);
            }
        }
    }
}