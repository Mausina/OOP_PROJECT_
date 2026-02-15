using HotelServices.Services;
using HotelServices.Pages;
using System.Windows;
using System.Windows.Input;

namespace HotelServices
{
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService;

        public MainWindow()
        {
            InitializeComponent();
            _authService = new AuthService();

            // Дозволяє перетягувати вікно
            this.MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Введіть логін та пароль";
                return;
            }

            var user = _authService.Authenticate(username, password);
            if (user != null)
            {
                var dashboard = new DashboardWindow(user);
                dashboard.Show();
                this.Close();
            }
            else
            {
                lblError.Text = "Невірний логін або пароль";
            }
        }
    }
}