using Microsoft.EntityFrameworkCore;

namespace TournamentManager.Contexts
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TournamentDbContext _databaseContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(TournamentDbContext databaseContext)
        {
            _databaseContext = databaseContext;
            _dbSet = databaseContext.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            Save();
        }

        public void AddRange(ICollection<T> entities)
        {
            _dbSet.AddRange(entities);
            Save();
        }

        public void DeleteById(int id)
        {
            var entityToDelete = _dbSet.Find(id);

            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                Save();
            }
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public List<T> GetAll(bool tracked = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            Save();
        }

        public void Save()
        {
            _databaseContext.SaveChanges();
        }
    }
}
