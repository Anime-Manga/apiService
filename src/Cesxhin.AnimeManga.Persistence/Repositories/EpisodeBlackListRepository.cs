using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Domain.Models;
using Cesxhin.AnimeManga.Modules.Exceptions;
using Cesxhin.AnimeManga.Modules.NlogManager;
using NLog;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Persistence.Repositories
{
    public class EpisodeBlackListRepository : IEpisodeBlackListRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<EpisodeBlacklist> GetObjectBlackList(EpisodeBlacklist genericQueue)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<EpisodeBlacklist> rs;
                try
                {
                    rs = await connection.QueryAsync<EpisodeBlacklist>(e => e.Url == genericQueue.Url && e.NameCfg == genericQueue.NameCfg);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetObjectBlackList, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs.First();
                else
                    throw new ApiNotFoundException("Not found GetObjectBlackList");
            }
        }

        public async Task<IEnumerable<EpisodeBlacklist>> GetObjectsBlackList()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<EpisodeBlacklist> rs;

                try
                {
                    rs = await connection.QueryAllAsync<EpisodeBlacklist>();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetObjectsBlackList, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs;
                else
                    throw new ApiNotFoundException("Not found GetObjectsBlackList");
            }
        }

        public async Task<EpisodeBlacklist> PutObjectBlackList(EpisodeBlacklist genericQueue)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                object rs = null;
                try
                {
                    rs = await connection.InsertAsync(genericQueue);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed PutObjectBlackList, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && !string.IsNullOrEmpty(rs.ToString()))
                    return genericQueue;
                else
                    throw new ApiNotFoundException("Not found PutObjectBlackList");
            }
        }
    }
}
