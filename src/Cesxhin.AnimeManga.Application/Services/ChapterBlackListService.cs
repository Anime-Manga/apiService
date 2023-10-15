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
    public class ChapterBlackListService : IChapterBlackListService
    {
        public readonly IChapterBlackListRepository _chapterBlackListRepository;
        public ChapterBlackListService(IChapterBlackListRepository chapterBlackListRepository)
        {
            _chapterBlackListRepository = chapterBlackListRepository;
        }

        public async Task<GenericBlackListDTO> GetObjectBlackList(GenericBlackListDTO objectGeneral)
        {
            {
                var find = await _chapterBlackListRepository.GetObjectBlackList(ChapterBlacklist.GenericQueueDTOToChapterBlacklist(objectGeneral));

                return GenericBlackListDTO.ChapterBlackListToGenericBlackListDTO(find);
            }
        }

        public async Task<List<GenericBlackListDTO>> GetObjectsBlackList()
        {
            var rs = await _chapterBlackListRepository.GetObjectsBlackList();

            List<GenericBlackListDTO> listGenericQueueDTO = new();

            foreach (var EpisodeQueue in rs.ToList())
            {
                listGenericQueueDTO.Add(GenericBlackListDTO.ChapterBlackListToGenericBlackListDTO(EpisodeQueue));
            }

            return listGenericQueueDTO;
        }

        public async Task<GenericBlackListDTO> PutObjectBlackList(GenericBlackListDTO objectGeneral)
        {
            var objectGeneralRepository = ChapterBlacklist.GenericQueueDTOToChapterBlacklist(objectGeneral);

            try
            {
                await _chapterBlackListRepository.GetObjectBlackList(objectGeneralRepository);
                throw new ApiConflictException("Conflict Chapter queue");
            }
            catch (ApiNotFoundException)
            {
                var rs = await _chapterBlackListRepository.PutObjectBlackList(objectGeneralRepository);
                return GenericBlackListDTO.ChapterBlackListToGenericBlackListDTO(rs);
            }
        }
    }
}
