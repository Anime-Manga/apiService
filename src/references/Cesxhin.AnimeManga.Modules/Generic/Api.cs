﻿using Cesxhin.AnimeManga.Modules.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Modules.Generic
{
    public class Api<T> where T : class
    {
        //init
        readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API") ?? "localhost";
        readonly string _port = Environment.GetEnvironmentVariable("PORT_API") ?? "5000";
        readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API") ?? "http";

        //settings deserialize
        readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        //get one object
        public async Task<T> GetOne(string path, Dictionary<string, string> query = null)
        {
            using (var client = new HttpClient())
            {
                string url = $"{_protocol}://{_address}:{_port}{path}";

                if (query != null)
                    url = QueryHelpers.AddQueryString(url, query);

                var resultHttp = await client.GetAsync(url);
                if (resultHttp.IsSuccessStatusCode)
                {
                    //string to class object
                    return JsonSerializer.Deserialize<T>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.Content.ToString());
                }
            }
        }

        //get more object
        public async Task<List<T>> GetMore(string path, Dictionary<string, string> query = null)
        {
            using (var client = new HttpClient())
            {
                var url = QueryHelpers.AddQueryString($"{_protocol}://{_address}:{_port}{path}", query);

                var resultHttp = await client.GetAsync(url);
                if (resultHttp.IsSuccessStatusCode)
                {
                    //string to class object
                    return JsonSerializer.Deserialize<List<T>>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.Content.ToString());
                }
            }
        }

        //put one object
        public async Task<T> PutOne(string path, T classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PutAsync($"{_protocol}://{_address}:{_port}{path}", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<T>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.ReasonPhrase);
                }
            }
        }

        //for JObject
        public async Task<JObject> PutOne(string path, JObject classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO.ToString()), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PutAsync($"{_protocol}://{_address}:{_port}{path}", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    var contentResponse = await resultHttp.Content.ReadAsStringAsync();
                    return JObject.Parse(contentResponse);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.ReasonPhrase);
                }
            }
        }

        //put more object
        public async Task<List<T>> PutMore(string path, List<T> classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PutAsync($"{_protocol}://{_address}:{_port}{path}", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<List<T>>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.Content.ToString());
                }
            }
        }

        //post one object
        public async Task<T> PostOne(string path, T classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PostAsync($"{_protocol}://{_address}:{_port}{path}", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<T>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.Content.ToString());
                }
            }
        }

        //post more object
        public async Task<List<T>> PostMore(string path, List<T> classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PostAsync($"{_protocol}://{_address}:{_port}{path}", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<List<T>>(await resultHttp.Content.ReadAsStringAsync(), options);
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else
                {
                    throw new ApiConflictException(resultHttp.Content.ToString());
                }
            }
        }

        //post message to Discord
        public void PostMessageDiscord(string path, T classDTO)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(classDTO), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = client.PostAsync(path, content).GetAwaiter().GetResult();
                if (resultHttp.IsSuccessStatusCode)
                {
                    return;
                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
                else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new ApiNotFoundException(resultHttp.Content.ToString());
                }
            }
        }
    }
}
