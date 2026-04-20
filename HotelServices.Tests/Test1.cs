using HotelServices.Models;
using HotelServices.Services;
using ResourceType = HotelServices.Models.ResourceType;
using ReservationStatus = HotelServices.Models.ReservationStatus;
using Strings = HotelServices.Services.Strings;
namespace HotelServices.Tests
{
    [TestClass]
    public class RevenueCalculatorTests
    {
        [TestMethod]
        public void PerNight_OneNight_ReturnsPrice()
        {
            var calc = new PerNightCalculator();
            var r = new Resource
            {
                Price = 500,
                StartDate = new DateTime(2025, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2025, 1, 2, 12, 0, 0)
            };
            Assert.AreEqual(500m, calc.Calculate(r));
        }

        [TestMethod]
        public void PerNight_NoDates_ReturnsBasePrice()
        {
            var calc = new PerNightCalculator();
            var r = new Resource { Price = 500 };
            Assert.AreEqual(500m, calc.Calculate(r));
        }

        [TestMethod]
        public void PerHour_FiveHours_ReturnsPriceTimesFive()
        {
            var calc = new PerHourCalculator();
            var r = new Resource
            {
                Price = 100,
                StartDate = new DateTime(2025, 1, 1, 10, 0, 0),
                EndDate = new DateTime(2025, 1, 1, 15, 0, 0)
            };
            Assert.AreEqual(500m, calc.Calculate(r));
        }

        [TestMethod]
        public void PerGuest_FourGuests_MultipliesByFour()
        {
            var calc = new PerGuestCalculator();
            var r = new Resource { Price = 50, Guests = 4 };
            Assert.AreEqual(200m, calc.Calculate(r));
        }

        [TestMethod]
        public void Factory_Apartment_ReturnsPerNight()
        {
            var calc = RevenueCalculatorFactory.GetCalculator(ResourceType.Apartment);
            Assert.IsInstanceOfType(calc, typeof(PerNightCalculator));
        }

        [TestMethod]
        public void Factory_ConferenceRoom_ReturnsPerHour()
        {
            var calc = RevenueCalculatorFactory.GetCalculator(ResourceType.ConferenceRoom);
            Assert.IsInstanceOfType(calc, typeof(PerHourCalculator));
        }

        [TestMethod]
        public void Factory_RestaurantTable_ReturnsPerGuest()
        {
            var calc = RevenueCalculatorFactory.GetCalculator(ResourceType.RestaurantTable);
            Assert.IsInstanceOfType(calc, typeof(PerGuestCalculator));
        }
    }

    [TestClass]
    public class StatisticsTests
    {
        private List<Resource> Sample()
        {
            return new List<Resource>
            {
                new Resource
                {
                    Id = 1, Name = "Suite", Type = ResourceType.Apartment, Price = 500,
                    Status = ReservationStatus.Occupied,
                    StartDate = new DateTime(2025, 3, 1),
                    EndDate = new DateTime(2025, 3, 2)
                },
                new Resource
                {
                    Id = 2, Name = "Hall", Type = ResourceType.ConferenceRoom, Price = 100,
                    Status = ReservationStatus.Reserved,
                    StartDate = new DateTime(2025, 3, 5, 10, 0, 0),
                    EndDate = new DateTime(2025, 3, 5, 14, 0, 0)
                },
                new Resource
                {
                    Id = 3, Name = "Table", Type = ResourceType.RestaurantTable,
                    Price = 50, Guests = 4,
                    Status = ReservationStatus.Available
                }
            };
        }

        [TestMethod]
        public void BookingCount_TwoBookings_ReturnsTwo()
        {
            var provider = new BookingCountProvider();
            var result = provider.Compute(Sample(),
                new DateTime(2025, 3, 1),
                new DateTime(2025, 3, 31));
            Assert.AreEqual("2", result);
        }

        [TestMethod]
        public void OccupancyRate_TwoOfThree_Returns67Percent()
        {
            var provider = new OccupancyRateProvider();
            var result = provider.Compute(Sample(),
                new DateTime(2025, 3, 1),
                new DateTime(2025, 3, 31));
            Assert.IsTrue(result.Contains("67"));
        }

        [TestMethod]
        public void ActiveResources_TwoActive_ReturnsTwo()
        {
            var provider = new ActiveResourcesProvider();
            var result = provider.Compute(Sample(), DateTime.Today, DateTime.Today);
            Assert.AreEqual("2", result);
        }

        [TestMethod]
        public void TopResources_OrdersByRevenueDescending()
        {
            var top = TopResourcesCalculator.GetTop(Sample(),
                new DateTime(2025, 3, 1),
                new DateTime(2025, 3, 31),
                5);
            Assert.IsTrue(top.Count >= 2);
            Assert.IsTrue(top[0].Revenue >= top[1].Revenue);
        }

        [TestMethod]
        public void Polymorphism_AllProvidersShareInterface()
        {
            var providers = new List<IStatisticsProvider>
            {
                new TotalRevenueProvider(),
                new BookingCountProvider(),
                new OccupancyRateProvider(),
                new ActiveResourcesProvider()
            };
            foreach (var p in providers)
            {
                Assert.IsNotNull(p.Title);
                Assert.IsNotNull(p.Compute(Sample(), DateTime.Today.AddMonths(-1), DateTime.Today));
            }
        }
    }

    [TestClass]
    public class LanguageTests
    {
        [TestMethod]
        public void Strings_English_ReturnsEnglish()
        {
            Assert.AreEqual("Sign In", Strings.Get("Login_Button", AppLanguage.EN));
        }

        [TestMethod]
        public void Strings_Ukrainian_ReturnsUkrainian()
        {
            Assert.AreEqual("Увійти", Strings.Get("Login_Button", AppLanguage.UA));
        }

        [TestMethod]
        public void Strings_MissingKey_ReturnsKey()
        {
            Assert.AreEqual("NoKey_X1", Strings.Get("NoKey_X1"));
        }

        [TestMethod]
        public void LanguageService_Singleton()
        {
            Assert.AreSame(LanguageService.Instance, LanguageService.Instance);
        }
    }
}