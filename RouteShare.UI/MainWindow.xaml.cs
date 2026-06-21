namespace RouteShare.UI
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AdminPanelPage());
        }
    }
}