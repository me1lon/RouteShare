using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RouteShare.Domain
{
    public static class DataManager
    {
        public static RegisteredUser CurrentUser { get; set; }
        public static List<RegisteredUser> Users { get; set; } = new List<RegisteredUser>();
        private static readonly string filePath = "users.json";

        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public static void SaveUsers()
        {
            string json = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(filePath, json);
        }

        public static void LoadUsers(bool force = false)
        {
            if (Users.Count > 0 && !force) return;

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<List<RegisteredUser>>(json, options);
                if (loaded != null)
                {
                    Users = loaded;

                    Dictionary<string, Trip> realTrips = new Dictionary<string, Trip>();

                    foreach (var user in Users)
                    {
                        if (user.BookedSeatsMap == null) user.BookedSeatsMap = new Dictionary<string, int>();
                        if (user.MyTrips == null) user.MyTrips = new List<Trip>();

                        foreach (var trip in user.MyTrips)
                        {
                            if (!user.BookedSeatsMap.ContainsKey(trip.Id))
                            {
                                trip.Driver = user;
                                realTrips[trip.Id] = trip;
                            }
                        }
                    }

                    foreach (var user in Users)
                    {
                        for (int i = 0; i < user.MyTrips.Count; i++)
                        {
                            var tripId = user.MyTrips[i].Id;
                            if (user.BookedSeatsMap.ContainsKey(tripId) && realTrips.ContainsKey(tripId))
                            {
                                user.MyTrips[i] = realTrips[tripId];
                            }
                        }
                    }
                }
            }
        }
    }
}