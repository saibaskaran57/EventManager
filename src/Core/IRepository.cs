using System.Threading.Tasks;

namespace Core
{
    public interface IRepository<TData>
    {
        Task Save(string id, TData data);

        Task<TData> Retrieve(string id);

        Task Delete(string id);
    }
}
