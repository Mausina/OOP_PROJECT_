using HotelServices.Models;
using HotelServices.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HotelServices.Dialogs
{
    public partial class ReportDialog : Window
    {
        private readonly DataService _dataService;
        private readonly ResourceType _resourceType;

        public ReportDialog(ResourceType resourceType)
        {
            InitializeComponent();
            _dataService = new DataService();
            _resourceType = resourceType;

            // Встановлюємо початкові дати
            dpStartDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpEndDate.SelectedDate = DateTime.Today;

            // Встановлюємо тип ресурсу
            cmbResourceType.ItemsSource = Enum.GetValues(typeof(ResourceType));
            cmbResourceType.SelectedItem = _resourceType;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Будь ласка, вкажіть період для звіту", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата початку не може бути пізніше дати завершення", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var reportData = _dataService.GetReportData(
                    _resourceType,
                    dpStartDate.SelectedDate.Value,
                    dpEndDate.SelectedDate.Value);

                reportDataGrid.ItemsSource = reportData;

                MessageBox.Show($"Звіт по {GetResourceTypeName(_resourceType)} за період з " +
                    $"{dpStartDate.SelectedDate.Value.ToShortDateString()} по " +
                    $"{dpEndDate.SelectedDate.Value.ToShortDateString()} сформовано",
                    "Звіт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при формуванні звіту: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (reportDataGrid.Items.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту", "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Зберегти звіт як PDF",
                    FileName = $"Звіт_{_resourceType}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    GeneratePdfReport(saveFileDialog.FileName);
                    MessageBox.Show("Звіт успішно експортовано до PDF", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при експорті в PDF: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePdfReport(string filePath)
        {
            // Реєструємо шрифт з підтримкою кирилиці (Arial Unicode MS або інший)
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(baseFont, 10);
            Font fontBold = new Font(baseFont, 10, Font.BOLD);
            Font fontTitle = new Font(baseFont, 18, Font.BOLD);
            Font fontHeader = new Font(baseFont, 10, Font.BOLD);
            Font fontSummary = new Font(baseFont, 12, Font.BOLD);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Заголовок звіту англійською
                var title = new Paragraph($"Report for {GetResourceTypeNameEnglish(_resourceType)}", fontTitle);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                // Період звіту
                var period = new Paragraph($"Period: from {dpStartDate.SelectedDate.Value:dd.MM.yyyy} to {dpEndDate.SelectedDate.Value:dd.MM.yyyy}", font);
                period.Alignment = Element.ALIGN_CENTER;
                document.Add(period);

                document.Add(new Paragraph("\n"));

                // Таблиця з даними
                PdfPTable table = new PdfPTable(6); // 6 стовпців
                table.WidthPercentage = 100;
                float[] columnWidths = new float[] { 0.5f, 1.5f, 2f, 2f, 1.5f, 1.5f };
                table.SetWidths(columnWidths);

                // Англійські заголовки стовпців
                string[] columnHeaders = new string[]
                {
            "#",
            "Status",
            "Start Date",
            "End Date",
            "Duration (h)",
            "Income (UAH)"
                };

                // Додаємо заголовки
                foreach (var headerText in columnHeaders)
                {
                    var header = new PdfPCell(new Phrase(headerText, fontHeader));
                    header.BackgroundColor = new BaseColor(220, 220, 220);
                    header.HorizontalAlignment = Element.ALIGN_CENTER;
                    header.Padding = 5;
                    table.AddCell(header);
                }

                // Дані з таблиці
                int rowNumber = 1;
                foreach (var item in reportDataGrid.Items)
                {
                    if (item is Resource resource)
                    {
                        table.AddCell(new Phrase(rowNumber++.ToString(), font));
                        table.AddCell(new Phrase(resource.Status.ToString(), font));
                        table.AddCell(new Phrase(resource.StartDate?.ToString("dd.MM.yyyy HH:mm") ?? "-", font));
                        table.AddCell(new Phrase(resource.EndDate?.ToString("dd.MM.yyyy HH:mm") ?? "-", font));
                        table.AddCell(new Phrase(resource.OccupancyDuration.ToString("N1"), font));
                        table.AddCell(new Phrase(resource.TotalIncome.ToString("N2"), font));
                    }
                }

                document.Add(table);

                // Підсумки англійською
                document.Add(new Paragraph("\n"));
                var totalIncome = reportDataGrid.Items.Cast<Resource>().Sum(r => r.TotalIncome);
                var summary = new Paragraph($"Total income: {totalIncome:N2} UAH", fontSummary);
                summary.Alignment = Element.ALIGN_RIGHT;
                document.Add(summary);

                // Футер
                var footer = new Paragraph($"Report generated: {DateTime.Now:dd.MM.yyyy HH:mm}", font);
                footer.Alignment = Element.ALIGN_RIGHT;
                document.Add(footer);

                document.Close();
            }
        }

        private string GetResourceTypeNameEnglish(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Apartment: return "apartments";
                case ResourceType.ConferenceRoom: return "conference rooms";
                case ResourceType.ParkingSpace: return "parking spaces";
                case ResourceType.RestaurantTable: return "restaurant tables";
                case ResourceType.AdditionalService: return "additional services";
                default: return "resources";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string GetResourceTypeName(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Apartment: return "апартаментам";
                case ResourceType.ConferenceRoom: return "конференц-залам";
                case ResourceType.ParkingSpace: return "паркомісцям";
                case ResourceType.RestaurantTable: return "ресторанним столам";
                case ResourceType.AdditionalService: return "додатковим послугам";
                default: return "ресурсам";
            }
        }
    }
}