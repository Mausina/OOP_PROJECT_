using HotelServices.Models;
using HotelServices.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HotelServices.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly User _currentUser;
        private readonly LanguageService _lang = LanguageService.Instance;

        public DashboardPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            DataContext = this;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (_currentUser.Role == UserRole.Director)
            {
                btnUserManagement.Visibility = Visibility.Visible;
            }

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            lblDashTitle.Text         = Strings.Get("Dashboard_Title");
            lblDashSubtitle.Text      = Strings.Get("Dashboard_Subtitle");

            lblApartments.Text        = Strings.Get("Dashboard_Apartments");
            lblApartmentsSub.Text     = Strings.Get("Dashboard_Apartments_Sub");
            btnApartments.ToolTip     = Strings.Get("Tooltip_Apartments");

            lblConference.Text        = Strings.Get("Dashboard_Conference");
            lblConferenceSub.Text     = Strings.Get("Dashboard_Conference_Sub");
            btnConference.ToolTip     = Strings.Get("Tooltip_Conference");

            lblParking.Text           = Strings.Get("Dashboard_Parking");
            lblParkingSub.Text        = Strings.Get("Dashboard_Parking_Sub");
            btnParking.ToolTip        = Strings.Get("Tooltip_Parking");

            lblRestaurant.Text        = Strings.Get("Dashboard_Restaurant");
            lblRestaurantSub.Text     = Strings.Get("Dashboard_Restaurant_Sub");
            btnRestaurant.ToolTip     = Strings.Get("Tooltip_Restaurant");

            lblServices.Text          = Strings.Get("Dashboard_Services");
            lblServicesSub.Text       = Strings.Get("Dashboard_Services_Sub");
            btnServices.ToolTip       = Strings.Get("Tooltip_Services");

            lblUsers.Text             = Strings.Get("Dashboard_Users");
            lblUsersSub.Text          = Strings.Get("Dashboard_Users_Sub");
            btnUserManagement.ToolTip = Strings.Get("Tooltip_Users");
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e) { }

        private void NavigateToApartments(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new ResourcePage(ResourceType.Apartment, _currentUser));
        }

        private void NavigateToConferenceRooms(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new ResourcePage(ResourceType.ConferenceRoom, _currentUser));
        }

        private void NavigateToParking(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new ResourcePage(ResourceType.ParkingSpace, _currentUser));
        }

        private void NavigateToRestaurant(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new ResourcePage(ResourceType.RestaurantTable, _currentUser));
        }

        private void NavigateToServices(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new ResourcePage(ResourceType.AdditionalService, _currentUser));
        }

        private void NavigateToUserManagement(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) AnimateButtonClick(button);
            NavigationService.Navigate(new UserManagementPage(_currentUser));
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;
        }
    }
}