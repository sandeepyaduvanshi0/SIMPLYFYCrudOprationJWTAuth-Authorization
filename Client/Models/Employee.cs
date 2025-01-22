namespace Client.Models
{
    public class Employee
    {
        public int id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public string? img { get; set; }
        public IFormFile? imageFile { get; set; }
    }
}
