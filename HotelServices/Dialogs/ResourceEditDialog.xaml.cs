using HotelServices.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HotelServices.Dialogs
{
    public partial class ResourceEditDialog : Window
    {
        public Resource Resource { get; private set; }
        private readonly ResourceType _resourceType;

        public ResourceEditDialog(ResourceType resourceType, Resource resource)
        {
            InitializeComponent();
            _resourceType = resourceType;

            // Ініціалізація вибору часу
            InitializeTimeComboBoxes();

            if (resource == null)
            {
                Resource = new Resource
                {
                    Type = resourceType,
                    Status = ReservationStatus.Available,
                    StartDate = null,
                    EndDate = null
                };
                lblTitle.Text = "Додати новий ресурс";
            }
            else
            {
                Resource = resource;
                lblTitle.Text = "Редагування ресурсу";
                txtName.Text = resource.Name;
                txtDescription.Text = resource.Description;
                txtPrice.Text = resource.Price.ToString();

                if (resource.StartDate.HasValue)
                {
                    dpStartDate.SelectedDate = resource.StartDate.Value.Date;
                    cmbStartHour.SelectedItem = resource.StartDate.Value.Hour;
                    cmbStartMinute.SelectedItem = resource.StartDate.Value.Minute;
                }

                if (resource.EndDate.HasValue)
                {
                    dpEndDate.SelectedDate = resource.EndDate.Value.Date;
                    cmbEndHour.SelectedItem = resource.EndDate.Value.Hour;
                    cmbEndMinute.SelectedItem = resource.EndDate.Value.Minute;
                }
            }

            cmbStatus.ItemsSource = Enum.GetValues(typeof(ReservationStatus));
            cmbStatus.SelectedItem = resource?.Status ?? ReservationStatus.Available;
            cmbStatus.SelectionChanged += CmbStatus_SelectionChanged;

            UpdateDatePickersVisibility();
            LoadAdditionalFields();

            // Анімація появи вікна
            AnimateDialogLoad();
        }

        private void AnimateDialogLoad()
        {
            // Анімація заголовка
            var titleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            lblTitle.BeginAnimation(UIElement.OpacityProperty, titleAnimation);

            // Анімація появи основного вмісту
            var contentAnimation = new DoubleAnimation
            {
                From = 0.7,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(400)
            };
            this.BeginAnimation(UIElement.OpacityProperty, contentAnimation);
        }

        private void InitializeTimeComboBoxes()
        {
            // Заповнення годин (0-23)
            for (int i = 0; i < 24; i++)
            {
                cmbStartHour.Items.Add(i);
                cmbEndHour.Items.Add(i);
            }

            // Заповнення хвилин (00, 15, 30, 45)
            cmbStartMinute.Items.Add(0);
            cmbStartMinute.Items.Add(15);
            cmbStartMinute.Items.Add(30);
            cmbStartMinute.Items.Add(45);

            cmbEndMinute.Items.Add(0);
            cmbEndMinute.Items.Add(15);
            cmbEndMinute.Items.Add(30);
            cmbEndMinute.Items.Add(45);

            // Встановлення поточного часу за замовчуванням
            var now = DateTime.Now;
            cmbStartHour.SelectedItem = now.Hour;
            cmbStartMinute.SelectedItem = (now.Minute / 15) * 15; // Округлюємо до 15 хв

            cmbEndHour.SelectedItem = now.AddHours(1).Hour;
            cmbEndMinute.SelectedItem = (now.AddHours(1).Minute / 15) * 15;
        }

        private DateTime? GetDateTimeFromPickers(DatePicker datePicker, ComboBox hourCombo, ComboBox minuteCombo)
        {
            if (datePicker.SelectedDate == null || hourCombo.SelectedItem == null || minuteCombo.SelectedItem == null)
                return null;

            return new DateTime(
                datePicker.SelectedDate.Value.Year,
                datePicker.SelectedDate.Value.Month,
                datePicker.SelectedDate.Value.Day,
                (int)hourCombo.SelectedItem,
                (int)minuteCombo.SelectedItem,
                0);
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDatePickersVisibility();
        }

        private void UpdateDatePickersVisibility()
        {
            var currentStatus = (ReservationStatus)cmbStatus.SelectedItem;
            bool datesRequired = currentStatus != ReservationStatus.Available;

            Visibility visibility = datesRequired ? Visibility.Visible : Visibility.Collapsed;

            // Використовуємо StackPanel для групового керування видимістю
            startDatePanel.Visibility = visibility;
            endDatePanel.Visibility = visibility;
        }

        private void LoadAdditionalFields()
        {
            additionalFieldsPanel.Children.Clear();

            // Заголовок для розділу додаткових полів
            TextBlock header = new TextBlock
            {
                Text = "Додаткова інформація",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D5F8B")),
                Margin = new Thickness(0, 0, 0, 10)
            };
            additionalFieldsPanel.Children.Add(header);

            switch (_resourceType)
            {
                case ResourceType.Apartment:
                    AddField("Кількість кімнат:", "txtRooms", Resource?.Rooms?.ToString() ?? "");
                    AddCheckBox("Люкс:", "chkIsLuxury", Resource?.IsLuxury ?? false);
                    break;
                case ResourceType.ConferenceRoom:
                    AddField("Місткість:", "txtCapacity", Resource?.Capacity?.ToString() ?? "");
                    break;
                case ResourceType.ParkingSpace:
                    AddField("Номер паркомісця:", "txtParkingNumber", Resource?.ParkingNumber ?? "");
                    break;
                case ResourceType.RestaurantTable:
                    AddField("Номер столу:", "txtTableNumber", Resource?.TableNumber?.ToString() ?? "");
                    AddField("Кількість гостей:", "txtGuests", Resource?.Guests?.ToString() ?? "");
                    break;
                case ResourceType.AdditionalService:
                    AddField("Тип послуги:", "txtServiceType", Resource?.ServiceType ?? "");
                    break;
            }
        }

        private void AddField(string label, string fieldName, string value)
        {
            var labelBlock = new TextBlock
            {
                Text = label,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D5F8B")),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var textBox = new TextBox
            {
                Name = fieldName,
                Text = value,
                Height = 35,
                BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCDDEE")),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8, 5, 8, 5),
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 14,
                Background = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };

            additionalFieldsPanel.Children.Add(labelBlock);
            additionalFieldsPanel.Children.Add(textBox);
        }

        private void AddCheckBox(string label, string fieldName, bool isChecked)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 15)
            };

            var labelBlock = new TextBlock
            {
                Text = label,
                Width = 100,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D5F8B")),
                FontSize = 14
            };

            var checkBox = new CheckBox
            {
                Name = fieldName,
                IsChecked = isChecked,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };

            stackPanel.Children.Add(labelBlock);
            stackPanel.Children.Add(checkBox);

            additionalFieldsPanel.Children.Add(stackPanel);
        }

        private TextBox FindTextBox(string name)
        {
            foreach (var child in additionalFieldsPanel.Children)
            {
                if (child is TextBox textBox && textBox.Name == name)
                {
                    return textBox;
                }
            }
            return null;
        }

        private CheckBox FindCheckBox(string name)
        {
            foreach (var child in additionalFieldsPanel.Children)
            {
                if (child is StackPanel panel)
                {
                    foreach (var panelChild in panel.Children)
                    {
                        if (panelChild is CheckBox checkBox && checkBox.Name == name)
                        {
                            return checkBox;
                        }
                    }
                }
            }
            return null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowErrorMessage("Будь ласка, введіть назву ресурсу");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                ShowErrorMessage("Будь ласка, введіть коректну ціну");
                return;
            }

            var currentStatus = (ReservationStatus)cmbStatus.SelectedItem;
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (currentStatus != ReservationStatus.Available)
            {
                startDate = GetDateTimeFromPickers(dpStartDate, cmbStartHour, cmbStartMinute);
                endDate = GetDateTimeFromPickers(dpEndDate, cmbEndHour, cmbEndMinute);

                if (startDate == null || endDate == null)
                {
                    ShowErrorMessage("Будь ласка, вкажіть дату та час початку і завершення");
                    return;
                }

                if (startDate >= endDate)
                {
                    ShowErrorMessage("Час початку повинен бути раніше часу завершення");
                    return;
                }
            }

            Resource.Name = txtName.Text;
            Resource.Description = txtDescription.Text;
            Resource.Price = price;
            Resource.Status = currentStatus;
            Resource.StartDate = startDate;
            Resource.EndDate = endDate;

            switch (_resourceType)
            {
                case ResourceType.Apartment:
                    if (int.TryParse(FindTextBox("txtRooms")?.Text, out int rooms))
                        Resource.Rooms = rooms;
                    Resource.IsLuxury = FindCheckBox("chkIsLuxury")?.IsChecked ?? false;
                    break;
                case ResourceType.ConferenceRoom:
                    if (int.TryParse(FindTextBox("txtCapacity")?.Text, out int capacity))
                        Resource.Capacity = capacity;
                    break;
                case ResourceType.ParkingSpace:
                    Resource.ParkingNumber = FindTextBox("txtParkingNumber")?.Text;
                    break;
                case ResourceType.RestaurantTable:
                    if (int.TryParse(FindTextBox("txtTableNumber")?.Text, out int tableNumber))
                        Resource.TableNumber = tableNumber;
                    if (int.TryParse(FindTextBox("txtGuests")?.Text, out int guests))
                        Resource.Guests = guests;
                    break;
                case ResourceType.AdditionalService:
                    Resource.ServiceType = FindTextBox("txtServiceType")?.Text;
                    break;
            }

            // Анімація при збереженні
            AnimateButtonClick(sender as Button);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Анімація при скасуванні
            AnimateButtonClick(sender as Button);

            DialogResult = false;
            Close();
        }

        private void AnimateButtonClick(Button button)
        {
            if (button == null) return;

            // Створюємо стислу анімацію для кнопки при натисканні
            var scaleDownAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.95,
                Duration = TimeSpan.FromMilliseconds(100)
            };

            var scaleUpAnimation = new DoubleAnimation
            {
                From = 0.95,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(100),
                BeginTime = TimeSpan.FromMilliseconds(100)
            };

            var scaleTransform = new System.Windows.Media.ScaleTransform(1, 1);
            button.RenderTransform = scaleTransform;

            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleDownAnimation);
            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleDownAnimation);

            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleUpAnimation);
            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleUpAnimation);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}