namespace HotelServices.Models
{
    public enum UserRole
    {
        Manager,
        Director
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
    }
}