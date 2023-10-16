using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Interfaces.Services
{
    public interface IGeneralObjectBlackListService<TObjectDTO>
    {
        //get
        Task<TObjectDTO> GetObjectBlackList(TObjectDTO objectGeneral);
        Task<List<TObjectDTO>> GetObjectsBlackList();

        //put
        Task<TObjectDTO> PutObjectBlackList(TObjectDTO objectGeneral);
    }
}
