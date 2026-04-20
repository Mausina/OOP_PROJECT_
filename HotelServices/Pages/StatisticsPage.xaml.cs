using HotelServices.Models;
using HotelServices.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace HotelServices.Pages
{
    public class StatCardViewModel
    {
        public string Title { get; set; } = "";
        public string Value { get; set; } = "";
        public string Color { get; set; } = "#2D5F8B";
    }

    public partial class StatisticsPage : Page
    {
        private readonly User _currentUser;
        private readonly DataService _dataService;
        private readonly LanguageService _lang = LanguageService.Instance;
        private readonly List<IStatisticsProvider> _providers;

        public StatisticsPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _dataService = new DataService();
            DataContext = this;

            _providers = new List<IStatisticsProvider>
            {
                new TotalRevenueProvider(),
                new BookingCountProvider(),
                new OccupancyRateProvider(),
                new ActiveResourcesProvider()
            };

            dpFrom.SelectedDate = DateTime.Today.AddDays(-30);
            dpTo.SelectedDate = DateTime.Today;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
            LoadStatistics();
        }

        private void ApplyLanguage()
        {
            lblTitle.Text = "📊 " + Strings.Get("Stats_Title");
            lblFrom.Text = Strings.Get("Stats_From");
            lblTo.Text = Strings.Get("Stats_To");
            btnRefresh.Content = Strings.Get("Stats_Refresh");
            btnBack.Content = Strings.Get("Btn_Back");
            lblTopTitle.Text = Strings.Get("Stats_TopTitle");

            colRank.Header = "#";
            colNameTop.Header = Strings.Get("Col_Name");
            colTypeTop.Header = Strings.Get("Stats_Type");
            colRevenue.Header = Strings.Get("Report_ColIncome");
        }

        private void LoadStatistics()
        {
            try
            {
                var from = dpFrom.SelectedDate ?? DateTime.Today.AddDays(-30);
                var to = dpTo.SelectedDate ?? DateTime.Today;

                if (from > to)
                {
                    MessageBox.Show("Start date must be before end date",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var resources = _dataService.GetAllResources();

                var cards = new ObservableCollection<StatCardViewModel>();
                foreach (IStatisticsProvider provider in _providers)
                {
                    cards.Add(new StatCardViewModel
                    {
                        Title = provider.Title,
                        Value = provider.Compute(resources, from, to),
                        Color = provider.AccentColor
                    });
                }
                statsItemsControl.ItemsSource = cards;

                var topList = TopResourcesCalculator.GetTop(resources, from, to, 5);
                topGrid.ItemsSource = topList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load statistics: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is DashboardWindow dashboard)
            {
                dashboard.mainFrame.Navigate(new DashboardPage(_currentUser));
            }
        }
    }
}