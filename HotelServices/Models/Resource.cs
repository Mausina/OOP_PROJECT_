using System;

namespace HotelServices.Models
{
    public enum ResourceType
    {
        Apartment,
        ConferenceRoom,
        ParkingSpace,
        RestaurantTable,
        AdditionalService
    }

    public enum ReservationStatus
    {
        Available,
        Reserved,
        Occupied,
        Maintenance
    }

    public class Resource
    {
        public int Id { get; set; }
        public ResourceType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ReservationStatus Status { get; set; }

        // Для апартаментів
        public int? Rooms { get; set; }
        public bool? IsLuxury { get; set; }

        // Для конференц-залів
        public int? Capacity { get; set; }

        // Для паркомісць
        public string? ParkingNumber { get; set; }

        // Для ресторану
        public int? TableNumber { get; set; }
        public int? Guests { get; set; }

        // Для додаткових послуг
        public string? ServiceType { get; set; }

        // Бронювання
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ReservedByUserId { get; set; }

        public decimal TotalIncome { get; set; } // Загальний дохід
        public double OccupancyDuration { get; set; } // Тривалість у годинах
    }
}