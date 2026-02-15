using HotelServices.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HotelServices
{
    public partial class UserEditDialog : Window
    {
        public User User { get; private set; }

        public UserEditDialog(User user)
        {
            InitializeComponent();

            if (user == null)
            {
                User = new User();
                lblTitle.Text = "Додати нового користувача";
            }
            else
            {
                User = user;
                lblTitle.Text = "Редагування користувача";
                txtFullName.Text = user.FullName;
                txtUsername.Text = user.Username;
            }

            cmbRole.ItemsSource = System.Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = user?.Role ?? UserRole.Manager;

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowErrorMessage("Будь ласка, заповніть всі поля");
                return;
            }

            User.FullName = txtFullName.Text;
            User.Username = txtUsername.Text;
            User.Password = txtPassword.Password;
            User.Role = (UserRole)cmbRole.SelectedItem;

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