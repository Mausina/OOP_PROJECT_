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
        private readonly LanguageService _lang = LanguageService.Instance;

        public ReportDialog(ResourceType resourceType)
        {
            InitializeComponent();
            _dataService = new DataService();
            _resourceType = resourceType;

            dpStartDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpEndDate.SelectedDate = DateTime.Today;

            cmbResourceType.ItemsSource = Enum.GetValues(typeof(ResourceType));
            cmbResourceType.SelectedItem = _resourceType;

            _lang.LanguageChanged += (s, e) => ApplyLanguage();
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            lblTitle.Text        = Strings.Get("Report_Title");
            lblFrom.Text         = Strings.Get("Report_PeriodFrom");
            lblTo.Text           = Strings.Get("Report_PeriodTo");
            lblResType.Text      = Strings.Get("Report_ResourceType");
            colName.Header       = Strings.Get("Report_ColName");
            colStatus.Header     = Strings.Get("Report_ColStatus");
            colStart.Header      = Strings.Get("Report_ColStart");
            colEnd.Header        = Strings.Get("Report_ColEnd");
            colDuration.Header   = Strings.Get("Report_ColDuration");
            colIncome.Header     = Strings.Get("Report_ColIncome");
            btnGenerate.Content  = Strings.Get("Btn_Generate");
            btnExportPdf.Content = Strings.Get("Btn_ExportPdf");
            btnCancel.Content    = Strings.Get("Btn_Cancel");
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show(Strings.Get("Error_SelectPeriod"), Strings.Get("Error_Title"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show(Strings.Get("Error_DateOrder"), Strings.Get("Error_Title"),
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Get("Error_ReportGen")}: {ex.Message}", Strings.Get("Error_Title"),
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
                    FileName = $"Report_{_resourceType}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
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

                var title = new Paragraph($"Report for {GetResourceTypeNameEnglish(_resourceType)}", fontTitle);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                var period = new Paragraph($"Period: from {dpStartDate.SelectedDate.Value:dd.MM.yyyy} to {dpEndDate.SelectedDate.Value:dd.MM.yyyy}", font);
                period.Alignment = Element.ALIGN_CENTER;
                document.Add(period);

                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                float[] columnWidths = new float[] { 0.5f, 1.5f, 2f, 2f, 1.5f, 1.5f };
                table.SetWidths(columnWidths);

                string[] columnHeaders = { "#", "Status", "Start Date", "End Date", "Duration (h)", "Income (UAH)" };

                foreach (var headerText in columnHeaders)
                {
                    var header = new PdfPCell(new Phrase(headerText, fontHeader));
                    header.BackgroundColor = new BaseColor(220, 220, 220);
                    header.HorizontalAlignment = Element.ALIGN_CENTER;
                    header.Padding = 5;
                    table.AddCell(header);
                }

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

                document.Add(new Paragraph("\n"));
                var totalIncome = reportDataGrid.Items.Cast<Resource>().Sum(r => r.TotalIncome);
                var summary = new Paragraph($"Total income: {totalIncome:N2} UAH", fontSummary);
                summary.Alignment = Element.ALIGN_RIGHT;
                document.Add(summary);

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
                case ResourceType.Apartment: return Strings.Get("ResType_Apartment");
                case ResourceType.ConferenceRoom: return Strings.Get("ResType_Conference");
                case ResourceType.ParkingSpace: return Strings.Get("ResType_Parking");
                case ResourceType.RestaurantTable: return Strings.Get("ResType_Restaurant");
                case ResourceType.AdditionalService: return Strings.Get("ResType_Services");
                default: return Strings.Get("ResType_Default");
            }
        }
    }
}