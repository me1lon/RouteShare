using RouteShare.Domain;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RouteShare.UI
{
    public partial class LoginPage : Page
    {
        public LoginPage()
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBoxInput.Password;

            ErrorText.Text = "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Заповніть всі поля!";
                return;
            }

            var user = DataManager.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && user.Login(email, password))
            {
                DataManager.CurrentUser = user; // Зберігаємо сесію
                NavigationService.Navigate(new MainPage());
            }
            else
            {
                ErrorText.Text = "Неправильний Email або пароль!";
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
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