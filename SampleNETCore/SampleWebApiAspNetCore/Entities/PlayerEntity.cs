namespace SampleWebApiAspNetCore.Entities
{
    public class PlayerEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Job { get; set; }
        public int Level { get; set; }
        public DateTime Created { get; set; }
    }
}
