namespace TournamentManager.Contexts
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        T GetById(int id);
        List<T?> GetAll(bool tracked = true);
        void Update(T entity);
        void DeleteById(int id);
        void Save();
    }
}
