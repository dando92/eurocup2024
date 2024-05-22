namespace TournamentManager
{
    public class LocalCacheMemory : ICache
    {
        public Dictionary<string, object> _cachedObjects;

        public LocalCacheMemory()
        {
            _cachedObjects = new Dictionary<string, object>();
        }

        public void Add<T>(string cachedPropertyName, T obj)
        {
            _cachedObjects.Add(cachedPropertyName, obj);
        }

        public T Get<T>(string cachedPropertyName)
        {
            if (_cachedObjects.ContainsKey(cachedPropertyName))
                return (T)_cachedObjects[cachedPropertyName];
            else
                return default(T);
        }

        public void Remove(string cachedPropertyName)
        {
            _cachedObjects.Remove(cachedPropertyName);
        }
    }
}
