using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Domain.DTO;
using Cesxhin.AnimeManga.Domain.Models;
using Cesxhin.AnimeManga.Modules.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Services
{
    public class ChapterQueueService : IChapterQueueService
    {
        public readonly IChapterQueueRepository _chapterQueueRepository;
        public ChapterQueueService(IChapterQueueRepository chapterQueueRepository)
        {
            _chapterQueueRepository = chapterQueueRepository;
        }

        public async Task<GenericQueueDTO> DeleteObjectQueue(GenericQueueDTO objectGeneral)
        {
            var find = await _chapterQueueRepository.GetObjectQueue(ChapterQueue.GenericQueueDTOToChapterQueue(objectGeneral));
            await _chapterQueueRepository.DeleteObjectQueue(find);

            return objectGeneral;
        }

        public async Task<List<GenericQueueDTO>> GetObjectsQueue()
        {
            var rs = await _chapterQueueRepository.GetObjectsQueue();

            List<GenericQueueDTO> listGenericQueueDTO = new();

            foreach (var ChapterQueue in rs.ToList())
            {
                listGenericQueueDTO.Add(GenericQueueDTO.ChapterQueueToGenericQueueDTO(ChapterQueue));
            }

            return listGenericQueueDTO;
        }

        public async Task<GenericQueueDTO> PutObjectQueue(GenericQueueDTO objectGeneral)
        {
            var objectGeneralRepository = ChapterQueue.GenericQueueDTOToChapterQueue(objectGeneral);

            try
            {
                await _chapterQueueRepository.GetObjectQueue(objectGeneralRepository);
                throw new ApiConflictException("Conflict Chapter queue");
            }
            catch (ApiNotFoundException)
            {
                var rs = await _chapterQueueRepository.PutObjectQueue(objectGeneralRepository);
                return GenericQueueDTO.ChapterQueueToGenericQueueDTO(rs);
            }
        }
    }
}
