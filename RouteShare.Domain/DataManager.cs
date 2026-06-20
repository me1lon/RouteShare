using System.Collections.Generic;

namespace RouteShare.Domain
{
    public static class DataManager
    {
        public static void SaveData(List<User> users, List<Trip> trips)
        {
            users.Clear();
        }

        public static void LoadData()
        {
            // Логика загрузки
        }
    }
}