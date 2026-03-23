using HotelServices.Services;
using HotelServices.Pages;
using System.Windows;
using System.Windows.Input;

namespace HotelServices
{
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService;
        private readonly LanguageService _lang = LanguageService.Instance;

        public MainWindow()
        {
            InitializeComponent();
            _authService = new AuthService();

            // Дозволяє перетягувати вікно
            this.MouseDown += Window_MouseDown;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            this.Title          = Strings.Get("Window_Login");
            btnLang.Content     = _lang.ButtonText;
            lblLoginTitle.Text  = Strings.Get("Login_Title");
            lblUsername.Text    = Strings.Get("Login_Username");
            lblPassword.Text    = Strings.Get("Login_Password");
            btnLogin.Content    = Strings.Get("Login_Button");
            lblError.Text       = string.Empty;
        }

        private void BtnLang_Click(object sender, RoutedEventArgs e)
        {
            _lang.Toggle();
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
                lblError.Text = Strings.Get("Login_Error_Empty");
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
                lblError.Text = Strings.Get("Login_Error_Invalid");
            }
        }
    }
}