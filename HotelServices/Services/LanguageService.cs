using System;

namespace HotelServices.Services
{
    public enum AppLanguage { UA, EN }

    public class LanguageService
    {
        private static LanguageService _instance;
        public static LanguageService Instance => _instance ??= new LanguageService();

        private AppLanguage _currentLanguage = AppLanguage.UA;
        public AppLanguage CurrentLanguage => _currentLanguage;

        public event EventHandler LanguageChanged;

        private LanguageService() { }

        public void Toggle()
        {
            _currentLanguage = _currentLanguage == AppLanguage.UA ? AppLanguage.EN : AppLanguage.UA;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public string ButtonText => _currentLanguage == AppLanguage.UA ? "🌐 EN" : "🌐 UA";
    }
}
