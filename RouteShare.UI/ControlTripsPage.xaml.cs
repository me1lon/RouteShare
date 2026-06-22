using RouteShare.Domain;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RouteShare.UI
{
    public partial class ControlTripsPage : Page
    {
        public ObservableCollection<Trip> MyTrips { get; set; }

        public ControlTripsPage()
        {
            InitializeComponent();

            if (DataManager.CurrentUser != null)
            {
                MyTrips = new ObservableCollection<Trip>(DataManager.CurrentUser.MyTrips);
            }
            else
            {
                MyTrips = new ObservableCollection<Trip>();
            }

            TripsList.ItemsSource = MyTrips;
            UpdateHeader();
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

        private void RoleText_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Trip trip)
            {
                if (DataManager.CurrentUser != null && trip.Driver != null && trip.Driver.Email == DataManager.CurrentUser.Email)
                {
                    tb.Text = "Ваша поїздка (Водій)";
                }
                else
                {
                    tb.Text = "Ви пасажир";
                }
            }
        }

        private void SeatsText_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Trip trip)
            {
                var user = DataManager.CurrentUser;
                if (user != null)
                {
                    if (trip.Driver != null && trip.Driver.Email == user.Email)
                    {
                        tb.Text = $"{trip.Seats} місця залишилось";
                    }
                    else
                    {
                        int booked = user.BookedSeatsMap.ContainsKey(trip.Id) ? user.BookedSeatsMap[trip.Id] : 0;
                        tb.Text = $"{booked} місця заброньовано";
                    }
                }
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

        private void CreateTrip_Click(object sender, RoutedEventArgs e)
        {
            if (DataManager.CurrentUser == null)
            {
                MessageBox.Show("Увійдіть в акаунт, щоб створити поїздку!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string departure = DepartureInput.Text;
                string arrival = ArrivalInput.Text;

                if (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival) || departure == DepartureInput.Tag.ToString() && arrival == ArrivalInput.Tag.ToString())
                {
                    MessageBox.Show("Заповніть реальні міста відправлення та прибуття!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime departureDateTime = DateTime.ParseExact($"{DepDateInput.Text} {DepTimeInput.Text}", "dd.MM.yyyy HH:mm", null);
                DateTime arrivalDateTime = DateTime.ParseExact($"{ArrDateInput.Text} {ArrTimeInput.Text}", "dd.MM.yyyy HH:mm", null);

                int seats = int.Parse(SeatsInput.Text);
                decimal price = decimal.Parse(PriceInput.Text);

                Trip newTrip = new Trip
                {
                    DepartureCity = departure,
                    ArrivalCity = arrival,
                    Date = departureDateTime,
                    ArrivalDate = arrivalDateTime,
                    Seats = seats,
                    Price = price,
                    Driver = DataManager.CurrentUser
                };

                MyTrips.Add(newTrip);

                DataManager.CurrentUser.CreateTrip(newTrip);
                DataManager.SaveUsers();

                MessageBox.Show("Поїздку успішно створено та збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetField(DepartureInput);
                ResetField(ArrivalInput);
                ResetField(DepDateInput);
                ResetField(DepTimeInput);
                ResetField(ArrDateInput);
                ResetField(ArrTimeInput);
                ResetField(SeatsInput);
                ResetField(PriceInput);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetField(TextBox tb)
        {
            tb.Text = tb.Tag.ToString();
            tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
        }

        private void CancelTrip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Trip tripToCancel)
            {
                MyTrips.Remove(tripToCancel);

                if (DataManager.CurrentUser != null)
                {
                    if (tripToCancel.Driver == null || tripToCancel.Driver.Email != DataManager.CurrentUser.Email)
                    {
                        int seatsToReturn = DataManager.CurrentUser.BookedSeatsMap.ContainsKey(tripToCancel.Id) ? DataManager.CurrentUser.BookedSeatsMap[tripToCancel.Id] : 1;
                        tripToCancel.Seats += seatsToReturn;
                    }

                    DataManager.CurrentUser.CancelTrip(tripToCancel);
                    DataManager.SaveUsers();
                }
            }
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