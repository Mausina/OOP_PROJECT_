using HotelServices.Models;
using HotelServices.Services;
using HotelServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HotelServices.Dialogs;

namespace HotelServices.Pages
{
    public partial class ResourcePage : Page
    {
        private readonly ResourceType _resourceType;
        private readonly User _currentUser;
        private readonly DataService _dataService;
        private readonly LanguageService _lang = LanguageService.Instance;
        private List<Resource> _resources;

        public ResourcePage(ResourceType resourceType, User currentUser)
        {
            InitializeComponent();
            _resourceType = resourceType;
            _currentUser = currentUser;
            _dataService = new DataService();
            DataContext = this;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
            LoadResources();
        }

        private void ApplyLanguage()
        {
            // Title
            lblTitle.Text = _resourceType switch
            {
                ResourceType.Apartment        => Strings.Get("Resource_Apartments"),
                ResourceType.ConferenceRoom   => Strings.Get("Resource_Conference"),
                ResourceType.ParkingSpace     => Strings.Get("Resource_Parking"),
                ResourceType.RestaurantTable  => Strings.Get("Resource_Restaurant"),
                ResourceType.AdditionalService => Strings.Get("Resource_Services"),
                _                             => Strings.Get("Resource_Apartments")
            };

            // Buttons
            btnAdd.Content          = Strings.Get("Btn_Add");
            btnEdit.Content         = Strings.Get("Btn_Edit");
            btnDelete.Content       = Strings.Get("Btn_Delete");
            btnReport.Content       = Strings.Get("Btn_Report");
            btnBack.Content         = Strings.Get("Btn_Back");
            btnResetFilters.Content = Strings.Get("Btn_ResetFilters");

            // Search placeholder
            PlaceholderTextBlock.Text = Strings.Get("Placeholder_Search");

            // Filter ComboBox items
            cmbAll.Content         = Strings.Get("Filter_AllStatuses");
            cmbAvailable.Content   = Strings.Get("Filter_Available");
            cmbReserved.Content    = Strings.Get("Filter_Reserved");
            cmbOccupied.Content    = Strings.Get("Filter_Occupied");
            cmbMaintenance.Content = Strings.Get("Filter_Maintenance");

            // DataGrid column headers
            colName.Header   = Strings.Get("Col_Name");
            colPrice.Header  = Strings.Get("Col_Price");
            colStatus.Header = Strings.Get("Col_Status");
            colStart.Header  = Strings.Get("Col_Start");
            colEnd.Header    = Strings.Get("Col_End");
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (Window.GetWindow(this) is DashboardWindow dashboard)
            {
                dashboard.mainFrame.Navigate(new DashboardPage(_currentUser));
            }
        }

        private void LoadResources()
        {
            _resources = _dataService.GetResourcesByType(_resourceType);
            resourcesGrid.ItemsSource = _resources;
        }

        private void AddResource(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            var dialog = new ResourceEditDialog(_resourceType, null);
            if (dialog.ShowDialog() == true)
            {
                _dataService.AddResource(dialog.Resource);
                LoadResources();
                ShowNotification("Ресурс успішно додано");
            }
        }

        private void EditResource(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (resourcesGrid.SelectedItem is Resource selectedResource)
            {
                var dialog = new ResourceEditDialog(_resourceType, selectedResource);
                if (dialog.ShowDialog() == true)
                {
                    _dataService.UpdateResource(dialog.Resource);
                    LoadResources();
                    ShowNotification("Ресурс успішно оновлено");
                }
            }
            else
            {
                ShowWarning("Будь ласка, виберіть ресурс для редагування");
            }
        }

        private void DeleteResource(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (resourcesGrid.SelectedItem is Resource selectedResource)
            {
                if (MessageBox.Show("Ви впевнені, що хочете видалити цей ресурс?", "Підтвердження",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _dataService.DeleteResource(selectedResource.Id);
                    LoadResources();
                    ShowNotification("Ресурс успішно видалено");
                }
            }
            else
            {
                ShowWarning("Будь ласка, виберіть ресурс для видалення");
            }
        }

        private void GenerateReport(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            var reportDialog = new ReportDialog(_resourceType);
            reportDialog.ShowDialog();
        }

        private void ResourcesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
            PlaceholderTextBlock.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            SearchTextBox.Text = string.Empty;
            StatusFilterComboBox.SelectedIndex = 0;
            ApplyFilters();
            ShowNotification("Фільтри скинуто");
        }


        private void ResourcesGrid_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (resourcesGrid.SelectedItem is Resource selectedResource)
            {
                var dialog = new ResourceEditDialog(_resourceType, selectedResource);
                if (dialog.ShowDialog() == true)
                {
                    _dataService.UpdateResource(dialog.Resource);
                    LoadResources();
                }
            }
        }
        private void ApplyFilters()
        {
            if (_resources == null) return;

            var filtered = _resources.AsEnumerable();

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(r =>
                    r.Name.ToLower().Contains(searchText) ||
                    (r.Description != null && r.Description.ToLower().Contains(searchText)));
            }

            if (StatusFilterComboBox.SelectedIndex > 0 && StatusFilterComboBox.SelectedIndex <= Enum.GetValues(typeof(ReservationStatus)).Length)
            {
                var selectedStatus = (ReservationStatus)(StatusFilterComboBox.SelectedIndex - 1);
                filtered = filtered.Where(r => r.Status == selectedStatus);
            }

            resourcesGrid.ItemsSource = filtered.ToList();
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;
        }

        private void ShowNotification(string message) { }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}