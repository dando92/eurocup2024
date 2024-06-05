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
        }

        public void AddRange(ICollection<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void DeleteById(int id)
        {
            var entityToDelete = _dbSet.Find(id);

            if (entityToDelete != null)
                _dbSet.Remove(entityToDelete);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll(bool tracked = true)
        {
            if (!tracked)
                return _dbSet.AsNoTracking();
            else
                return _dbSet;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Save(int maxRetries = 5, int delay = 100)
        {
            bool saveFailed;
            int retries = 0;

            do
            {
                saveFailed = false;

                try
                {
                    _databaseContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Retry");

                    saveFailed = true;
                    retries++;
                    if (retries > maxRetries)
                        throw;
                    System.Threading.Thread.Sleep(delay);
                }
            } while (saveFailed);
        }
    }
}
