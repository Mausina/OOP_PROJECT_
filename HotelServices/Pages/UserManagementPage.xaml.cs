using HotelServices.Models;
using HotelServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HotelServices.Pages
{
    public partial class UserManagementPage : Page
    {
        private readonly User _currentUser;
        private readonly DataService _dataService;
        private readonly LanguageService _lang = LanguageService.Instance;
        private List<User> _users = new List<User>();

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
            lblPageTitle.Text = Strings.Get("Dashboard_Users");
            btnAddUser.Content = Strings.Get("Btn_AddUser");
            btnEdit.Content = Strings.Get("Btn_Edit");
            btnDelete.Content = Strings.Get("Btn_Delete");
            btnBack.Content = Strings.Get("Btn_Back");
            btnResetFilters.Content = Strings.Get("Btn_ResetFilters");

            PlaceholderTextBlock.Text = Strings.Get("Placeholder_Search");

            cmbAllRoles.Content = Strings.Get("Filter_AllRoles");
            cmbAdmin.Content = Strings.Get("Filter_Admin");
            cmbManager.Content = Strings.Get("Filter_Manager");

            colUsername.Header = Strings.Get("Col_Username");
            colFullName.Header = Strings.Get("Col_FullName");
            colRole.Header = Strings.Get("Col_Role");
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is DashboardWindow dashboard)
            {
                dashboard.mainFrame.Navigate(new DashboardPage(_currentUser));
            }
        }

        private void LoadUsers()
        {
            try
            {
                _users = _dataService.GetAllUsers() ?? new List<User>();
                usersGrid.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            var dialog = new UserEditDialog(null);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _dataService.AddUser(dialog.User);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add user: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditUser(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem is User selectedUser)
            {
                var dialog = new UserEditDialog(selectedUser);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        _dataService.UpdateUser(dialog.User);
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to update user: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            if (usersGrid.SelectedItem is User selectedUser)
            {
                if (selectedUser.Id == _currentUser.Id)
                {
                    MessageBox.Show("You cannot delete yourself",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dataService.DeleteUser(selectedUser.Id);
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete user: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            SearchTextBox.Text = string.Empty;
            RoleFilterComboBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_users == null) return;

            IEnumerable<User> filtered = _users;

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
    }
}