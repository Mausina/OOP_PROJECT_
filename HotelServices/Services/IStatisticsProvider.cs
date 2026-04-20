using HotelServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelServices.Services
{
    // ========== REVENUE CALCULATORS ==========

    public interface IRevenueCalculator
    {
        decimal Calculate(Resource resource);
        string PricingUnit { get; }
    }

    public abstract class RevenueCalculatorBase : IRevenueCalculator
    {
        public abstract decimal Calculate(Resource resource);
        public abstract string PricingUnit { get; }

        protected double GetDurationHours(Resource resource)
        {
            if (!resource.StartDate.HasValue || !resource.EndDate.HasValue)
                return 0;
            return (resource.EndDate.Value - resource.StartDate.Value).TotalHours;
        }
    }

    public class PerNightCalculator : RevenueCalculatorBase
    {
        public override string PricingUnit => "per night";
        public override decimal Calculate(Resource resource)
        {
            var hours = GetDurationHours(resource);
            if (hours <= 0) return resource.Price;
            return resource.Price * (decimal)(hours / 24);
        }
    }

    public class PerHourCalculator : RevenueCalculatorBase
    {
        public override string PricingUnit => "per hour";
        public override decimal Calculate(Resource resource)
        {
            var hours = GetDurationHours(resource);
            if (hours <= 0) return resource.Price;
            return resource.Price * (decimal)hours;
        }
    }

    public class PerGuestCalculator : RevenueCalculatorBase
    {
        public override string PricingUnit => "per guest";
        public override decimal Calculate(Resource resource)
        {
            var guests = resource.Guests ?? 1;
            return resource.Price * guests;
        }
    }

    public static class RevenueCalculatorFactory
    {
        public static IRevenueCalculator GetCalculator(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Apartment:
                case ResourceType.ParkingSpace:
                    return new PerNightCalculator();

                case ResourceType.ConferenceRoom:
                case ResourceType.AdditionalService:
                    return new PerHourCalculator();

                case ResourceType.RestaurantTable:
                    return new PerGuestCalculator();

                default:
                    return new PerNightCalculator();
            }
        }
    }

    // ========== STATISTICS PROVIDERS ==========

    public interface IStatisticsProvider
    {
        string Title { get; }
        string Compute(List<Resource> resources, DateTime from, DateTime to);
        string AccentColor { get; }
    }

    public abstract class BaseStatisticsProvider : IStatisticsProvider
    {
        public abstract string Title { get; }
        public abstract string Compute(List<Resource> resources, DateTime from, DateTime to);
        public virtual string AccentColor => "#2D5F8B";

        protected IEnumerable<Resource> GetBookedInPeriod(
            List<Resource> resources, DateTime from, DateTime to)
        {
            return resources.Where(r =>
                r.StartDate.HasValue &&
                r.EndDate.HasValue &&
                r.StartDate.Value <= to &&
                r.EndDate.Value >= from);
        }
    }

    public class TotalRevenueProvider : BaseStatisticsProvider
    {
        public override string Title => "Total Revenue";
        public override string AccentColor => "#2D5F8B";

        public override string Compute(List<Resource> resources, DateTime from, DateTime to)
        {
            var booked = GetBookedInPeriod(resources, from, to);
            decimal total = booked.Sum(r =>
                RevenueCalculatorFactory.GetCalculator(r.Type).Calculate(r));
            return $"₴ {total:N0}";
        }
    }

    public class BookingCountProvider : BaseStatisticsProvider
    {
        public override string Title => "Total Bookings";

        public override string Compute(List<Resource> resources, DateTime from, DateTime to)
        {
            var count = GetBookedInPeriod(resources, from, to).Count();
            return count.ToString();
        }
    }

    public class OccupancyRateProvider : BaseStatisticsProvider
    {
        public override string Title => "Occupancy Rate";
        public override string AccentColor => "#0F6E56";

        public override string Compute(List<Resource> resources, DateTime from, DateTime to)
        {
            if (resources == null || resources.Count == 0)
                return "0%";

            var bookedCount = GetBookedInPeriod(resources, from, to).Count();
            var rate = (double)bookedCount / resources.Count * 100;
            return $"{rate:F0}%";
        }
    }

    public class ActiveResourcesProvider : BaseStatisticsProvider
    {
        public override string Title => "Active Resources";

        public override string Compute(List<Resource> resources, DateTime from, DateTime to)
        {
            var active = resources.Count(r =>
                r.Status == ReservationStatus.Occupied ||
                r.Status == ReservationStatus.Reserved);
            return active.ToString();
        }
    }

    // ========== TOP RESOURCES ==========

    public class TopResourcesRow
    {
        public int Rank { get; set; }
        public string Name { get; set; } = "";
        public string TypeName { get; set; } = "";
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public static class TopResourcesCalculator
    {
        public static List<TopResourcesRow> GetTop(
            List<Resource> resources, DateTime from, DateTime to, int topN = 5)
        {
            var ranked = resources
                .Where(r => r.StartDate.HasValue && r.EndDate.HasValue)
                .Where(r => r.StartDate!.Value <= to && r.EndDate!.Value >= from)
                .Select(r => new
                {
                    Resource = r,
                    Revenue = RevenueCalculatorFactory.GetCalculator(r.Type).Calculate(r)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(topN)
                .ToList();

            var result = new List<TopResourcesRow>();
            int rank = 1;
            foreach (var item in ranked)
            {
                result.Add(new TopResourcesRow
                {
                    Rank = rank++,
                    Name = item.Resource.Name ?? "",
                    TypeName = item.Resource.Type.ToString(),
                    BookingCount = 1,
                    Revenue = item.Revenue
                });
            }
            return result;
        }
    }
}