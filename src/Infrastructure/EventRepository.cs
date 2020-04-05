using System.Threading.Tasks;
using Core;

namespace Infrastructure
{
    public sealed class EventRepository : BaseRepository, IRepository<object>
    {
        public async Task Save(string id, object data)
        {
            await base.Set<object>(id, data);
        }

        public async Task<object> Retrieve(string id)
        {
            return await base.Get<object>(id);
        }

        public async Task Delete(string id)
        {
            await base.Remove(id);
        }
    }
}
