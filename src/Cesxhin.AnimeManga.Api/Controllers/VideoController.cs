using Cesxhin.AnimeManga.Application.Interfaces.Controllers;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Application.NlogManager;
using Cesxhin.AnimeManga.Domain.DTO;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Cesxhin.AnimeManga.Application.HtmlAgilityPack;

namespace Cesxhin.AnimeManga.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnimeController : ControllerBase, IGeneralControllerBase<string, EpisodeDTO, EpisodeRegisterDTO, DownloadDTO>
    {
        //interfaces
        private readonly IDescriptionVideoService _descriptionService;
        private readonly IEpisodeService _episodeService;
        private readonly IEpisodeRegisterService _episodeRegisterService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        public AnimeController(
            IEpisodeService episodeService,
            IEpisodeRegisterService episodeRegisterService,
            IDescriptionVideoService descriptionService,
            IBus publishEndpoint
            )
        {
            _descriptionService = descriptionService;
            _episodeService = episodeService;
            _episodeRegisterService = episodeRegisterService;
            _publishEndpoint = publishEndpoint;
        }

        //get list all anime without filter
        [HttpGet("/video")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll()
        {
            try
            {
                var listAll = await _descriptionService.GetNameAllAsync();

                if (listAll == null)
                    return NotFound();

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listAll));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get anime by name
        [HttpGet("/video/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string name)
        {
            try
            {
                var description = await _descriptionService.GetNameByNameAsync(name);

                if (description == null)
                    return NotFound();

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(description));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list anime by start name similar
        [HttpGet("/video/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string name)
        {
            try
            {
                var description = await _descriptionService.GetMostNameByNameAsync(name);

                if (description == null)
                    return NotFound();

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(description));
            }
            catch
            {
                return StatusCode(500);
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

                if (listEpisodes == null)
                    return NotFound();

                return Ok(listEpisodes);
            }
            catch
            {
                return StatusCode(500);
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

                if (episode == null)
                    return NotFound();

                return Ok(episode);
            }
            catch
            {
                return StatusCode(500);
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

                if (episodeRegister == null)
                    return NotFound();

                return Ok(episodeRegister);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert anime
        [HttpPost("/video")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutInfo(string description)
        {
            try
            {
                //insert
                var descriptionResult = await _descriptionService.InsertNameAsync(JObject.Parse(description));

                if (descriptionResult == null)
                    return Conflict();

                return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(descriptionResult));
            }
            catch
            {
                return StatusCode(500);
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

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
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

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
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

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put episodeRegister into db
        [HttpPut("/episode/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(EpisodeRegisterDTO objectRegisterClass)
        {
            try
            {
                var rs = await _episodeRegisterService.UpdateObjectRegisterAsync(objectRegisterClass);
                if (rs == null)
                    return NotFound();

                return Ok(rs);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put anime into db
        [HttpPost("/video/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO downloadClass)
        {
            try
            {
                JObject cfg = null;
                //read schema
                try
                {
                    cfg = RipperSchema.readFile(downloadClass.nameCfg);
                }
                catch(Exception e)
                {
                    return NotFound();
                }

                //get anime and episodes
                var description = RipperVideoGeneric.GetAnime(cfg, downloadClass.Url);
                var episodes = RipperVideoGeneric.GetEpisodes(cfg, downloadClass.Url, description["name_id"].ToString());

                var listEpisodeRegister = new List<EpisodeRegisterDTO>();

                foreach (var episode in episodes)
                {
                    listEpisodeRegister.Add(new EpisodeRegisterDTO
                    {
                        EpisodeId = episode.ID,
                        EpisodePath = $"{_folder}/{episode.VideoId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.VideoId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4"
                    });
                }

                var descriptionResult = await _descriptionService.InsertNameAsync(JObject.Parse(description.ToString()));

                if (descriptionResult == null)
                    return Conflict();

                //insert episodes
                var episodeResult = await _episodeService.InsertObjectsAsync(episodes);

                if (episodeResult == null)
                    return Conflict();

                //insert episodesRegisters
                var episodeRegisterResult = await _episodeRegisterService.InsertObjectsRegistersAsync(listEpisodeRegister);

                if (episodeResult == null)
                    return Conflict();

                //create message for notify
                string message = $"🧮ApiService say: \nAdd new Anime: {description["name_id"]}\n";

                try
                {
                    var messageNotify = new NotifyDTO
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
            catch
            {
                return StatusCode(500);
            }
        }

        //reset state download of episodeRegister into db
        [HttpPut("/video/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(List<EpisodeDTO> objectsClass)
        {
            try
            {
                foreach (var episode in objectsClass)
                {
                    episode.StateDownload = null;
                    await _episodeService.ResetStatusDownloadObjectByIdAsync(episode);
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //delete description
        [HttpDelete("/video/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInfo(string id)
        {
            try
            {
                //insert
                var animeResult = await _descriptionService.DeleteNameByIdAsync(id);

                if (animeResult == null)
                    return NotFound();
                else if (animeResult == "-1")
                    return Conflict();

                //create message for notify
                string message = $"🧮ApiService say: \nRemoved this Anime by DB and Plex: {id}\n";

                try
                {
                    var messageNotify = new NotifyDTO
                    {
                        Message = message
                    };
                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Ok(animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get all db anime
        [HttpGet("/video/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listDescription = await _descriptionService.GetNameAllWithAllAsync();

                if (listDescription == null)
                    return NotFound();

                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listDescription));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/video/list/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericUrlDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListSearchByName(string name)
        {
            try
            {
                var descriptionUrls = RipperVideoGeneric.GetAnimeUrl(name);
                if (descriptionUrls != null || descriptionUrls.Count >= 0)
                {
                    //list anime
                    List<GenericUrlDTO> list = new();

                    foreach (var descrptionUrl in descriptionUrls)
                    {
                        var descriptionUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(descrptionUrl);

                        //check if already exists
                        var description = await _episodeService.GetObjectsByNameAsync(descriptionUrlDTO.Name);
                        if (description != null)
                            descriptionUrlDTO.Exists = true;

                        list.Add(descriptionUrlDTO);
                    }
                    return Ok(list);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //update status episode
        [HttpPut("/video/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(EpisodeDTO objectClass)
        {
            try
            {
                //update
                var resultEpisode = await _episodeService.UpdateStateDownloadAsync(objectClass);
                if (resultEpisode == null)
                    return NotFound();

                return Ok(resultEpisode);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}