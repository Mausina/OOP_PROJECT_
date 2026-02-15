using HotelServices.Models;
using HotelServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HotelServices.Pages
{
    public partial class UserManagementPage : Page
    {
        private readonly User _currentUser;
        private readonly DataService _dataService;
        private List<User> _users;

        public UserManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _dataService = new DataService();
            DataContext = this;

            LoadUsers();
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (Window.GetWindow(this) is DashboardWindow dashboard)
            {
                dashboard.mainFrame.Navigate(new DashboardPage(_currentUser));
            }
        }

        private void LoadUsers()
        {
            _users = _dataService.GetAllUsers();
            if (_users == null)
            {
                _users = new List<User>();
            }
            usersGrid.ItemsSource = _users;

            // Стилізація даних в таблиці на основі статусу
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            var dialog = new UserEditDialog(null);
            if (dialog.ShowDialog() == true)
            {
                _dataService.AddUser(dialog.User);
                LoadUsers();
                ShowNotification("Користувача успішно додано");
            }
        }

        private void EditUser(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (usersGrid.SelectedItem is User selectedUser)
            {
                var dialog = new UserEditDialog(selectedUser);
                if (dialog.ShowDialog() == true)
                {
                    _dataService.UpdateUser(dialog.User);
                    LoadUsers();
                    ShowNotification("Дані користувача успішно оновлено");
                }
            }
            else
            {
                ShowWarning("Будь ласка, виберіть користувача для редагування");
            }
        }

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick(sender as Button);
            if (usersGrid.SelectedItem is User selectedUser)
            {
                if (selectedUser.Id == _currentUser.Id)
                {
                    ShowWarning("Ви не можете видалити самого себе");
                    return;
                }

                if (MessageBox.Show("Ви впевнені, що хочете видалити цього користувача?", "Підтвердження",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _dataService.DeleteUser(selectedUser.Id);
                    LoadUsers();
                    ShowNotification("Користувача успішно видалено");
                }
            }
            else
            {
                ShowWarning("Будь ласка, виберіть користувача для видалення");
            }
        }

        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            RoleFilterComboBox.SelectedIndex = 0;
            ApplyFilters();
            ShowNotification("Фільтри скинуто");
        }

        private void ApplyFilters()
        {
            if (_users == null) return;

            var filtered = _users.AsEnumerable();

            // Пошук за текстом
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(u =>
                    (u.Username != null && u.Username.ToLower().Contains(searchText)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(searchText)));
            }

            // Фільтр за роллю
            if (RoleFilterComboBox.SelectedIndex > 0 &&
                RoleFilterComboBox.SelectedIndex <= Enum.GetValues(typeof(UserRole)).Length)
            {
                var selectedRole = (UserRole)(RoleFilterComboBox.SelectedIndex - 1);
                filtered = filtered.Where(u => u.Role == selectedRole);
            }

            // Оновлення відображення даних
            usersGrid.ItemsSource = filtered.ToList();
        }

        // Метод для анімації натискання кнопки
        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;

        }

        // Методи для показу повідомлень користувачу
        private void ShowNotification(string message)
        {

        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}