using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using System.Linq.Dynamic.Core;

namespace SampleWebApiAspNetCore.Repositories
{
    public class PlayerSqlRepository : IPlayerRepository
    {
        private readonly PlayerDbContext _playerDbContext;

        public PlayerSqlRepository(PlayerDbContext playerDbContext)
        {
            _playerDbContext = playerDbContext;
        }

        public PlayerEntity GetSingle(int id)
        {
            return _playerDbContext.PlayerEntity.FirstOrDefault(x => x.Id == id);
        }

        public void Add(PlayerEntity item)
        {
            _playerDbContext.PlayerEntity.Add(item);
        }

        public void Delete(int id)
        {
            PlayerEntity playerEntity = GetSingle(id);
            _playerDbContext.PlayerEntity.Remove(playerEntity);
        }

        public PlayerEntity Update(int id, PlayerEntity entity)
        {
            _playerDbContext.PlayerEntity.Update(entity);
            return entity;
        }

        public IQueryable<PlayerEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<PlayerEntity> _allItems = _playerDbContext.PlayerEntity.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Level.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _playerDbContext.PlayerEntity.Count();
        }

        public bool Save()
        {
            return (_playerDbContext.SaveChanges() >= 0);
        }

        public ICollection<PlayerEntity> GetRandomPlayer()
        {
            List<PlayerEntity> toReturn = new List<PlayerEntity>();

            toReturn.Add(GetRandomItem("Warrior"));
            toReturn.Add(GetRandomItem("Archer"));
            toReturn.Add(GetRandomItem("Mage"));

            return toReturn;
        }

        private PlayerEntity GetRandomItem(string job)
        {
            return _playerDbContext.PlayerEntity
                .Where(x => x.Job == job)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
