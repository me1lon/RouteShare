using RouteShare.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RouteShare.UI
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataManager.LoadUsers();
            UpdateHeader();
            LoadAllTrips();
        }

        private void UpdateHeader()
        {
            if (DataManager.CurrentUser != null)
            {
                UnauthHeader.Visibility = Visibility.Collapsed;
                AuthHeader.Visibility = Visibility.Visible;
            }
            else
            {
                UnauthHeader.Visibility = Visibility.Visible;
                AuthHeader.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadAllTrips()
        {
            var allTrips = new List<Trip>();
            foreach (var user in DataManager.Users)
            {
                if (user.MyTrips != null)
                {
                    allTrips.AddRange(user.MyTrips.Where(t => t.Seats > 0 && t.Driver != null && t.Driver.Email == user.Email));
                }
            }
            SearchResultsList.ItemsSource = allTrips;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string departure = SearchDeparture.Text != SearchDeparture.Tag.ToString() ? SearchDeparture.Text.ToLower() : "";
            string arrival = SearchArrival.Text != SearchArrival.Tag.ToString() ? SearchArrival.Text.ToLower() : "";

            int seatsNeeded = 1;
            if (SearchSeats.Text != SearchSeats.Tag.ToString() && int.TryParse(SearchSeats.Text, out int parsedSeats))
            {
                seatsNeeded = parsedSeats;
            }

            var results = new List<Trip>();

            foreach (var user in DataManager.Users)
            {
                if (user.MyTrips != null)
                {
                    foreach (var trip in user.MyTrips)
                    {
                        if (trip.Driver != null && trip.Driver.Email == user.Email && trip.Seats >= seatsNeeded)
                        {
                            bool matchDeparture = string.IsNullOrWhiteSpace(departure) || trip.DepartureCity.ToLower().Contains(departure);
                            bool matchArrival = string.IsNullOrWhiteSpace(arrival) || trip.ArrivalCity.ToLower().Contains(arrival);

                            if (matchDeparture && matchArrival)
                            {
                                results.Add(trip);
                            }
                        }
                    }
                }
            }

            SearchResultsList.ItemsSource = results;
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if (DataManager.CurrentUser == null)
            {
                MessageBox.Show("Увійдіть в акаунт, щоб забронювати поїздку!", "Потрібна авторизація", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sender is Button btn && btn.DataContext is Trip tripToBook)
            {
                if (tripToBook.Driver != null && tripToBook.Driver.Email == DataManager.CurrentUser.Email)
                {
                    MessageBox.Show("Ви не можете забронювати власну поїздку!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int seatsToBook = 1;
                if (SearchSeats.Text != SearchSeats.Tag.ToString() && int.TryParse(SearchSeats.Text, out int parsedSeats))
                {
                    seatsToBook = parsedSeats;
                }

                if (tripToBook.Seats >= seatsToBook)
                {
                    DataManager.CurrentUser.BookTrip(tripToBook, seatsToBook);
                    DataManager.SaveUsers();

                    MessageBox.Show($"Поїздку успішно забронювано!\nВи забронювали {seatsToBook} місця(е). Вона тепер відображається у вкладці 'Управління поїздками'.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadAllTrips();
                }
                else
                {
                    MessageBox.Show("Недостатньо вільних місць!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == tb.Tag.ToString())
            {
                tb.Text = "";
                tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"));
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = tb.Tag.ToString();
                tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
            }
        }

        private void ProfileDropdown_Click(object sender, RoutedEventArgs e)
        {
            LogoutPopup.IsOpen = true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LogoutPopup.IsOpen = false;
            DataManager.CurrentUser = null;
            UpdateHeader();
            NavigationService.Navigate(new LoginPage());
        }

        private void LoginNav_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void FindTrip_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void ManageTrips_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ControlTripsPage());
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminPanelPage());
        }
    }
}