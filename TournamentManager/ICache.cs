namespace TournamentManager
{
    public interface ICache
    {
        T Get<T>(string cachedPropertyName);
        void Add<T>(string cachedPropertyName, T obj);
        void Remove(string cachedPropertyName);
    }
}
