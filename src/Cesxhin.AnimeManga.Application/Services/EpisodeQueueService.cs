using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Domain.DTO;
using Cesxhin.AnimeManga.Domain.Models;
using Cesxhin.AnimeManga.Modules.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Services
{
    public class EpisodeQueueService : IEpisodeQueueService
    {
        public readonly IEpisodeQueueRepository _episodeQueueRepository;
        public EpisodeQueueService(IEpisodeQueueRepository episodeQueueRepository)
        {
            _episodeQueueRepository = episodeQueueRepository;
        }

        public async Task<GenericQueueDTO> DeleteObjectQueue(GenericQueueDTO objectGeneral)
        {
            var find = await _episodeQueueRepository.GetObjectQueue(EpisodeQueue.GenericQueueDTOToEpisodeQueue(objectGeneral));
            await _episodeQueueRepository.DeleteObjectQueue(find);

            return objectGeneral;
        }

        public async Task<List<GenericQueueDTO>> GetObjectsQueue()
        {
            var rs = await _episodeQueueRepository.GetObjectsQueue();

            List<GenericQueueDTO> listGenericQueueDTO = new();

            foreach (var EpisodeQueue in rs.ToList())
            {
                listGenericQueueDTO.Add(GenericQueueDTO.EpisodeQueueToGenericQueueDTO(EpisodeQueue));
            }

            return listGenericQueueDTO;
        }

        public async Task<GenericQueueDTO> PutObjectQueue(GenericQueueDTO objectGeneral)
        {
            var objectGeneralRepository = EpisodeQueue.GenericQueueDTOToEpisodeQueue(objectGeneral);

            try
            {
                await _episodeQueueRepository.GetObjectQueue(objectGeneralRepository);
                throw new ApiConflictException("Conflict Chapter queue");
            }
            catch (ApiNotFoundException)
            {
                var rs = await _episodeQueueRepository.PutObjectQueue(objectGeneralRepository);
                return GenericQueueDTO.EpisodeQueueToGenericQueueDTO(rs);
            }
        }
    }
}
