using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Server.Models.DTOs
{
    public class EmpDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string? Img { get; set; }
        [ValidateNever]
        public IFormFile? imageFile { get; set; }
    }
}
