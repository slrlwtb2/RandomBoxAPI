namespace RandomBoxAPI.Repository.Interfaces
{
    public interface IRepository<T1, T2> where T1 : class
    {
        Task<List<T1>> GetAll();
        Task<T1> GetById(T2 id);
        Task Add(T1 entity);
        Task Delete(T2 id);
        Task Save();
        void Update(T1 entity);
        Task<bool> Exist(T2 id);

    }
}
