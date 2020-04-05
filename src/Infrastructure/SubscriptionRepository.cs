using System.Threading.Tasks;
using Core;
using Infrastructure.Models;

namespace Infrastructure
{
    public sealed class SubscriptionRepository : BaseRepository, IRepository<Subscription>
    {
        public async Task Save(string id, Subscription data)
        {
            await base.Set<Subscription>(id, data);
        }

        public async Task<Subscription> Retrieve(string id)
        {
            return await base.Get<Subscription>(id);
        }

        public async Task Delete(string id)
        {
            await base.Remove(id);
        }
    }
}
