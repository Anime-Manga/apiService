﻿using Cesxhin.AnimeManga.Application.Exceptions;
using Cesxhin.AnimeManga.Application.Generic;
using Cesxhin.AnimeManga.Application.NlogManager;
using Cesxhin.AnimeManga.Application.Parallel;
using Cesxhin.AnimeManga.Application.Proxy;
using Cesxhin.AnimeManga.Domain.DTO;
using Cesxhin.AnimeManga.Domain.Models;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Consumers
{
    public class DownloadVideoConsumer : IConsumer<EpisodeDTO>
    {
        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //parallel
        private readonly ParallelManager<EpisodeBuffer> parallel = new();

        //temp
        private string pathTemp = Environment.GetEnvironmentVariable("PATH_TEMP") ?? "D:\\TestVideo\\temp";

        //download
        private readonly int MAX_DELAY = int.Parse(Environment.GetEnvironmentVariable("MAX_DELAY") ?? "5");
        private readonly int DELAY_RETRY_ERROR = int.Parse(Environment.GetEnvironmentVariable("DELAY_RETRY_ERROR") ?? "10000");

        public Task Consume(ConsumeContext<EpisodeDTO> context)
        {
            //get body
            var episode = context.Message;

            //api
            Api<EpisodeRegisterDTO> episodeRegisterApi = new();
            Api<GenericVideoDTO> videoApi = new();
            Api<EpisodeDTO> episodeApi = new();

            EpisodeRegisterDTO episodeRegister = null;
            EpisodeDTO episodeVerify = null;

            //episodeRegister
            try
            {
                episodeRegister = episodeRegisterApi.GetOne($"/episode/register/episodeid/{episode.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //episode
            try
            {
                episodeVerify = episodeApi.GetOne($"/episode/id/{episode.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //check duplication messages
            if (episodeVerify != null && episodeVerify.StateDownload == "pending")
            {
                //paths
                var directoryPath = Path.GetDirectoryName(episodeRegister.EpisodePath);
                var filePathTemp = Path.GetFullPath($"{pathTemp}/{Path.GetFileName(episodeRegister.EpisodePath)}");

                //check directory
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (!Directory.Exists(Path.GetDirectoryName(filePathTemp)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filePathTemp));

                //check type url
                if (episode.UrlVideo != null)
                {
                    //api
                    Api<EpisodeDTO> episodeDTOApi = new();

                    //url with file
                    using (var client = new MyWebClient())
                    {
                        //set proxy
                        var ip = ProxyManagement.GetIp(episode.VideoId);

                        //task
                        client.DownloadProgressChanged += client_DownloadProgressChanged(filePathTemp, episode);
                        client.DownloadFileCompleted += client_DownloadFileCompleted(filePathTemp, episode);

                        //setup
                        int timeout = 0;

                        //send api failed download
                        episode.StateDownload = "downloading";
                        episode.PercentualDownload = 0;
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                        do
                        {
                            if (ip != null)
                                client.Proxy = new WebProxy(new Uri(ip));
                            else
                                client.Proxy = null;

                            if (timeout >= MAX_DELAY)
                            {
                                if (ProxyManagement.EnableProxy() && ip != null)
                                {
                                    ProxyManagement.BlackListAdd(ip, episode.VideoId);
                                    timeout = 0;
                                    ip = ProxyManagement.GetIp(episode.VideoId);
                                    continue;
                                }
                                else
                                {
                                    //send api failed download
                                    episode.StateDownload = "failed";
                                    SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                                    _logger.Error($"Failed download, details: {episode.UrlVideo}");
                                    return null;
                                }
                            }


                            _logger.Info("try download: " + episode.UrlVideo);
                            try
                            {
                                Dictionary<string, string> query = new()
                                {
                                    ["nameCfg"] = episode.nameCfg
                                };
                                var video = videoApi.GetOne($"/video/name/{episode.VideoId}", query).GetAwaiter().GetResult();

                                //setup client
                                client.Timeout = 60000; //? check

                                //start download
                                client.DownloadFileTaskAsync(new Uri(episode.UrlVideo), filePathTemp).ConfigureAwait(false).GetAwaiter().GetResult();

                                File.Move(filePathTemp, episodeRegister.EpisodePath, true);
                                break;
                            }
                            catch (ApiNotFoundException ex)
                            {
                                _logger.Error($"Not found anime so can't set headers referer for download, details: {ex.Message}");
                            }
                            catch (WebException ex)
                            {
                                _logger.Error(ex);
                                _logger.Warn($"The attempts remains: {MAX_DELAY - timeout} for {episode.UrlVideo}");
                                timeout++;

                                Thread.Sleep(DELAY_RETRY_ERROR);
                            }
                            catch (Exception ex)
                            {
                                _logger.Fatal($"Error download with url easy, details error: {ex.Message}");
                                timeout = MAX_DELAY;
                            }
                        } while (true);

                    }

                    //get hash and update
                    _logger.Info($"start calculate hash of episode id: {episode.ID}");
                    string hash = Hash.GetHash(episodeRegister.EpisodePath);
                    _logger.Info($"end calculate hash of episode id: {episode.ID}");

                    episodeRegister.EpisodeHash = hash;

                    try
                    {
                        episodeRegisterApi.PutOne("/episode/register", episodeRegister).GetAwaiter().GetResult();
                    }
                    catch (ApiNotFoundException ex)
                    {
                        _logger.Error($"Not found episodeRegister id: {episodeRegister.EpisodeId}, details error: {ex.Message}");
                    }
                    catch (ApiGenericException ex)
                    {
                        _logger.Fatal($"Error generic put episodeRegister, details error: {ex.Message}");
                    }

                    //download finish download
                    episode.StateDownload = "completed";
                    episode.PercentualDownload = 100;
                    SendStatusDownloadAPIAsync(episode, episodeApi);
                }
                else
                {
                    //url stream
                    try
                    {
                        Download(episode, filePathTemp, episodeRegister.EpisodePath, context);
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal($"Error download with url stream, details error: {ex.Message}");
                    }
                }
            }

            _logger.Info($"Completed task download episode id: {episode.ID}");
            return Task.CompletedTask;
        }

        //download url with files stream
        private async void Download(EpisodeDTO episode, string filePathTemp, string filePath, ConsumeContext<EpisodeDTO> context)
        {
            //timeout if not response one resource and close with status failed
            int timeoutFile = 0;

            //api
            Api<EpisodeDTO> episodeDTOApi = new();

            while (true)
            {
                if (timeoutFile >= 10)
                {
                    //send api failed download
                    episode.StateDownload = "failed";
                    SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                    throw new Exception($"{filePathTemp} impossible open file, contact administrator please");
                }

                try
                {
                    //create file and save to end operation
                    List<EpisodeBuffer> buffer = new();
                    List<Func<EpisodeBuffer>> tasks = new();

                    _logger.Info($"start download {episode.VideoId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                    //change by pending to downloading
                    episode.StateDownload = "downloading";
                    SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                    for (int numberFrame = episode.startNumberBuffer; numberFrame < episode.endNumberBuffer; numberFrame++)
                    {
                        var numberFrameSave = numberFrame;
                        tasks.Add(new Func<EpisodeBuffer>(() => { return DownloadBuffParallel(episode, numberFrameSave, filePathTemp, episodeDTOApi); }));
                    }

                    parallel.AddTasks(tasks);
                    parallel.Start();

                    while (!parallel.CheckFinish())
                    {
                        //send status download
                        episode.PercentualDownload = parallel.PercentualCompleted();
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);
                        Thread.Sleep(3000);
                    }

                    buffer = parallel.GetResultAndClear();

                    buffer.Sort(delegate (EpisodeBuffer p1, EpisodeBuffer p2) { return p1.Id.CompareTo(p2.Id); });

                    if (buffer == null)
                    {
                        //send end download
                        episode.StateDownload = "failed";
                        episode.PercentualDownload = 0;
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                        _logger.Error($"failed download {episode.VideoId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");
                        return;
                    }

                    List<string> paths = new();

                    foreach (var singleBuffer in buffer)
                    {
                        using (var fsBuffer = new FileStream(singleBuffer.Path, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fsBuffer.Write(singleBuffer.Data);
                            paths.Add(singleBuffer.Path);
                        }
                    }

                    _logger.Info($"end download {episode.VideoId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                    //send end download
                    episode.StateDownload = "wait conversion";
                    episode.PercentualDownload = 100;
                    SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                    //send message to ConversionService;
                    try
                    {
                        var conversionDTO = new ConversionDTO
                        {
                            ID = episode.ID,
                            Paths = paths,
                            FilePath = filePath
                        };

                        await context.Publish(conversionDTO);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                    }

                    return;
                }
                catch (IOException ex)
                {
                    _logger.Error($"{filePathTemp} can't open, details: {ex.Message}");
                    timeoutFile++;
                }
                catch (Exception ex)
                {
                    _logger.Fatal($"{filePathTemp} can't open, details: {ex.Message}");
                }
            }
        }

        private EpisodeBuffer DownloadBuffParallel(EpisodeDTO episode, int numberFrame, string filePath, Api<EpisodeDTO> episodeDTOApi)
        {
            string url = $"{episode.BaseUrl}/{episode.Resolution}/{episode.Resolution}-{numberFrame.ToString("D3")}.ts";
            Uri uri = new Uri(url);

            //setup
            int timeout = 0;

            //download frame
            using (var client = new MyWebClient())
            {
                client.Timeout = 60000; //? check

                //set proxy
                var ip = ProxyManagement.GetIp(episode.VideoId);
                do
                {
                    if(ip != null)
                        client.Proxy = new WebProxy(new Uri(ip));
                    else
                        client.Proxy = null;

                    if (timeout >= MAX_DELAY)
                    {
                        if(ProxyManagement.EnableProxy() && ip != null)
                        {
                            ProxyManagement.BlackListAdd(ip, episode.VideoId);
                            timeout = 0;
                            ip = ProxyManagement.GetIp(episode.VideoId);
                            continue;
                        }
                        else
                        {
                            //send api failed download
                            episode.StateDownload = "failed";
                            SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                            _logger.Error($"Failed download, details: {url}");

                            //delete file
                            //fs.Close();
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                                _logger.Warn($"The file is deleted {filePath}");
                            }
                            return null;
                        }
                    }

                    try
                    {
                        var data = client.DownloadData(uri);
                        return new EpisodeBuffer
                        {
                            Id = numberFrame,
                            Data = data,
                            Path = $"{pathTemp}/{episode.ID}-{episode.Resolution}-{numberFrame.ToString("D3")}.ts",
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        _logger.Warn($"The attempts remains: {MAX_DELAY - timeout} for {url}");
                        timeout++;

                        Thread.Sleep(DELAY_RETRY_ERROR);
                    }
                } while (true);
            }
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath, EpisodeDTO episode)
        {
            //change by pending to downloading
            episode.StateDownload = "downloading";

            int lastTriggerTime = 0;
            int intervalCheck;

            //api
            Api<EpisodeDTO> episodeDTO = new();

            try
            {
                Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
                {
                    //print progress
                    _logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: " + filePath);

                    //send only one data every 3 seconds
                    intervalCheck = DateTime.Now.Second;
                    if (lastTriggerTime > intervalCheck)
                        lastTriggerTime = 3;

                    if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                    {
                        lastTriggerTime = DateTime.Now.Second;

                        //send status download
                        episode.PercentualDownload = e.ProgressPercentage;
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                };
                return new DownloadProgressChangedEventHandler(action);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath, EpisodeDTO episode)
        {
            try
            {
                //api
                Api<EpisodeDTO> episodeDTO = new();

                //recive response action
                Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        _logger.Error($"Interrupt download file {filePath}");
                        _logger.Error(e.Error);

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                                _logger.Warn($"The file is deleted {filePath}");
                            }
                            catch (IOException ex)
                            {
                                _logger.Error($"cannot delete file {filePath}, details error:{ex.Message}");
                            }
                        }
                        /*//send failed download
                        episode.StateDownload = "failed";
                        SendStatusDownloadAPIAsync(episode, episodeDTO);*/
                    }
                    else
                    {
                        _logger.Info($"Download completed! {filePath}");
                    }
                };
                return new AsyncCompletedEventHandler(action);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        private void SendStatusDownloadAPIAsync(EpisodeDTO episode, Api<EpisodeDTO> episodeApi)
        {
            try
            {
                episodeApi.PutOne("/video/statusDownload", episode).GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episode id: {episode.ID}, details: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Error($"Error generic api, details: {ex.Message}");
            }
        }
    }

    //custom WebClient for set Timeout
    public class MyWebClient : WebClient
    {
        public int? Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            webRequest.Timeout = (int)Timeout;
            return webRequest;
        }
    }
}
