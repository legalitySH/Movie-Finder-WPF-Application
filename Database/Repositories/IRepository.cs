namespace MovieFinder.Database.Repositories
{
    interface IRepository<K, T> where T : class
    {
        abstract List<T> GetAll();
        abstract T? Get(K id);
        abstract void Add(T item);
        abstract void Update(T item);
        abstract void Delete(K id);
        abstract bool isExists(K id);
    }
}
