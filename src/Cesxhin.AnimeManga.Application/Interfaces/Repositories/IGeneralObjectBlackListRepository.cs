using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Interfaces.Repositories
{
    public interface IGeneralObjectBlackListRepository<T>
    {
        //get
        Task<T> GetObjectBlackList(T objectGeneral);
        Task<IEnumerable<T>> GetObjectsBlackList();

        //put
        Task<T> PutObjectBlackList(T objectGeneral);
    }
}
