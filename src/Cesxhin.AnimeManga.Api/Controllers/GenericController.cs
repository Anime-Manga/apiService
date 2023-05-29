using Cesxhin.AnimeManga.Modules.Exceptions;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Modules.Parallel;
using Cesxhin.AnimeManga.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class GenericController : ControllerBase
    {
        //interfaces
        private readonly IDescriptionVideoService _descriptionVideoService;
        private readonly IDescriptionBookService _descriptionBookService;

        //env
        private readonly JObject schemas = JObject.Parse(Environment.GetEnvironmentVariable("SCHEMA"));

        public GenericController(
            IDescriptionVideoService descriptionVideoService,
            IDescriptionBookService descriptionBookService
            )
        {
            _descriptionVideoService = descriptionVideoService;
            _descriptionBookService = descriptionBookService;
        }
        //check test
        [HttpGet("/cfg")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getSchema()
        {
            try
            {
                return Ok(Environment.GetEnvironmentVariable("SCHEMA"));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //check test
        [HttpGet("/check")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Check()
        {
            try
            {
                return Ok("Ok");
            }
            catch
            {
                return StatusCode(500);
            }
        }
        //get all db only saved by account
        [HttpGet("/all/watchlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOnlyWatchList(string username)
        {
            List<dynamic> listGeneric = new();
            dynamic result;
            try
            {
                foreach (var item in schemas)
                {
                    var schema = schemas.GetValue(item.Key).ToObject<JObject>();
                    if (schema.GetValue("type").ToString() == "video")
                    {
                        result = await _descriptionVideoService.GetNameAllOnlyWatchListAsync(item.Key, username);
                    }
                    else
                    {
                        result = await _descriptionBookService.GetNameAllOnlyWatchListAsync(item.Key, username);
                    }

                    if (result != null)
                        listGeneric.AddRange(result);
                }

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listGeneric));
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //get all db
        [HttpGet("/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(string username)
        {
            List<dynamic> listGeneric = new();
            dynamic result;
            try
            {
                foreach (var item in schemas)
                {
                    var schema = schemas.GetValue(item.Key).ToObject<JObject>();
                    result = null;

                    if (schema.GetValue("type").ToString() == "video")
                    {
                        try
                        {
                            result = await _descriptionVideoService.GetNameAllAsync(item.Key, username);
                        }
                        catch (ApiNotFoundException) { }
                    }
                    else
                    {
                        try
                        {
                            result = await _descriptionBookService.GetNameAllAsync(item.Key, username);
                        }
                        catch (ApiNotFoundException) { }
                    }

                    if (result != null)
                        listGeneric.AddRange(result);
                }

                if (listGeneric.Count > 0)
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listGeneric));
                else
                    throw new ApiNotFoundException();
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //get all db
        [HttpGet("/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSearch(string username)
        {
            List<dynamic> listGeneric = new();
            ParallelManager<IEnumerable<JObject>> parallel = new();
            List<Func<IEnumerable<JObject>>> tasks = new();
            dynamic result;
            try
            {
                foreach (var item in schemas)
                {
                    var schema = schemas.GetValue(item.Key).ToObject<JObject>();
                    if (schema.GetValue("type").ToString() == "video")
                    {
                        var key = item.Key;
                        tasks.Add(new Func<IEnumerable<JObject>>(() => _descriptionVideoService.GetNameAllAsync(key, username).GetAwaiter().GetResult()));
                    }
                    else
                    {
                        var key = item.Key;
                        tasks.Add(new Func<IEnumerable<JObject>>(() => _descriptionBookService.GetNameAllAsync(key, username).GetAwaiter().GetResult()));
                    }

                }

                parallel.AddTasks(tasks);
                parallel.Start();
                parallel.WhenCompleted();

                result = parallel.GetResult();

                foreach (var item in result)
                    listGeneric.AddRange(item);

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listGeneric));
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //put data check disk free space
        [HttpPut("/disk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DiskSpaceDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCheckDiskFreeSpace(DiskSpaceDTO disk)
        {
            try
            {
                Environment.SetEnvironmentVariable("CHECK_DISK_FREE_SPACE", disk.DiskSizeFree.ToString());
                Environment.SetEnvironmentVariable("CHECK_DISK_TOTAL_SPACE", disk.DiskSizeTotal.ToString());
                Environment.SetEnvironmentVariable("CHECK_DISK_INTERVAL", disk.Interval.ToString());

                var check = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                Environment.SetEnvironmentVariable("CHECK_DISK_LAST_CHECK", check.ToString());
                return Ok(disk);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get data check disk free space
        [HttpGet("/disk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DiskSpaceDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCheckDiskFreeSpace()
        {
            try
            {
                //get
                var checkDiskFree = Environment.GetEnvironmentVariable("CHECK_DISK_FREE_SPACE");
                var checkDiskTotal = Environment.GetEnvironmentVariable("CHECK_DISK_TOTAL_SPACE");
                var lastCheck = Environment.GetEnvironmentVariable("CHECK_DISK_LAST_CHECK");
                var interval = Environment.GetEnvironmentVariable("CHECK_DISK_INTERVAL");

                //check
                if (checkDiskTotal != null && checkDiskTotal != null)
                {
                    //return with object
                    var disk = new DiskSpaceDTO
                    {
                        DiskSizeTotal = long.Parse(checkDiskTotal),
                        DiskSizeFree = long.Parse(checkDiskFree),
                        LastCheck = long.Parse(lastCheck),
                        Interval = int.Parse(interval)
                    };
                    return Ok(disk);
                }
                else
                    return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put data check disk free space
        [HttpPut("/health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetHealtService(HealthDTO health)
        {
            try
            {
                Environment.SetEnvironmentVariable($"HEALT_SERVICE_{health.NameService.ToUpper()}_LAST_CHECK", health.LastCheck.ToString());
                Environment.SetEnvironmentVariable($"HEALT_SERVICE_{health.NameService.ToUpper()}_INTERVAL", health.Interval.ToString());
                return Ok(health);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get data check disk free space
        [HttpGet("/health")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HealthDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHealtService()
        {
            try
            {
                //set
                List<HealthDTO> healthServiceDTOs = new();

                string[] services = new string[8] { "DOWNLOAD", "UPGRADE-ANIME", "UPGRADE-MANGA", "API", "UPDATE-ANIME", "UPDATE-MANGA", "NOTIFY", "CONVERSION" };

                var lastCheck = "";
                var intervalCheck = "";

                //gets
                foreach (string service in services)
                {
                    var health = new HealthDTO
                    {
                        NameService = service
                    };

                    //get last check
                    lastCheck = Environment.GetEnvironmentVariable($"HEALT_SERVICE_{service}_LAST_CHECK");
                    if (lastCheck != null)
                        health.LastCheck = long.Parse(lastCheck);
                    else
                        health.LastCheck = 0;

                    //get interval
                    intervalCheck = Environment.GetEnvironmentVariable($"HEALT_SERVICE_{service}_INTERVAL");
                    if (intervalCheck != null)
                        health.Interval = int.Parse(intervalCheck);
                    else
                        health.Interval = 0;

                    healthServiceDTOs.Add(health);
                }
                return Ok(healthServiceDTOs);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
