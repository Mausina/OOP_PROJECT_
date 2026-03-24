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

        // For apartments
        public int? Rooms { get; set; }
        public bool? IsLuxury { get; set; }

        //  For conference rooms
        public int? Capacity { get; set; }

        //  For parking spaces
        public string? ParkingNumber { get; set; }

        // For a restaurant
        public int? TableNumber { get; set; }
        public int? Guests { get; set; }

        // For additional services
        public string? ServiceType { get; set; }

        // Reservations
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ReservedByUserId { get; set; }

        public decimal TotalIncome { get; set; } //  Total revenue
        public double OccupancyDuration { get; set; } // Duration in hours
    }
}