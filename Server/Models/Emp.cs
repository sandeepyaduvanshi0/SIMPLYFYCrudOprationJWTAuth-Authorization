using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Server.Models
{
    public class Emp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string? Img { get; set; }       

    }
}
