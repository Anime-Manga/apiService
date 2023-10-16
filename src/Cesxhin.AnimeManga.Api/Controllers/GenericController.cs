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
    }
}
