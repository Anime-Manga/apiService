using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Interfaces.Repositories
{
    public interface IGeneralObjectQueueRepository<T>
    {
        //get
        Task<IEnumerable<T>> GetObjectsQueue();
        Task<T> GetObjectQueue(T objectGeneral);

        //put
        Task<T> PutObjectQueue(T objectGeneral);

        //delete
        Task<int> DeleteObjectQueue(T objectGeneral);
    }
}
