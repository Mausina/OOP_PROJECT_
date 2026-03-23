using HotelServices.Models;
using HotelServices.Pages;
using HotelServices.Services;
using System.Windows;

namespace HotelServices
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;
        private readonly LanguageService _lang = LanguageService.Instance;

        public DashboardWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            Title = $"Головна панель - {user.FullName} ({user.Role})";
            mainFrame.Navigate(new DashboardPage(user));

            _lang.LanguageChanged += (s, e) => btnLang.Content = _lang.ButtonText;
            btnLang.Content = _lang.ButtonText;
        }

        private void BtnLang_Click(object sender, RoutedEventArgs e)
        {
            _lang.Toggle();
        }
    }
}