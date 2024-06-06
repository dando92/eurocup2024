using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace TournamentManager.Contexts
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(ICollection<T> entities);
        T GetById(int id);
        IQueryable<T> GetAll(bool tracked = true);
        void Update(T entity);
        void DeleteById(int id);
        void Save(int maxRetries = 3, int delay = 100);
    }

    public interface IGenericRepository
    {
        void Add<T>(T entity) where T : class;
        
        void AddRange<T>(ICollection<T> entities) where T : class;
        T GetById<T>(int id) where T : class;
        IQueryable<T> GetAll<T>(bool tracked = true) where T : class;
        void Update<T>(T entity) where T : class;
        void DeleteById<T>(int id) where T : class;
        void Save(int maxRetries = 3, int delay = 100);
    }
}
