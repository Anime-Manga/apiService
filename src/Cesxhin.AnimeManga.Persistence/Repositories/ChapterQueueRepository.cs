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
    public class ChapterQueueRepository : IChapterQueueRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<int> DeleteObjectQueue(ChapterQueue objectGeneral)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs;
                try
                {
                    rs = await connection.DeleteAsync<ChapterQueue>(e => e.Url == objectGeneral.Url && e.NameCfg == objectGeneral.NameCfg);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed DeleteChapterQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }


                if (rs > 0)
                    return rs;
                else
                    throw new ApiNotFoundException("Not found DeleteChapterQueue");
            }
        }

        public async Task<ChapterQueue> GetObjectQueue(ChapterQueue genericQueue)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<ChapterQueue> rs;
                try
                {
                    rs = await connection.QueryAsync<ChapterQueue>(e => e.Url == genericQueue.Url && e.NameCfg == genericQueue.NameCfg);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChapterQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs.First();
                else
                    throw new ApiNotFoundException("Not found GetChapterQueue");
            }
        }

        public async Task<IEnumerable<ChapterQueue>> GetObjectsQueue()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<ChapterQueue> rs;

                try
                {
                    rs = await connection.QueryAllAsync<ChapterQueue>();
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

        public async Task<ChapterQueue> PutObjectQueue(ChapterQueue genericQueue)
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
                    _logger.Error($"Failed PutChapterQueue, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && !string.IsNullOrEmpty(rs.ToString()))
                    return genericQueue;
                else
                    throw new ApiNotFoundException("Not found PutChapterQueue");
            }
        }
    }
}
