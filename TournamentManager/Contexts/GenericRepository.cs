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

        public IQueryable<T> GetAll(bool tracked = true)
        {
            if (!tracked)
                return _dbSet.AsNoTracking();
            else
                return _dbSet;
        }

        public void Update(T entity)
        {
            bool tracked = _databaseContext.Entry(entity).State != EntityState.Detached;

            if (tracked)
                return;

            _dbSet.Update(entity);
            Save();
        }

        public void Save()
        {
            _databaseContext.SaveChanges();
        }

        private static object[] GetPrimaryKeys<T>(DbContext context, T value)
        {
            var keyNames = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                      .Select(x => x.Name).ToArray();
            var result = new object[keyNames.Length];
            for (int i = 0; i < keyNames.Length; i++)
            {
                result[i] = typeof(T).GetProperty(keyNames[i])?.GetValue(value);
            }
            return result;
        }
    }
}
