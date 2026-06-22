using Microsoft.Win32;
using RouteShare.Domain;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RouteShare.UI
{
    public partial class AdminPanelPage : Page
    {
        public AdminPanelPage()
        {
            InitializeComponent();
            LoadData();
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

        private void LoadData()
        {
            DataManager.LoadUsers();

            UsersList.ItemsSource = null;
            UsersList.ItemsSource = DataManager.Users;

            var allTrips = new System.Collections.Generic.List<Trip>();
            foreach (var user in DataManager.Users)
            {
                if (user.MyTrips != null)
                {
                    allTrips.AddRange(user.MyTrips.Where(t => t.Driver != null && t.Driver.Email == user.Email));
                }
            }

            TripsList.ItemsSource = null;
            TripsList.ItemsSource = allTrips;
        }

        private void LoadJson_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.Title = "Виберіть файл для завантаження даних";

            if (openFileDialog.ShowDialog() == true)
            {
                string json = System.IO.File.ReadAllText(openFileDialog.FileName);
                var loadedUsers = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<RegisteredUser>>(json) ?? new System.Collections.Generic.List<RegisteredUser>();

                DataManager.Users = loadedUsers;
                DataManager.SaveUsers();
                DataManager.LoadUsers(true);
                LoadData();

                MessageBox.Show($"Дані успішно завантажено з файлу:\n{openFileDialog.FileName}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveJson_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.FileName = "RouteShare_Export.json";
            saveFileDialog.Title = "Виберіть місце для збереження";

            if (saveFileDialog.ShowDialog() == true)
            {
                string json = System.Text.Json.JsonSerializer.Serialize(DataManager.Users, new System.Text.Json.JsonSerializerOptions { WriteIndented = true, IncludeFields = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
                System.IO.File.WriteAllText(saveFileDialog.FileName, json);

                MessageBox.Show($"Дані успішно збережено у файл:\n{saveFileDialog.FileName}", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RegisteredUser userToBlock)
            {
                if (DataManager.CurrentUser != null && userToBlock.Email == DataManager.CurrentUser.Email)
                {
                    MessageBox.Show("Ви не можете заблокувати самі себе!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataManager.Users.Remove(userToBlock);
                DataManager.SaveUsers();
                LoadData();
            }
        }

        private void DeleteTrip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Trip tripToDelete)
            {
                var owner = DataManager.Users.FirstOrDefault(u => u.MyTrips != null && u.MyTrips.Contains(tripToDelete));
                if (owner != null)
                {
                    owner.MyTrips.Remove(tripToDelete);
                    DataManager.SaveUsers();
                    LoadData();
                }
            }
        }

        private void ShowUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersContainer.Visibility = Visibility.Visible;
            TripsContainer.Visibility = Visibility.Collapsed;

            UsersTabBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6E6"));
            TripsTabBorder.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ShowTrips_Click(object sender, RoutedEventArgs e)
        {
            UsersContainer.Visibility = Visibility.Collapsed;
            TripsContainer.Visibility = Visibility.Visible;

            UsersTabBorder.Background = new SolidColorBrush(Colors.Transparent);
            TripsTabBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6E6"));
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