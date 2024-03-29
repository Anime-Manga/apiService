﻿using Cesxhin.AnimeManga.Modules.Exceptions;
using Cesxhin.AnimeManga.Modules.HtmlAgilityPack;
using Cesxhin.AnimeManga.Application.Interfaces.Controllers;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Modules.NlogManager;
using Cesxhin.AnimeManga.Domain.DTO;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnimeController : ControllerBase, IGeneralControllerBase<string, EpisodeDTO, EpisodeRegisterDTO, DownloadDTO, ProgressEpisodeDTO, GenericQueueDTO, GenericBlackListDTO>
    {
        //interfaces
        private readonly IDescriptionVideoService _descriptionService;
        private readonly IEpisodeService _episodeService;
        private readonly IEpisodeRegisterService _episodeRegisterService;
        private readonly IProgressEpisodeService _progressEpisodeService;
        private readonly IEpisodeQueueService _episodeQueueService;
        private readonly IAccountService _accountService;
        private readonly IEpisodeBlackListService _episodeBlackListService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        private readonly JObject _schema = JObject.Parse(Environment.GetEnvironmentVariable("SCHEMA"));

        public AnimeController(
            IEpisodeService episodeService,
            IEpisodeRegisterService episodeRegisterService,
            IDescriptionVideoService descriptionService,
            IProgressEpisodeService progressEpisodeService,
            IEpisodeQueueService episodeQueueService,
            IEpisodeBlackListService episodeBlackListService,
            IAccountService accountService,
            IBus publishEndpoint
            )
        {
            _descriptionService = descriptionService;
            _episodeService = episodeService;
            _episodeRegisterService = episodeRegisterService;
            _publishEndpoint = publishEndpoint;
            _episodeQueueService = episodeQueueService;
            _progressEpisodeService = progressEpisodeService;
            _accountService = accountService;
            _episodeBlackListService = episodeBlackListService;
        }

        //get list all anime without filter
        [HttpGet("/video")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll(string nameCfg, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var listAll = await _descriptionService.GetNameAllAsync(nameCfg, username);
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listAll));
                }
                else
                    return BadRequest();
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

        //get anime by name
        [HttpGet("/video/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string name, string nameCfg, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var description = await _descriptionService.GetNameByNameAsync(nameCfg, name, username);
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(description));
                }
                else
                    return BadRequest();
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

        //get list anime by start name similar
        [HttpGet("/video/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string nameCfg, string name, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var description = await _descriptionService.GetMostNameByNameAsync(nameCfg, name, username);
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(description));
                }
                else
                    return BadRequest();
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

        //get episode by name anime
        [HttpGet("/episode/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EpisodeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectByName(string name)
        {
            try
            {
                var listEpisodes = await _episodeService.GetObjectsByNameAsync(name);
                return Ok(listEpisodes);
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

        //get episode by id
        [HttpGet("/episode/id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectById(string id)
        {
            try
            {
                var episode = await _episodeService.GetObjectByIDAsync(id);
                return Ok(episode);
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

        //get episodeRegister by id
        [HttpGet("/episode/register/episodeid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectRegisterByObjectId(string id)
        {
            try
            {
                var episodeRegister = await _episodeRegisterService.GetObjectRegisterByObjectId(id);
                return Ok(episodeRegister);
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

        //insert anime
        [HttpPost("/video")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutInfo(string nameCfg, string description)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    //insert
                    var descriptionResult = await _descriptionService.InsertNameAsync(nameCfg, JObject.Parse(description));
                    return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(descriptionResult));
                }
                else
                    return BadRequest();
            }
            catch (ApiConflictException)
            {
                return Conflict();
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

        //update anime
        [HttpPut("/video")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInfo([FromBody] string content)
        {
            try
            {
                var description = JObject.Parse(content);
                string nameCfg = (string)description["nameCfg"];

                if (_schema.ContainsKey(nameCfg))
                {
                    //update
                    var descriptionResult = await _descriptionService.UpdateNameAsync(nameCfg, description);
                    return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(descriptionResult));
                }
                else
                    return BadRequest();
            }
            catch (ApiConflictException)
            {
                return Conflict();
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

        //insert episode
        [HttpPost("/episode")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObject(EpisodeDTO objectClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertObjectAsync(objectClass);
                return Created("none", episodeResult);
            }
            catch (ApiConflictException)
            {
                return Conflict();
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

        //insert list episodes
        [HttpPost("/episodes")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<EpisodeDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjects(List<EpisodeDTO> objectsClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertObjectsAsync(objectsClass);
                return Created("none", episodeResult);
            }
            catch (ApiConflictException)
            {
                return Conflict();
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

        //insert list episodesRegisters
        [HttpPost("/episodes/registers")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<EpisodeRegisterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectsRegisters(List<EpisodeRegisterDTO> objectsRegistersClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeRegisterService.InsertObjectsRegistersAsync(objectsRegistersClass);
                return Created("none", episodeResult);
            }
            catch (ApiConflictException)
            {
                return Conflict();
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

        //put episodeRegister into db
        [HttpPut("/episode/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(EpisodeRegisterDTO objectRegisterClass)
        {
            try
            {
                var rs = await _episodeRegisterService.UpdateObjectRegisterAsync(objectRegisterClass);
                return Ok(rs);
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

        //put anime into db
        [HttpPost("/video/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO downloadClass, string username)
        {
            try
            {
                AuthDTO account = null;
                try
                {
                    account = await _accountService.FindAccountByUsername(username);
                }
                catch(ApiNotFoundException)
                {
                    throw new ApiNotAuthorizeException();
                }

                if (account.Role != 100)
                    throw new ApiNotAuthorizeException();

                JObject cfg = null;

                if (!_schema.ContainsKey(downloadClass.nameCfg))
                    return BadRequest();

                cfg = _schema.GetValue(downloadClass.nameCfg).ToObject<JObject>();

                //get anime and episodes
                var description = RipperVideoGeneric.GetDescriptionVideo(cfg, downloadClass.Url, downloadClass.nameCfg);
                var episodes = RipperVideoGeneric.GetEpisodes(cfg, downloadClass.Url, description["name_id"].ToString(), downloadClass.nameCfg);

                var listEpisodeRegister = new List<EpisodeRegisterDTO>();

                foreach (var episode in episodes)
                {
                    listEpisodeRegister.Add(new EpisodeRegisterDTO
                    {
                        EpisodeId = episode.ID,
                        EpisodePath = $"{_folder}/{episode.VideoId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.VideoId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4"
                    });
                }

                var descriptionResult = await _descriptionService.InsertNameAsync(downloadClass.nameCfg, JObject.Parse(description.ToString()));

                //insert episodes
                var episodeResult = await _episodeService.InsertObjectsAsync(episodes);

                //insert episodesRegisters
                var episodeRegisterResult = await _episodeRegisterService.InsertObjectsRegistersAsync(listEpisodeRegister);

                //delete if exist queue
                try
                {
                    await _episodeQueueService.DeleteObjectQueue(new GenericQueueDTO {
                        NameCfg = downloadClass.nameCfg,
                        Url = downloadClass.Url
                    });
                }
                catch (ApiNotFoundException) { }

                //create message for notify
                string message = $"Added: {description["name_id"]} [Anime]\n";

                try
                {
                    var messageNotify = new NotifyAnimeDTO
                    {
                        Message = message,
                        Image = description["cover"].ToString()
                    };
                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(descriptionResult));
            }
            catch (ApiConflictException)
            {
                return Conflict();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (ApiNotAuthorizeException)
            {
                return StatusCode(401);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return StatusCode(500);
            }
        }

        //reset state download of episodeRegister into db
        [HttpPut("/video/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EpisodeDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(string name, string username)
        {
            try
            {
                AuthDTO account = null;
                try
                {
                    account = await _accountService.FindAccountByUsername(username);
                }
                catch (ApiNotFoundException)
                {
                    throw new ApiNotAuthorizeException();
                }

                if (account.Role != 100)
                    throw new ApiNotAuthorizeException();

                var rs = await _episodeService.ResetStatusMultipleDownloadObjectByIdAsync(name);
                return Ok(rs);
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (ApiNotAuthorizeException)
            {
                return StatusCode(401);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //delete description
        [HttpDelete("/video/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInfo(string nameCfg, string id, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    AuthDTO account = null;
                    try
                    {
                        account = await _accountService.FindAccountByUsername(username);
                    }
                    catch (ApiNotFoundException)
                    {
                        throw new ApiNotAuthorizeException();
                    }

                    if (account.Role != 100)
                        throw new ApiNotAuthorizeException();

                    var listEpisodeService = await _episodeService.GetObjectsByNameAsync(id);
                    var listEpisodeRegister = await _episodeRegisterService.GetObjectsRegistersByListObjectId(listEpisodeService.ToList());
                    var videoDescription = await _descriptionService.GetNameByNameAsync(nameCfg, id, null);

                    //delete
                    var videoResult = await _descriptionService.DeleteNameByIdAsync(nameCfg, id);

                    //create message for notify
                    string message = $"Removed: {id} [Anime]\n";

                    try
                    {
                        var messageNotify = new NotifyAnimeDTO
                        {
                            Message = message,
                            Image = videoDescription.GetValue("cover").ToString()
                        };
                        await _publishEndpoint.Publish(messageNotify);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                    }

                    foreach (var episode in listEpisodeRegister)
                    {
                        try
                        {
                            await _publishEndpoint.Publish(episode);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                        }
                    }

                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(videoResult));
                }
                else
                    return BadRequest();
            }
            catch (ApiConflictException)
            {
                return Conflict();
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //get all db anime
        [HttpGet("/video/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericVideoDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(string nameCfg, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var listDescription = await _descriptionService.GetNameAllWithAllAsync(nameCfg, username);
                    return Ok(listDescription);
                }
                else
                    return BadRequest();
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/video/list/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericUrlDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListSearchByName(string nameCfg, string name)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var searchSchema = _schema.GetValue(nameCfg).ToObject<JObject>().GetValue("search").ToObject<JObject>();
                    var descriptionUrls = RipperVideoGeneric.GetVideoUrl(searchSchema, name);

                    List<GenericBlackListDTO> listBlackList = new();
                    try
                    {
                        listBlackList = await _episodeBlackListService.GetObjectsBlackList();
                    }
                    catch (ApiNotFoundException) { }

                    if (listBlackList.Any())
                    {
                        descriptionUrls = descriptionUrls.Where(book =>
                            listBlackList.Where(blackList =>
                                book.Name == blackList.Name &&
                                book.Url == blackList.Url &&
                                nameCfg == blackList.NameCfg
                            ).ToList().Count == 0
                        ).ToList();
                    }

                    //list anime
                    List<GenericUrlDTO> list = new();

                    foreach (var descrptionUrl in descriptionUrls)
                    {
                        var descriptionUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(descrptionUrl);

                        //check if already exists
                        try
                        {
                            await _descriptionService.GetNameByNameAsync(nameCfg, descriptionUrlDTO.Name, null);
                            descriptionUrlDTO.Exists = true;
                        }
                        catch (ApiNotFoundException) { }

                        list.Add(descriptionUrlDTO);
                    }
                    return Ok(list);
                }
                else
                    return BadRequest();
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //update status episode
        [HttpPut("/video/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(EpisodeDTO objectClass)
        {
            try
            {
                //update
                var resultEpisode = await _episodeService.UpdateStateDownloadAsync(objectClass);
                return Ok(resultEpisode);
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //put progress for tracker
        [HttpPut("/episode/progress")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgressEpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutStateProgress(ProgressEpisodeDTO progress)
        {
            try
            {
                var result = await _progressEpisodeService.UpdateProgress(progress);
                return Ok(result);
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //put progress for tracker
        [HttpGet("/episode/progress")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgressEpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStateProgress(string name, string username, string nameCfg)
        {
            try
            {
                var result = await _progressEpisodeService.GetProgressByName(name, username, nameCfg);
                return Ok(result);
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/episode/all-queue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericQueueDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectsQueue()
        {
            try
            {
                var result = await _episodeQueueService.GetObjectsQueue();
                return Ok(result);
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
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/episode/queue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericQueueDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectQueue(GenericQueueDTO objectClass)
        {
            try
            {
                var result = await _episodeQueueService.PutObjectQueue(objectClass);

                //create message for notify
                string message = $"Someone likes: {objectClass.Name} [Anime]\n";

                try
                {
                    var messageNotify = new NotifyRequestAnimeDTO
                    {
                        Message = message
                    };

                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Ok(result);
            }
            catch (ApiNotFoundException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (ApiConflictException)
            {
                return Conflict();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/episode/queue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericQueueDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteObjectQueue(GenericQueueDTO objectClass)
        {
            try
            {
                var result = await _episodeQueueService.DeleteObjectQueue(objectClass);
                return Ok(result);
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
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/episode/queue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericQueueDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectQueue(string name, string url, string nameCfg)
        {
            try
            {
                var result = await _episodeQueueService.GetObjectQueue(new GenericQueueDTO
                {
                    Name = name,
                    NameCfg = nameCfg,
                    Url = url
                });
                return Ok(result);
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
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/episode/blacklist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericBlackListDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectBlackList(GenericBlackListDTO objectClass)
        {
            try
            {
                var result = await _episodeBlackListService.PutObjectBlackList(objectClass);

                await _episodeQueueService.DeleteObjectQueue(new GenericQueueDTO {
                    Name = objectClass.Name,
                    Url = objectClass.Url,
                    NameCfg = objectClass.NameCfg,
                });

                return Ok(result);
            }
            catch (ApiConflictException)
            {
                return NotFound();
            }
            catch (ApiGenericException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}