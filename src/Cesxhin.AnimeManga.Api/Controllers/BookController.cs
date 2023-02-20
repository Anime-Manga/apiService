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
    public class BookController : ControllerBase, IGeneralControllerBase<string, ChapterDTO, ChapterRegisterDTO, DownloadDTO>
    {
        //interfaces
        private readonly IDescriptionBookService _bookService;
        private readonly IChapterService _chapterService;
        private readonly IChapterRegisterService _chapterRegisterService;
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
            IBus publishEndpoint
            )
        {
            _publishEndpoint = publishEndpoint;
            _bookService = bookService;
            _chapterService = chapterService;
            _chapterRegisterService = chapterRegisterService;
        }

        //get list all manga without filter
        [HttpGet("/book")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll(string nameCfg)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var listManga = await _bookService.GetNameAllAsync(nameCfg);

                    if (listManga == null)
                        return NotFound();

                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listManga));
                }
                else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get manga by name
        [HttpGet("/book/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string nameCfg, string name)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var anime = await _bookService.GetNameByNameAsync(nameCfg, name);

                    if (anime == null)
                        return NotFound();

                    return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(anime));
                }else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list manga by start name similar
        [HttpGet("/book/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string nameCfg, string name)
        {
            try
            {
                if (_schema.ContainsKey(nameCfg))
                {
                    var searchSchema = _schema.GetValue(nameCfg).ToObject<JObject>().GetValue("search").ToObject<JObject>();
                    var bookUrls = RipperBookGeneric.GetBookUrl(searchSchema, name);
                    if (bookUrls != null && bookUrls.Count > 0)
                    {
                        //list anime
                        List<GenericUrlDTO> listManga = new();

                        foreach (var book in bookUrls)
                        {
                            var bookDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(book);

                            var checkManga = await _bookService.GetNameByNameAsync(nameCfg, book.Name);
                            if (checkManga != null)
                                bookDTO.Exists = true;

                            listManga.Add(bookDTO);
                        }
                        return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(listManga));
                    }
                    return NotFound();
                }
                else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get all db manga
        [HttpGet("/book/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericBookDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(string nameCfg)
        {
            try
            {
                if (nameCfg != null && _schema.ContainsKey(nameCfg))
                {
                    var listManga = await _bookService.GetNameAllWithAllAsync(nameCfg);

                    if (listManga == null)
                        return NotFound();

                    return Ok(listManga);
                }else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
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

                if (listChapters == null)
                    return NotFound();

                return Ok(listChapters);
            }
            catch
            {
                return StatusCode(500);
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

                if (chapter == null)
                    return NotFound();

                return Ok(chapter);
            }
            catch
            {
                return StatusCode(500);
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

                if (chapterRegister == null)
                    return NotFound();

                return Ok(chapterRegister);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/book/list/name/{name}")]
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
                    var bookUrls = RipperBookGeneric.GetBookUrl(searchSchema, name);
                    if (bookUrls != null && bookUrls.Count >= 0)
                    {
                        //list manga
                        List<GenericUrlDTO> list = new();

                        foreach (var bookUrl in bookUrls)
                        {
                            var bookUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(bookUrl);

                            //check if already exists
                            var book = await _chapterService.GetObjectsByNameAsync(bookUrlDTO.Name);
                            if (book != null)
                                bookUrlDTO.Exists = true;

                            list.Add(bookUrlDTO);
                        }
                        return Ok(list);
                    }
                    return NotFound();
                }
                else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert manga
        [HttpPost("/book")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
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

                    if (mangaResult == null)
                        return Conflict();

                    return Created("none", Newtonsoft.Json.JsonConvert.SerializeObject(mangaResult));
                }else
                    return BadRequest();
            }
            catch
            {
                return StatusCode(500);
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

                if (chapterResult == null)
                    return Conflict();

                return Created("none", chapterResult);
            }
            catch
            {
                return StatusCode(500);
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

                if (chaptersResult == null)
                    return Conflict();

                return Created("none", chaptersResult);
            }
            catch
            {
                return StatusCode(500);
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

                if (chapterResult == null)
                    return Conflict();

                return Created("none", chapterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put chapterRegister into db
        [HttpPut("/chapter/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(ChapterRegisterDTO objectRegisterClass)
        {
            try
            {
                var chapterRegisterResult = await _chapterRegisterService.UpdateObjectRegisterAsync(objectRegisterClass);
                if (chapterRegisterResult == null)
                    return NotFound();

                return Ok(chapterRegisterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        //reset state download of chapterRegister into db
        [HttpPut("/book/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(List<ChapterDTO> objectsClass)
        {
            try
            {
                foreach (var chapter in objectsClass)
                {
                    chapter.StateDownload = null;
                    await _chapterService.ResetStatusDownloadObjectByIdAsync(chapter);
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put manga into db
        [HttpPost("/book/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO downloadClass)
        {
            try
            {
                if (!_schema.ContainsKey(downloadClass.nameCfg))
                    return BadRequest();

                //get manga
                var manga = RipperBookGeneric.GetDescriptionBook(_schema.GetValue(downloadClass.nameCfg).ToObject<JObject>(), downloadClass.Url);
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

                if (resultDescription == null)
                    return Conflict();

                //insert chapters
                var listChapters = await _chapterService.InsertObjectsAsync(chapters);

                if (listChapters == null)
                    return Conflict();

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

                if (episodeRegisterResult == null)
                    return Conflict();

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
            catch(Exception e)
            {
                _logger.Error(e);
                return StatusCode(500);
            }
        }

        //update status chapter
        [HttpPut("/book/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(ChapterDTO objectClass)
        {
            try
            {
                //update
                var chapterResult = await _chapterService.UpdateStateDownloadAsync(objectClass);
                if (chapterResult == null)
                    return NotFound();

                return Ok(chapterResult);
            }
            catch
            {
                return StatusCode(500);
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

                    if (book == null)
                        return NotFound();

                    if (book == "-1")
                        return Conflict();

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
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
