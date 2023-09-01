using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;

namespace SampleWebApiAspNetCore.Repositories
{
    public interface IPlayerRepository
    {
        PlayerEntity GetSingle(int id);
        void Add(PlayerEntity entity);
        void Delete(int id);
        PlayerEntity Update(int id, PlayerEntity entity);
        IQueryable<PlayerEntity> GetAll(QueryParameters queryParameters);
        ICollection<PlayerEntity> GetRandomPlayer();
        int Count();
        bool Save();
    }
}
