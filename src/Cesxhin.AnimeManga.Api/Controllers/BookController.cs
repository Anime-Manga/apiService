using Cesxhin.AnimeManga.Application.Exceptions;
using Cesxhin.AnimeManga.Application.HtmlAgilityPack;
using Cesxhin.AnimeManga.Application.Interfaces.Controllers;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Application.NlogManager;
using Cesxhin.AnimeManga.Domain.DTO;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class BookController : ControllerBase, IGeneralControllerBase<string, ChapterDTO, ChapterRegisterDTO, DownloadDTO, ProgressChapterDTO>
    {
        //interfaces
        private readonly IDescriptionBookService _bookService;
        private readonly IChapterService _chapterService;
        private readonly IChapterRegisterService _chapterRegisterService;
        private readonly IProgressChapterService _progressChapterService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        private readonly JObject _schema = JObject.Parse(Environment.GetEnvironmentVariable("SCHEMA"));

        public BookController(
            IDescriptionBookService bookService,
            IChapterService chapterService,
            IChapterRegisterService chapterRegisterService,
            IProgressChapterService progressChapterService,
            IBus publishEndpoint
            )
        {
            _publishEndpoint = publishEndpoint;
            _bookService = bookService;
            _chapterService = chapterService;
            _chapterRegisterService = chapterRegisterService;
            _progressChapterService = progressChapterService;
        }

        //get list all manga without filter
        [HttpGet("/book")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll(string nameCfg, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var listManga = await _bookService.GetNameAllAsync(nameCfg, username);
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listManga));
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

        //get manga by name
        [HttpGet("/book/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string nameCfg, string name, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var anime = await _bookService.GetNameByNameAsync(nameCfg, name, username);
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(anime));
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

        //get list manga by start name similar
        [HttpGet("/book/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string nameCfg, string name, string username)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var searchSchema = _schema.GetValue(nameCfg).ToObject<JObject>().GetValue("search").ToObject<JObject>();
                    var bookUrls = RipperBookGeneric.GetBookUrl(searchSchema, name);

                    //list anime
                    List<GenericUrlDTO> listManga = new();

                    foreach (var book in bookUrls)
                    {
                        var bookDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(book);

                        var checkManga = await _bookService.GetNameByNameAsync(nameCfg, book.Name, username);
                        if (checkManga != null)
                            bookDTO.Exists = true;

                        listManga.Add(bookDTO);
                    }
                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listManga));
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

        //get all db manga
        [HttpGet("/book/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericBookDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(string nameCfg, string username)
        {
            try
            {
                if (nameCfg != null && _schema.ContainsKey(nameCfg))
                {
                    var listManga = await _bookService.GetNameAllWithAllAsync(nameCfg, username);
                    return Ok(listManga);
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

        //get chapter by name anime
        [HttpGet("/chapter/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChapterDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectByName(string name)
        {
            try
            {
                var listChapters = await _chapterService.GetObjectsByNameAsync(name);
                return Ok(listChapters);
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

        //get chapter by id
        [HttpGet("/chapter/id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectById(string id)
        {
            try
            {
                var chapter = await _chapterService.GetObjectByIDAsync(id);
                return Ok(chapter);
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

        //get chapterRegister by id
        [HttpGet("/chapter/register/chapterid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectRegisterByObjectId(string id)
        {
            try
            {
                var chapterRegister = await _chapterRegisterService.GetObjectRegisterByObjectId(id);
                return Ok(chapterRegister);
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

        //get list name by external db
        [HttpGet("/book/list/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericUrlDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListSearchByName(string nameCfg, string name)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var searchSchema = _schema.GetValue(nameCfg).ToObject<JObject>().GetValue("search").ToObject<JObject>();
                    var bookUrls = RipperBookGeneric.GetBookUrl(searchSchema, name);

                    //list manga
                    List<GenericUrlDTO> list = new();

                    foreach (var bookUrl in bookUrls)
                    {
                        var bookUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(bookUrl);

                        //check if already exists
                        try
                        {
                            await _chapterService.GetObjectsByNameAsync(bookUrlDTO.Name);
                            bookUrlDTO.Exists = true;
                        }
                        catch (ApiNotFoundException) { }

                        list.Add(bookUrlDTO);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //insert manga
        [HttpPost("/book")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutInfo(string nameCfg, string infoClass)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    //insert
                    var mangaResult = await _bookService.InsertNameAsync(nameCfg, JObject.Parse(infoClass));
                    return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(mangaResult));
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

        //update manga
        [HttpPut("/book")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                    var mangaResult = await _bookService.UpdateNameAsync(nameCfg, description);
                    return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(mangaResult));
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

        //insert chapter
        [HttpPost("/chapter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObject(ChapterDTO objectClass)
        {
            try
            {
                //insert
                var chapterResult = await _chapterService.InsertObjectAsync(objectClass);
                return Created("none", chapterResult);
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

        //insert list chapters
        [HttpPost("/chapters")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<ChapterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjects(List<ChapterDTO> objectsClass)
        {
            try
            {
                //insert
                var chaptersResult = await _chapterService.InsertObjectsAsync(objectsClass);
                return Created("none", chaptersResult);
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

        //insert list chaptersRegisters
        [HttpPost("/chapters/registers")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<ChapterRegisterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectsRegisters(List<ChapterRegisterDTO> objectsRegistersClass)
        {
            try
            {
                //insert
                var chapterResult = await _chapterRegisterService.InsertObjectsRegistersAsync(objectsRegistersClass);
                return Created("none", chapterResult);
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

        //put chapterRegister into db
        [HttpPut("/chapter/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(ChapterRegisterDTO objectRegisterClass)
        {
            try
            {
                var chapterRegisterResult = await _chapterRegisterService.UpdateObjectRegisterAsync(objectRegisterClass);
                return Ok(chapterRegisterResult);
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


        //reset state download of chapterRegister into db
        [HttpPut("/book/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChapterDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(string name)
        {
            try
            {
                var result = await _chapterService.ResetStatusMultipleDownloadObjectByIdAsync(name);
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
                return StatusCode(500, ex.Message);
            }
        }

        //put manga into db
        [HttpPost("/book/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO downloadClass)
        {
            try
            {
                if (!_schema.ContainsKey(downloadClass.nameCfg))
                    return BadRequest();

                //get manga
                var manga = RipperBookGeneric.GetDescriptionBook(_schema.GetValue(downloadClass.nameCfg).ToObject<JObject>(), downloadClass.Url, downloadClass.nameCfg);
                string name = manga.GetValue("name_id").ToString();
                string cover = manga.GetValue("cover").ToString();

                //get chapters
                var chapters = RipperBookGeneric.GetChapters(
                    _schema.GetValue(downloadClass.nameCfg).ToObject<JObject>(),
                    downloadClass.Url,
                    name,
                    downloadClass.nameCfg
                   );

                try
                {
                    manga.GetValue("totalVolumes").ToObject<float>();
                    manga.GetValue("totalChapters").ToObject<float>();
                }
                catch
                {
                    float maxVolumes = 0;
                    int maxChapters = chapters.Count;

                    foreach (var chapter in chapters)
                    {
                        if (chapter.CurrentVolume > maxVolumes)
                            maxVolumes = chapter.CurrentVolume;
                    }

                    manga["totalVolumes"] = maxVolumes;
                    manga["totalChapters"] = maxChapters;
                }

                //insert manga
                var resultDescription = await _bookService.InsertNameAsync(downloadClass.nameCfg, manga);

                //insert chapters
                var listChapters = await _chapterService.InsertObjectsAsync(chapters);

                var listChapterRegister = new List<ChapterRegisterDTO>();
                List<string> chapterPaths = new();

                foreach (var chapter in chapters)
                {

                    for (int i = 0; i <= chapter.NumberMaxImage; i++)
                    {
                        chapterPaths.Add($"{_folder}/{chapter.NameManga}/Volume {chapter.CurrentVolume}/Chapter {chapter.CurrentChapter}/{chapter.NameManga} s{chapter.CurrentVolume}c{chapter.CurrentChapter}n{i}.png");
                    }

                    listChapterRegister.Add(new ChapterRegisterDTO
                    {
                        ChapterId = chapter.ID,
                        ChapterPath = chapterPaths.ToArray()
                    });

                    chapterPaths.Clear();
                }

                //insert episodesRegisters
                var episodeRegisterResult = await _chapterRegisterService.InsertObjectsRegistersAsync(listChapterRegister);

                //create message for notify
                string message = $"🧮ApiService say: \nAdd new Manga: {name}\n";

                try
                {
                    var messageNotify = new NotifyDTO
                    {
                        Message = message,
                        Image = cover
                    };
                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(resultDescription));
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
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        //update status chapter
        [HttpPut("/book/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(ChapterDTO objectClass)
        {
            try
            {
                //update
                var chapterResult = await _chapterService.UpdateStateDownloadAsync(objectClass);
                return Ok(chapterResult);
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

        //delete manga
        [HttpDelete("/book/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInfo(string nameCfg, string id)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var book = await _bookService.DeleteNameByIdAsync(nameCfg, id);

                    //create message for notify
                    string message = $"🧮ApiService say: \nRemoved this Manga by DB: {id}\n";

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

                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(book));
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
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        //put progress for tracker
        [HttpPut("/chapter/progress")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgressChapterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutStateProgress(ProgressChapterDTO progress)
        {
            try
            {
                var result = await _progressChapterService.UpdateProgress(progress);
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

        //get progress for tracker
        [HttpGet("/chapter/progress")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgressChapterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStateProgress(string name, string username, string nameCfg)
        {
            try
            {
                var result = await _progressChapterService.GetProgressByName(name, username, nameCfg);
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
    }
}
