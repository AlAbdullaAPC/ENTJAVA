using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(PlayerDbContext context)
        {
            context.PlayerEntity.Add(new PlayerEntity() { Level = 10, Job = "Warrior", Name = "Bilbo_B", Created = DateTime.Now });
            context.PlayerEntity.Add(new PlayerEntity() { Level = 30, Job = "Warrior", Name = "Leeroy", Created = DateTime.Now });
            context.PlayerEntity.Add(new PlayerEntity() { Level = 5, Job = "Archer", Name = "ElonTusk", Created = DateTime.Now });
            context.PlayerEntity.Add(new PlayerEntity() { Level = 2, Job = "Mage", Name = "Gravit", Created = DateTime.Now });

            context.SaveChanges();
        }
    }
}
