using System;

namespace RouteShare.Domain
{
    public class Trip
    {
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime Date { get; set; }
        public int Seats { get; set; }
        public decimal Price { get; set; }
        public RegisteredUser Driver { get; set; }

        public void CancelReservation()
        {
            Seats++;
        }
    }
}