﻿namespace TournamentManager.Contexts
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
}
