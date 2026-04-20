using System.Collections.Generic;

namespace HotelServices.Services
{
    public static class Strings
    {
        private static readonly Dictionary<string, string> UA = new Dictionary<string, string>
        {
            // MainWindow (Login)
            ["Login_Title"]              = "Авторизація",
            ["Login_Username"]           = "Логін:",
            ["Login_Password"]           = "Пароль:",
            ["Login_Button"]             = "Увійти",
            ["Login_Error_Empty"]        = "Введіть логін та пароль",
            ["Login_Error_Invalid"]      = "Невірний логін або пароль",

            // DashboardPage
            ["Dashboard_Title"]          = "Головна панель управління",
            ["Dashboard_Subtitle"]       = "Готельна система управління сервісами",
            ["Dashboard_Apartments"]     = "Управління апартаментами",
            ["Dashboard_Apartments_Sub"] = "Перегляд та редагування номерів",
            ["Dashboard_Conference"]     = "Бронювання конференц-залів",
            ["Dashboard_Conference_Sub"] = "Розклад та місткість залів",
            ["Dashboard_Parking"]        = "Облік паркомісць",
            ["Dashboard_Parking_Sub"]    = "Статус та резервування",
            ["Dashboard_Restaurant"]     = "Бронювання ресторану",
            ["Dashboard_Restaurant_Sub"] = "Столи, дати та гості",
            ["Dashboard_Services"]       = "Додаткові послуги",
            ["Dashboard_Services_Sub"]   = "Спортзал, SPA та інше",
            ["Dashboard_Users"]          = "Управління користувачами",
            ["Dashboard_Users_Sub"]      = "Додавання та редагування",

            // Tooltips DashboardPage
            ["Tooltip_Apartments"]       = "Перейти до управління апартаментами",
            ["Tooltip_Conference"]       = "Перейти до бронювання конференц-залів",
            ["Tooltip_Parking"]          = "Перейти до обліку паркомісць",
            ["Tooltip_Restaurant"]       = "Перейти до бронювання ресторану",
            ["Tooltip_Services"]         = "Перейти до додаткових послуг",
            ["Tooltip_Users"]            = "Перейти до управління користувачами",

            // ResourcePage titles (set by code-behind based on resource type)
            ["Resource_Apartments"]      = "Апартаменти",
            ["Resource_Conference"]      = "Конференц-зали",
            ["Resource_Parking"]         = "Паркомісця",
            ["Resource_Restaurant"]      = "Ресторан",
            ["Resource_Services"]        = "Додаткові послуги",

            // ResourcePage / UserManagementPage shared buttons
            ["Btn_Add"]                  = "Додати",
            ["Btn_Edit"]                 = "Редагувати",
            ["Btn_Delete"]               = "Видалити",
            ["Btn_Back"]                 = "На головну",
            ["Btn_Report"]               = "Звіт",
            ["Btn_ResetFilters"]         = "Скинути фільтри",
            ["Btn_AddUser"]              = "Додати користувача",
            ["Placeholder_Search"]       = "Пошук...",

            // ResourcePage filter combo items
            ["Filter_AllStatuses"]       = "Всі статуси",
            ["Filter_Available"]         = "Доступний",
            ["Filter_Reserved"]          = "Заброньований",
            ["Filter_Occupied"]          = "Зайнятий",
            ["Filter_Maintenance"]       = "На обслуговуванні",

            // UserManagementPage filter combo items
            ["Filter_AllRoles"]          = "Всі ролі",
            ["Filter_Admin"]             = "Адміністратор",
            ["Filter_Manager"]           = "Менеджер",

            // UserManagementPage table headers
            ["Col_Id"]                   = "ID",
            ["Col_Username"]             = "Ім'я користувача",
            ["Col_FullName"]             = "Повне ім'я",
            ["Col_Role"]                 = "Роль",

            // ResourcePage table headers
            ["Col_Name"]                 = "Назва",
            ["Col_Price"]                = "Ціна",
            ["Col_Status"]               = "Статус",
            ["Col_Start"]                = "Початок",
            ["Col_End"]                  = "Завершення",

            // UserEditDialog
            ["UserEdit_TitleAdd"]        = "Додавання користувача",
            ["UserEdit_TitleEdit"]       = "Редагування користувача",
            ["UserEdit_FullName"]        = "Повне ім'я:",
            ["UserEdit_Username"]        = "Логін:",
            ["UserEdit_Password"]        = "Пароль:",
            ["UserEdit_Role"]            = "Роль:",
            ["Btn_Save"]                 = "Зберегти",
            ["Btn_Cancel"]               = "Скасувати",

            // ResourceEditDialog
            ["ResEdit_TitleAdd"]         = "Додавання ресурсу",
            ["ResEdit_TitleEdit"]        = "Редагування ресурсу",
            ["ResEdit_Name"]             = "Назва:",
            ["ResEdit_Description"]      = "Опис:",
            ["ResEdit_Price"]            = "Ціна:",
            ["ResEdit_Status"]           = "Статус:",
            ["ResEdit_StartDate"]        = "Дата та час початку:",
            ["ResEdit_EndDate"]          = "Дата та час завершення:",
            ["ResEdit_Time"]             = "Час:",

            // ReportDialog
            ["Report_Title"]             = "Формування звіту",
            ["Report_PeriodFrom"]        = "Період з:",
            ["Report_PeriodTo"]          = "по:",
            ["Report_ResourceType"]      = "Тип ресурсу:",
            ["Report_ColName"]           = "Назва",
            ["Report_ColStatus"]         = "Статус",
            ["Report_ColStart"]          = "Початок",
            ["Report_ColEnd"]            = "Кінець",
            ["Report_ColDuration"]       = "Тривалість (год)",
            ["Report_ColIncome"]         = "Дохід",
            ["Btn_Generate"]             = "Сформувати",
            ["Btn_ExportPdf"]            = "Експорт у PDF",

            // DashboardWindow title
            ["Window_Dashboard"]         = "Головна панель",
            ["Window_Login"]             = "HotelServices - Авторизація",

            ["Pdf_Success"] = "Звіт успішно експортовано до PDF",
            ["Success_Title"] = "Успіх",

            ["AdditionalInfo_Header"] = "Додаткова інформація",
            ["Field_Rooms"] = "Кількість кімнат:",
            ["Field_IsLuxury"] = "Люкс:",
            ["Field_Capacity"] = "Місткість:",
            ["Field_ParkingNumber"] = "Номер паркомісця:",
            ["Field_TableNumber"] = "Номер столу:",
            ["Field_Guests"] = "Кількість гостей:",
            ["Field_ServiceType"] = "Тип послуги:",

            // Statistics
            ["Stats_Title"] = "Статистика",
            ["Stats_From"] = "Період з:",
            ["Stats_To"] = "по:",
            ["Stats_Refresh"] = "Оновити",
            ["Stats_TopTitle"] = "Топ-5 найприбутковіших ресурсів",
            ["Stats_Type"] = "Тип",
            ["Dashboard_Statistics"] = "Статистика",
            ["Dashboard_Statistics_Sub"] = "Аналітика та звіти",
            ["Tooltip_Statistics"] = "Переглянути статистику",
        };

        private static readonly Dictionary<string, string> EN = new Dictionary<string, string>
        {

            // Statistics
            ["Stats_Title"] = "Statistics",
            ["Stats_From"] = "From:",
            ["Stats_To"] = "To:",
            ["Stats_Refresh"] = "Refresh",
            ["Stats_TopTitle"] = "Top 5 Most Profitable Resources",
            ["Stats_Type"] = "Type",
            ["Dashboard_Statistics"] = "Statistics",
            ["Dashboard_Statistics_Sub"] = "Analytics and reports",
            ["Tooltip_Statistics"] = "View statistics",

            // MainWindow (Login)
            ["Login_Title"]              = "Authorization",
            ["Login_Username"]           = "Login:",
            ["Login_Password"]           = "Password:",
            ["Login_Button"]             = "Sign In",
            ["Login_Error_Empty"]        = "Please enter login and password",
            ["Login_Error_Invalid"]      = "Invalid login or password",

            // DashboardPage
            ["Dashboard_Title"]          = "Main Control Panel",
            ["Dashboard_Subtitle"]       = "Hotel Service Management System",
            ["Dashboard_Apartments"]     = "Apartment Management",
            ["Dashboard_Apartments_Sub"] = "View and edit rooms",
            ["Dashboard_Conference"]     = "Conference Room Booking",
            ["Dashboard_Conference_Sub"] = "Schedule and room capacity",
            ["Dashboard_Parking"]        = "Parking Management",
            ["Dashboard_Parking_Sub"]    = "Status and reservations",
            ["Dashboard_Restaurant"]     = "Restaurant Booking",
            ["Dashboard_Restaurant_Sub"] = "Tables, dates and guests",
            ["Dashboard_Services"]       = "Additional Services",
            ["Dashboard_Services_Sub"]   = "Gym, SPA and more",
            ["Dashboard_Users"]          = "User Management",
            ["Dashboard_Users_Sub"]      = "Add and edit users",

            // Tooltips DashboardPage
            ["Tooltip_Apartments"]       = "Go to apartment management",
            ["Tooltip_Conference"]       = "Go to conference room booking",
            ["Tooltip_Parking"]          = "Go to parking management",
            ["Tooltip_Restaurant"]       = "Go to restaurant booking",
            ["Tooltip_Services"]         = "Go to additional services",
            ["Tooltip_Users"]            = "Go to user management",

            // ResourcePage titles
            ["Resource_Apartments"]      = "Apartments",
            ["Resource_Conference"]      = "Conference Rooms",
            ["Resource_Parking"]         = "Parking Spaces",
            ["Resource_Restaurant"]      = "Restaurant",
            ["Resource_Services"]        = "Additional Services",

            // Shared buttons
            ["Btn_Add"]                  = "Add",
            ["Btn_Edit"]                 = "Edit",
            ["Btn_Delete"]               = "Delete",
            ["Btn_Back"]                 = "Home",
            ["Btn_Report"]               = "Report",
            ["Btn_ResetFilters"]         = "Reset Filters",
            ["Btn_AddUser"]              = "Add User",
            ["Placeholder_Search"]       = "Search...",

            // ResourcePage filter combo items
            ["Filter_AllStatuses"]       = "All statuses",
            ["Filter_Available"]         = "Available",
            ["Filter_Reserved"]          = "Reserved",
            ["Filter_Occupied"]          = "Occupied",
            ["Filter_Maintenance"]       = "Under maintenance",

            // UserManagementPage filter combo items
            ["Filter_AllRoles"]          = "All roles",
            ["Filter_Admin"]             = "Administrator",
            ["Filter_Manager"]           = "Manager",

            // UserManagementPage table headers
            ["Col_Id"]                   = "ID",
            ["Col_Username"]             = "Username",
            ["Col_FullName"]             = "Full Name",
            ["Col_Role"]                 = "Role",

            // ResourcePage table headers
            ["Col_Name"]                 = "Name",
            ["Col_Price"]                = "Price",
            ["Col_Status"]               = "Status",
            ["Col_Start"]                = "Start",
            ["Col_End"]                  = "End",

            // UserEditDialog
            ["UserEdit_TitleAdd"]        = "Add User",
            ["UserEdit_TitleEdit"]       = "Edit User",
            ["UserEdit_FullName"]        = "Full Name:",
            ["UserEdit_Username"]        = "Login:",
            ["UserEdit_Password"]        = "Password:",
            ["UserEdit_Role"]            = "Role:",
            ["Btn_Save"]                 = "Save",
            ["Btn_Cancel"]               = "Cancel",

            // ResourceEditDialog
            ["ResEdit_TitleAdd"]         = "Add Resource",
            ["ResEdit_TitleEdit"]        = "Edit Resource",
            ["ResEdit_Name"]             = "Name:",
            ["ResEdit_Description"]      = "Description:",
            ["ResEdit_Price"]            = "Price:",
            ["ResEdit_Status"]           = "Status:",
            ["ResEdit_StartDate"]        = "Start date and time:",
            ["ResEdit_EndDate"]          = "End date and time:",
            ["ResEdit_Time"]             = "Time:",

            // ReportDialog
            ["Report_Title"]             = "Generate Report",
            ["Report_PeriodFrom"]        = "Period from:",
            ["Report_PeriodTo"]          = "to:",
            ["Report_ResourceType"]      = "Resource type:",
            ["Report_ColName"]           = "Name",
            ["Report_ColStatus"]         = "Status",
            ["Report_ColStart"]          = "Start",
            ["Report_ColEnd"]            = "End",
            ["Report_ColDuration"]       = "Duration (hrs)",
            ["Report_ColIncome"]         = "Revenue",
            ["Btn_Generate"]             = "Generate",
            ["Btn_ExportPdf"]            = "Export to PDF",

            // DashboardWindow title
            ["Window_Dashboard"]         = "Main Panel",
            ["Window_Login"]             = "HotelServices - Login",


            ["Pdf_Success"] = "Report successfully exported to PDF",
            ["Success_Title"] = "Success",

            ["AdditionalInfo_Header"] = "Additional Information",
            ["Field_Rooms"] = "Number of rooms:",
            ["Field_IsLuxury"] = "Luxury:",
            ["Field_Capacity"] = "Capacity:",
            ["Field_ParkingNumber"] = "Parking space number:",
            ["Field_TableNumber"] = "Table number:",
            ["Field_Guests"] = "Number of guests:",
            ["Field_ServiceType"] = "Service type:",
        };

        public static string Get(string key, AppLanguage lang = AppLanguage.UA)
        {
            var dict = lang == AppLanguage.UA ? UA : EN;
            return dict.TryGetValue(key, out var val) ? val : key;
        }

        public static string Get(string key) => Get(key, LanguageService.Instance.CurrentLanguage);
    }
}
