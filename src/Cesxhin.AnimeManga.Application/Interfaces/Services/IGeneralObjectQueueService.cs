using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Interfaces.Services
{
    public interface IGeneralObjectQueueService<TObjectDTO>
    {
        //get
        Task<List<TObjectDTO>> GetObjectsQueue();
        Task<TObjectDTO> GetObjectQueue(TObjectDTO objectGeneral);

        //put
        Task<TObjectDTO> PutObjectQueue(TObjectDTO objectGeneral);

        //delete
        Task<TObjectDTO> DeleteObjectQueue(TObjectDTO objectGeneral);
    }
}
