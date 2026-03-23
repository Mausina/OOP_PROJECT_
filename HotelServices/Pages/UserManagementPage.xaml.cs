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
        private readonly LanguageService _lang = LanguageService.Instance;
        private List<User> _users;

        public UserManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _dataService = new DataService();
            DataContext = this;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
            LoadUsers();
        }

        private void ApplyLanguage()
        {
            lblPageTitle.Text      = Strings.Get("Dashboard_Users");
            btnAddUser.Content     = Strings.Get("Btn_AddUser");
            btnEdit.Content        = Strings.Get("Btn_Edit");
            btnDelete.Content      = Strings.Get("Btn_Delete");
            btnBack.Content        = Strings.Get("Btn_Back");
            btnResetFilters.Content = Strings.Get("Btn_ResetFilters");

            PlaceholderTextBlock.Text = Strings.Get("Placeholder_Search");

            cmbAllRoles.Content = Strings.Get("Filter_AllRoles");
            cmbAdmin.Content    = Strings.Get("Filter_Admin");
            cmbManager.Content  = Strings.Get("Filter_Manager");

            colUsername.Header = Strings.Get("Col_Username");
            colFullName.Header = Strings.Get("Col_FullName");
            colRole.Header     = Strings.Get("Col_Role");
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
            if (_users == null) _users = new List<User>();
            usersGrid.ItemsSource = _users;
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

        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

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
            RoleFilterComboBox.SelectedIndex = 0;
            ApplyFilters();
            ShowNotification("Фільтри скинуто");
        }

        private void ApplyFilters()
        {
            if (_users == null) return;

            var filtered = _users.AsEnumerable();

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(u =>
                    (u.Username != null && u.Username.ToLower().Contains(searchText)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(searchText)));
            }

            if (RoleFilterComboBox.SelectedIndex > 0 &&
                RoleFilterComboBox.SelectedIndex <= Enum.GetValues(typeof(UserRole)).Length)
            {
                var selectedRole = (UserRole)(RoleFilterComboBox.SelectedIndex - 1);
                filtered = filtered.Where(u => u.Role == selectedRole);
            }

            usersGrid.ItemsSource = filtered.ToList();
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