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
    public class EpisodeBlackListService : IEpisodeBlackListService
    {
        public readonly IEpisodeBlackListRepository _episodeBlackListRepository;
        public EpisodeBlackListService(IEpisodeBlackListRepository episodeBlackListRepository)
        {
            _episodeBlackListRepository = episodeBlackListRepository;
        }

        public async Task<GenericBlackListDTO> GetObjectBlackList(GenericBlackListDTO objectGeneral)
        {
            {
                var find = await _episodeBlackListRepository.GetObjectBlackList(EpisodeBlacklist.GenericQueueDTOToEpisodeBlacklist(objectGeneral));

                return GenericBlackListDTO.EpisodeBlackListToGenericBlackListDTO(find);
            }
        }

        public async Task<List<GenericBlackListDTO>> GetObjectsBlackList()
        {
            var rs = await _episodeBlackListRepository.GetObjectsBlackList();

            List<GenericBlackListDTO> listGenericQueueDTO = new();

            foreach (var EpisodeQueue in rs.ToList())
            {
                listGenericQueueDTO.Add(GenericBlackListDTO.EpisodeBlackListToGenericBlackListDTO(EpisodeQueue));
            }

            return listGenericQueueDTO;
        }

        public async Task<GenericBlackListDTO> PutObjectBlackList(GenericBlackListDTO objectGeneral)
        {
            var objectGeneralRepository = EpisodeBlacklist.GenericQueueDTOToEpisodeBlacklist(objectGeneral);

            try
            {
                await _episodeBlackListRepository.GetObjectBlackList(objectGeneralRepository);
                throw new ApiConflictException("Conflict Chapter BlackList");
            }
            catch (ApiNotFoundException)
            {
                var rs = await _episodeBlackListRepository.PutObjectBlackList(objectGeneralRepository);
                return GenericBlackListDTO.EpisodeBlackListToGenericBlackListDTO(rs);
            }
        }
    }
}
