using HotelServices.Models;
using HotelServices.Services;

namespace HotelServices.Services
{
    public class AuthService
    {
        private readonly DataService _dataService;

        public AuthService()
        {
            _dataService = new DataService();
        }

        public User Authenticate(string username, string password)
        {
            var user = _dataService.GetAllUsers().FirstOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }
    }
}