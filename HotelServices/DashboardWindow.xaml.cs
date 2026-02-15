using HotelServices.Models;
using HotelServices.Pages;
using System.Windows;

namespace HotelServices
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;

        public DashboardWindow(User user)
        {
            InitializeComponent();  // Це має бути першим рядком!
            _currentUser = user;
            Title = $"Головна панель - {user.FullName} ({user.Role})";
            mainFrame.Navigate(new DashboardPage(user));
        }
    }
}