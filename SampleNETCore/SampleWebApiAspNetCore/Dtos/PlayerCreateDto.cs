using System.ComponentModel.DataAnnotations;

namespace SampleWebApiAspNetCore.Dtos
{
    public class PlayerCreateDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Job { get; set; }
        public int Level { get; set; }
        public DateTime Created { get; set; }
    }
}
