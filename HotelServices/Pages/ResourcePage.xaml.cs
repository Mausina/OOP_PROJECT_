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
        private List<Resource> _resources;

        public ResourcePage(ResourceType resourceType, User currentUser)
        {
            InitializeComponent();
            _resourceType = resourceType;
            _currentUser = currentUser;
            _dataService = new DataService();
            DataContext = this;

            SetTitle();
            LoadResources();
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (Window.GetWindow(this) is DashboardWindow dashboard)
            {
                dashboard.mainFrame.Navigate(new DashboardPage(_currentUser));
            }
        }

        private void SetTitle()
        {
            string title = _resourceType switch
            {
                ResourceType.Apartment => "Управління апартаментами",
                ResourceType.ConferenceRoom => "Бронювання конференц-залів",
                ResourceType.ParkingSpace => "Облік паркомісць",
                ResourceType.RestaurantTable => "Бронювання ресторану",
                ResourceType.AdditionalService => "Додаткові послуги",
                _ => "Ресурси"
            };

            lblTitle.Text = title;
        }

        private void LoadResources()
        {
            _resources = _dataService.GetResourcesByType(_resourceType);
            resourcesGrid.ItemsSource = _resources;

            // Стилізація даних в таблиці на основі статусу (можна розширити)
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

        private void ResourcesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Оновлення інтерфейсу при виборі елемента
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
            // Показати або сховати плейсхолдер
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

        private void ApplyFilters()
        {
            if (_resources == null) return;

            var filtered = _resources.AsEnumerable();

            // Пошук за текстом
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(r =>
                    r.Name.ToLower().Contains(searchText) ||
                    (r.Description != null && r.Description.ToLower().Contains(searchText)));
            }

            // Фільтр за статусом
            if (StatusFilterComboBox.SelectedIndex > 0 && StatusFilterComboBox.SelectedIndex <= Enum.GetValues(typeof(ReservationStatus)).Length)
            {
                var selectedStatus = (ReservationStatus)(StatusFilterComboBox.SelectedIndex - 1);
                filtered = filtered.Where(r => r.Status == selectedStatus);
            }

            // Оновлення відображення даних
            resourcesGrid.ItemsSource = filtered.ToList();

        }

        // Метод для анімації натискання кнопки
        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;

        }

        private void ShowNotification(string message)
        {

        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}