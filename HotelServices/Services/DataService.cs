using HotelServices.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace HotelServices.Services
{
    public class DataService
    {
        private const string DatabaseName = "HotelDB.sqlite";
        private readonly string _connectionString;

        public DataService()
        {
            _connectionString = $"Data Source={DatabaseName};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    // Створення таблиць
                    string createUsersTable = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        FullName TEXT NOT NULL,
                        Role INTEGER NOT NULL
                    )";

                    string createResourcesTable = @"
                    CREATE TABLE Resources (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Type INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Price REAL NOT NULL,
                        Status INTEGER NOT NULL,
                        Rooms INTEGER,
                        IsLuxury INTEGER,
                        Capacity INTEGER,
                        ParkingNumber TEXT,
                        TableNumber INTEGER,
                        Guests INTEGER,
                        ServiceType TEXT,
                        StartDate TEXT,
                        EndDate TEXT,
                        ReservedByUserId INTEGER,
                        FOREIGN KEY (ReservedByUserId) REFERENCES Users(Id)
                    )";

                    new SQLiteCommand(createUsersTable, connection).ExecuteNonQuery();
                    new SQLiteCommand(createResourcesTable, connection).ExecuteNonQuery();

                    // Додати адміна за замовчуванням
                    string addAdmin = @"
                    INSERT INTO Users (Username, Password, FullName, Role)
                    VALUES ('admin', 'admin', 'Директор', 1)";

                    string addManager = @"
                    INSERT INTO Users (Username, Password, FullName, Role)
                    VALUES ('manager', 'manager', 'Менеджер', 0)";

                    new SQLiteCommand(addAdmin, connection).ExecuteNonQuery();
                    new SQLiteCommand(addManager, connection).ExecuteNonQuery();

                    // Додати приклади ресурсів
                    string addResources = @"
                    INSERT INTO Resources (Type, Name, Description, Price, Status, Rooms, IsLuxury, Capacity, ParkingNumber, TableNumber, Guests, ServiceType)
                    VALUES 
                        (0, 'Люкс', 'Двокімнатний люкс', 2500, 0, 2, 1, NULL, NULL, NULL, NULL, NULL),
                        (1, 'Зал А', 'Конференц-зал', 5000, 0, NULL, NULL, 50, NULL, NULL, NULL, NULL),
                        (2, 'Паркомісце 1', 'Парковка A1', 200, 0, NULL, NULL, NULL, 'A1', NULL, NULL, NULL),
                        (3, 'Стіл 5', 'Стіл у ресторані', 0, 0, NULL, NULL, NULL, NULL, 5, 4, NULL),
                        (4, 'Спортзал', 'Доступ до спортзалу', 300, 0, NULL, NULL, NULL, NULL, NULL, NULL, 'Спорт')";

                    new SQLiteCommand(addResources, connection).ExecuteNonQuery();
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            Role = (UserRole)Convert.ToInt32(reader["Role"])
                        });
                    }
                }
            }

            return users;
        }

        public User GetUserById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Role = (UserRole)Convert.ToInt32(reader["Role"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void AddUser(User user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                INSERT INTO Users (Username, Password, FullName, Role)
                VALUES (@Username, @Password, @FullName, @Role)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Role", (int)user.Role);

                    command.ExecuteNonQuery();
                }

                // Отримати ID останнього доданого користувача
                query = "SELECT last_insert_rowid()";
                using (var command = new SQLiteCommand(query, connection))
                {
                    user.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                UPDATE Users 
                SET Username = @Username, 
                    Password = @Password, 
                    FullName = @FullName, 
                    Role = @Role
                WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Role", (int)user.Role);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Users WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Resource> GetResourcesByType(ResourceType type)
        {
            var resources = new List<Resource>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Resources WHERE Type = @Type";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", (int)type);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resources.Add(ReadResourceFromReader(reader));
                        }
                    }
                }
            }

            return resources;
        }

        public Resource GetResourceById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Resources WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadResourceFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void AddResource(Resource resource)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO Resources (
                    Type, Name, Description, Price, Status, 
                    Rooms, IsLuxury, Capacity, ParkingNumber, 
                    TableNumber, Guests, ServiceType, 
                    StartDate, EndDate, ReservedByUserId
                )
                VALUES (
                    @Type, @Name, @Description, @Price, @Status,
                    @Rooms, @IsLuxury, @Capacity, @ParkingNumber,
                    @TableNumber, @Guests, @ServiceType,
                    @StartDate, @EndDate, @ReservedByUserId
                )";

                using (var command = new SQLiteCommand(query, connection))
                {
                    AddResourceParameters(command, resource);
                    command.ExecuteNonQuery();
                }

                // Отримати ID останнього доданого ресурсу
                query = "SELECT last_insert_rowid()";
                using (var command = new SQLiteCommand(query, connection))
                {
                    resource.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateResource(Resource resource)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                UPDATE Resources 
                SET 
                    Type = @Type,
                    Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    Status = @Status,
                    Rooms = @Rooms,
                    IsLuxury = @IsLuxury,
                    Capacity = @Capacity,
                    ParkingNumber = @ParkingNumber,
                    TableNumber = @TableNumber,
                    Guests = @Guests,
                    ServiceType = @ServiceType,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    ReservedByUserId = @ReservedByUserId
                WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", resource.Id);
                    AddResourceParameters(command, resource);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteResource(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Resources WHERE Id = @Id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private Resource ReadResourceFromReader(SQLiteDataReader reader)
        {
            var resource = new Resource
            {
                Id = Convert.ToInt32(reader["Id"]),
                Type = (ResourceType)Convert.ToInt32(reader["Type"]),
                Name = reader["Name"].ToString(),
                Description = reader["Description"] is DBNull ? null : reader["Description"].ToString(),
                Price = Convert.ToDecimal(reader["Price"]),
                Status = (ReservationStatus)Convert.ToInt32(reader["Status"]),
                StartDate = reader["StartDate"] is DBNull ? null : DateTime.Parse(reader["StartDate"].ToString()),
                EndDate = reader["EndDate"] is DBNull ? null : ParseDate(reader["EndDate"].ToString()),
                ReservedByUserId = reader["ReservedByUserId"] is DBNull ? null : (int?)Convert.ToInt32(reader["ReservedByUserId"])
            };

            // Заповнення специфічних полів
            if (!(reader["Rooms"] is DBNull)) resource.Rooms = Convert.ToInt32(reader["Rooms"]);
            if (!(reader["IsLuxury"] is DBNull)) resource.IsLuxury = Convert.ToBoolean(reader["IsLuxury"]);
            if (!(reader["Capacity"] is DBNull)) resource.Capacity = Convert.ToInt32(reader["Capacity"]);
            if (!(reader["ParkingNumber"] is DBNull)) resource.ParkingNumber = reader["ParkingNumber"].ToString();
            if (!(reader["TableNumber"] is DBNull)) resource.TableNumber = Convert.ToInt32(reader["TableNumber"]);
            if (!(reader["Guests"] is DBNull)) resource.Guests = Convert.ToInt32(reader["Guests"]);
            if (!(reader["ServiceType"] is DBNull)) resource.ServiceType = reader["ServiceType"].ToString();

            return resource;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime result))
                return result;
            return null;
        }

        private void AddResourceParameters(SQLiteCommand command, Resource resource)
        {
            command.Parameters.AddWithValue("@Type", (int)resource.Type);
            command.Parameters.AddWithValue("@Name", resource.Name);
            command.Parameters.AddWithValue("@Description", (object)resource.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", resource.Price);
            command.Parameters.AddWithValue("@Status", (int)resource.Status);

            // Специфічні параметри
            command.Parameters.AddWithValue("@Rooms", (object)resource.Rooms ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLuxury", (object)resource.IsLuxury ?? DBNull.Value);
            command.Parameters.AddWithValue("@Capacity", (object)resource.Capacity ?? DBNull.Value);
            command.Parameters.AddWithValue("@ParkingNumber", (object)resource.ParkingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@TableNumber", (object)resource.TableNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Guests", (object)resource.Guests ?? DBNull.Value);
            command.Parameters.AddWithValue("@ServiceType", (object)resource.ServiceType ?? DBNull.Value);

            // Дати бронювання
            command.Parameters.AddWithValue("@StartDate", (object)resource.StartDate?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object)resource.EndDate?.ToString("o") ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReservedByUserId", (object)resource.ReservedByUserId ?? DBNull.Value);
        }

        public List<Resource> GetReportData(ResourceType resourceType, DateTime startDate, DateTime endDate)
        {
            var resources = new List<Resource>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
        SELECT * FROM Resources 
        WHERE Type = @Type 
        AND (
            (StartDate IS NOT NULL AND EndDate IS NOT NULL AND 
             StartDate BETWEEN @StartDate AND @EndDate)
            OR 
            (Status = @ReservedStatus OR Status = @OccupiedStatus)
        )";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", (int)resourceType);
                    command.Parameters.AddWithValue("@StartDate", startDate.ToString("o"));
                    command.Parameters.AddWithValue("@EndDate", endDate.ToString("o"));
                    command.Parameters.AddWithValue("@ReservedStatus", (int)ReservationStatus.Reserved);
                    command.Parameters.AddWithValue("@OccupiedStatus", (int)ReservationStatus.Occupied);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var resource = ReadResourceFromReader(reader);

                            // Додаткові розрахунки для звіту
                            if (resource.StartDate.HasValue && resource.EndDate.HasValue)
                            {
                                resource.TotalIncome = CalculateIncome(resource);
                                resource.OccupancyDuration = (resource.EndDate.Value - resource.StartDate.Value).TotalHours;
                            }

                            resources.Add(resource);
                        }
                    }
                }
            }

            return resources;
        }

        private decimal CalculateIncome(Resource resource)
        {
            if (!resource.StartDate.HasValue || !resource.EndDate.HasValue)
                return 0;

            var duration = (resource.EndDate.Value - resource.StartDate.Value).TotalHours;

            // Різні формули розрахунку для різних типів ресурсів
            switch (resource.Type)
            {
                case ResourceType.Apartment:
                    return resource.Price * (decimal)(duration / 24); // Ціна за добу
                case ResourceType.ConferenceRoom:
                    return resource.Price * (decimal)(duration / 1); // Ціна за годину
                case ResourceType.ParkingSpace:
                    return resource.Price * (decimal)(duration / 24); // Ціна за добу
                case ResourceType.RestaurantTable:
                    // Для столів - фіксована ціна за бронювання
                    return resource.Price * resource.Guests ?? 1;
                case ResourceType.AdditionalService:
                    return resource.Price * (decimal)duration;
                default:
                    return resource.Price;
            }
        }

        public List<Resource> GetFilteredResources(ResourceType type, string searchText = null, ReservationStatus? status = null)
        {
            var resources = new List<Resource>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Resources WHERE Type = @Type";
                var parameters = new List<SQLiteParameter>
        {
            new SQLiteParameter("@Type", (int)type)
        };

                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " AND (Name LIKE @SearchText OR Description LIKE @SearchText)";
                    parameters.Add(new SQLiteParameter("@SearchText", $"%{searchText}%"));
                }

                if (status.HasValue)
                {
                    query += " AND Status = @Status";
                    parameters.Add(new SQLiteParameter("@Status", (int)status.Value));
                }

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resources.Add(ReadResourceFromReader(reader));
                        }
                    }
                }
            }

            return resources;
        }

        public List<User> GetFilteredUsers(string searchText = null, UserRole? role = null)
        {
            var users = new List<User>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Users WHERE 1=1";
                var parameters = new List<SQLiteParameter>();

                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " AND (Username LIKE @SearchText OR FullName LIKE @SearchText)";
                    parameters.Add(new SQLiteParameter("@SearchText", $"%{searchText}%"));
                }

                if (role.HasValue)
                {
                    query += " AND Role = @Role";
                    parameters.Add(new SQLiteParameter("@Role", (int)role.Value));
                }

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Role = (UserRole)Convert.ToInt32(reader["Role"])
                            });
                        }
                    }
                }
            }

            return users;
        }
    }
}