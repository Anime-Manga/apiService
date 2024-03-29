﻿using Cesxhin.AnimeManga.Modules.Exceptions;
using Cesxhin.AnimeManga.Modules.Generic;
using Cesxhin.AnimeManga.Application.Interfaces.Repositories;
using Cesxhin.AnimeManga.Modules.NlogManager;
using Cesxhin.AnimeManga.Domain.Models;
using NLog;
using Npgsql;
using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Persistence.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<int> DeleteByNameAsync(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs = 0;
                try
                {
                    rs = await connection.DeleteAsync<Chapter>(e => e.NameManga == id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChapterByIDAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs > 0)
                    return rs;
                else
                    throw new ApiNotFoundException("Not found DeleteByNameAsync");
            }
        }

        public async Task<Chapter> GetObjectByIDAsync(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<Chapter> rs;

                try
                {
                    rs = await connection.QueryAsync<Chapter>(e => e.ID == id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChapterByIDAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return rs.First();
                else
                    throw new ApiNotFoundException("Not found GetObjectsByIDAsync");
            }
        }

        public async Task<IEnumerable<Chapter>> GetObjectsByNameAsync(string nameManga)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                IEnumerable<Chapter> rs;

                try
                {
                    rs = await connection.QueryAsync<Chapter>(e => e.NameManga == nameManga, orderBy: OrderField.Parse(new
                    {
                        currentchapter = Order.Ascending
                    }));
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChaptersByNameAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && rs.Any())
                    return ConvertGeneric<Chapter>.ConvertIEnurableToListCollection(rs);
                else
                    throw new ApiNotFoundException("Not found GetObjectsByNameAsync");
            }
        }

        public async Task<Chapter> InsertObjectAsync(Chapter chapter)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                object rs = null;

                try
                {
                    rs = await connection.InsertAsync(chapter);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed InsertObjectAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs != null && !string.IsNullOrEmpty(rs.ToString()))
                    return chapter;
                else
                    throw new ApiNotFoundException("Not found InsertObjectAsync");
            }
        }

        public async Task<IEnumerable<Chapter>> InsertObjectsAsync(List<Chapter> chapters)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs = 0;

                try
                {
                    rs = await connection.InsertAllAsync(chapters);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed InsertChapterAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs > 0)
                    return chapters;
                else
                    throw new ApiNotFoundException("Not found InsertObjectsAsync");
            }
        }

        public async Task<Chapter> ResetStatusDownloadObjectByIdAsync(Chapter chapter)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs = 0;

                chapter.PercentualDownload = 0;
                chapter.StateDownload = null;

                try
                {
                    rs = await connection.UpdateAsync(chapter, e => e.StateDownload != "completed" && e.ID == chapter.ID);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed ResetStatusDownloadChaptersByIdAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs > 0)
                    return chapter;
                else
                    throw new ApiNotFoundException("Not found ResetStatusDownloadObjectByIdAsync");
            }
        }

        public async Task<IEnumerable<Chapter>> ResetStatusDownloadObjectsByIdAsync(List<Chapter> chapters)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs = 0;
                try
                {
                    var chaptersUpdate = chapters.Where(e => e.StateDownload != "completed").ToList();
                    chaptersUpdate.ForEach(e =>
                    {
                        e.PercentualDownload = 0;
                        e.StateDownload = null;
                    });

                    rs = await connection.UpdateAllAsync(chaptersUpdate);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed ResetStatusDownloadChaptersByIdAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs > 0)
                    return chapters;
                else
                    throw new ApiNotFoundException("Not found ResetStatusDownloadObjectsByIdAsync");
            }
        }

        public async Task<Chapter> UpdateStateDownloadAsync(Chapter chapter)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                int rs = 0;
                try
                {
                    rs = await connection.UpdateAsync(chapter);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed UpdateStateDownloadAsync, details error: {ex.Message}");
                    throw new ApiGenericException(ex.Message);
                }

                if (rs > 0)
                    return chapter;
                else
                    throw new ApiNotFoundException("Not found UpdateStateDownloadAsync");
            }
        }
    }
}
