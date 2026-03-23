using HotelServices.Models;
using HotelServices.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HotelServices
{
    public partial class UserEditDialog : Window
    {
        public User User { get; private set; }
        private readonly LanguageService _lang = LanguageService.Instance;
        private readonly bool _isNew;

        public UserEditDialog(User user)
        {
            InitializeComponent();
            _isNew = user == null;

            if (_isNew)
            {
                User = new User();
            }
            else
            {
                User = user;
                txtFullName.Text = user.FullName;
                txtUsername.Text = user.Username;
            }

            cmbRole.ItemsSource = System.Enum.GetValues(typeof(UserRole));
            cmbRole.SelectedItem = user?.Role ?? UserRole.Manager;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();

            AnimateDialogLoad();
        }

        private void ApplyLanguage()
        {
            lblTitle.Text      = Strings.Get(_isNew ? "UserEdit_TitleAdd" : "UserEdit_TitleEdit");
            lblFullName.Text   = Strings.Get("UserEdit_FullName");
            lblUsername.Text   = Strings.Get("UserEdit_Username");
            lblPassword.Text   = Strings.Get("UserEdit_Password");
            lblRole.Text       = Strings.Get("UserEdit_Role");
            btnSave.Content    = Strings.Get("Btn_Save");
            btnCancel.Content  = Strings.Get("Btn_Cancel");
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