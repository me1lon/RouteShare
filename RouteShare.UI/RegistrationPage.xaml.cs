using RouteShare.Domain;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RouteShare.UI
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            DataManager.LoadUsers();
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBoxInput.Password;
            string confirmPassword = ConfirmPasswordBoxInput.Password;

            ErrorText.Text = "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Заповніть всі поля!";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorText.Text = "Паролі не співпадають!";
                return;
            }

            if (DataManager.Users.Any(u => u.Email == email))
            {
                ErrorText.Text = "Користувач з таким Email вже існує!";
                return;
            }

            RegisteredUser newUser = new RegisteredUser
            {
                Name = name, // Зберігаємо ім'я
                Email = email,
                Password = password
            };

            DataManager.Users.Add(newUser);
            DataManager.SaveUsers();

            DataManager.CurrentUser = newUser;
            NavigationService.Navigate(new MainPage());
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
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