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
    public class EpisodeQueueRepository : IEpisodeQueueRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<int> DeleteObjectQueue(EpisodeQueue objectGeneral)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs;
                try
                {
                    rs = await connection.DeleteAsync<EpisodeQueue>(e => e.Url == objectGeneral.Url && e.NameCfg == objectGeneral.NameCfg);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed DeleteEpisodeQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }


                if (rs > 0)
                    return rs;
                else
                    throw new ApiNotFoundException("Not found DeleteEpisodeQueue");
            }
        }

        public async Task<EpisodeQueue> GetObjectQueue(EpisodeQueue genericQueue)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<EpisodeQueue> rs;
                try
                {
                    rs = await connection.QueryAsync<EpisodeQueue>(e => e.Url == genericQueue.Url && e.NameCfg == genericQueue.NameCfg);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetEpisodeQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs.First();
                else
                    throw new ApiNotFoundException("Not found GetEpisodeQueue");
            }
        }

        public async Task<IEnumerable<EpisodeQueue>> GetObjectsQueue()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<EpisodeQueue> rs;

                try
                {
                    rs = await connection.QueryAllAsync<EpisodeQueue>();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetObjectsQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs;
                else
                    throw new ApiNotFoundException("Not found GetObjectsQueue");
            }
        }

        public async Task<EpisodeQueue> PutObjectQueue(EpisodeQueue genericQueue)
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
                    _logger.Error($"Failed PutEpisodeQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && !string.IsNullOrEmpty(rs.ToString()))
                    return genericQueue;
                else
                    throw new ApiNotFoundException("Not found PutEpisodeQueue");
            }
        }
    }
}
