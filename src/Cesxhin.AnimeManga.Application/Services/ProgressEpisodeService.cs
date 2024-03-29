﻿using Cesxhin.AnimeManga.Modules.Exceptions;
using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Domain.DTO;
using Cesxhin.AnimeManga.Domain.Models;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Services
{
    public class ProgressEpisodeService : IProgressEpisodeService
    {
        public readonly IProgressEpisodeRepository _progressEpisodeRepository;
        public ProgressEpisodeService(IProgressEpisodeRepository progressEpisodeRepository)
        {
            _progressEpisodeRepository = progressEpisodeRepository;
        }

        public async Task<ProgressEpisodeDTO> GetProgressByName(string name, string username, string nameCfg)
        {
            var result = await _progressEpisodeRepository.CheckProgress(name, username, nameCfg);
            return ProgressEpisodeDTO.ProgressEpisodeToProgressEpisodeDTO(result);
        }

        public async Task<ProgressEpisodeDTO> UpdateProgress(ProgressEpisodeDTO progress)
        {
            ProgressEpisode result;

            try
            {
                await _progressEpisodeRepository.CheckProgress(progress.Name, progress.Username, progress.NameCfg);
                result = await _progressEpisodeRepository.UpdateProgress(ProgressEpisode.ProgressEpisodeDTOToProgressEpisode(progress));
            }
            catch (ApiNotFoundException)
            {
                result = await _progressEpisodeRepository.CreateProgress(ProgressEpisode.ProgressEpisodeDTOToProgressEpisode(progress));
            }

            return ProgressEpisodeDTO.ProgressEpisodeToProgressEpisodeDTO(result);

        }
    }
}
