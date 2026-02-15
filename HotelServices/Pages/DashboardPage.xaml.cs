using HotelServices.Models;
using HotelServices;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HotelServices.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly User _currentUser;

        public DashboardPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            DataContext = this;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Налаштування показу кнопки управління користувачами для директора
            if (_currentUser.Role == UserRole.Director)
            {
                btnUserManagement.Visibility = Visibility.Visible;
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            // Цей метод порожній, оскільки кнопка прихована на головній сторінці
        }

        private void NavigateToApartments(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new ResourcePage(ResourceType.Apartment, _currentUser));
        }

        private void NavigateToConferenceRooms(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new ResourcePage(ResourceType.ConferenceRoom, _currentUser));
        }

        private void NavigateToParking(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new ResourcePage(ResourceType.ParkingSpace, _currentUser));
        }

        private void NavigateToRestaurant(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new ResourcePage(ResourceType.RestaurantTable, _currentUser));
        }

        private void NavigateToServices(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new ResourcePage(ResourceType.AdditionalService, _currentUser));
        }

        private void NavigateToUserManagement(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AnimateButtonClick(button);
            }
            NavigationService.Navigate(new UserManagementPage(_currentUser));
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;

        }
    }
}