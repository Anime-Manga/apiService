﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Application.Interfaces.Controllers
{
    public interface IGeneralControllerBase<I, O, R, D, E, Q, S>
    {
        //get
        public Task<IActionResult> GetInfoAll(string nameCfg, string username);
        public Task<IActionResult> GetInfoByName(string nameCfg, string name, string username);
        public Task<IActionResult> GetMostInfoByName(string nameCfg, string name, string username);
        public Task<IActionResult> GetAll(string nameCfg, string username);
        public Task<IActionResult> GetObjectByName(string name);
        public Task<IActionResult> GetObjectById(string id);
        public Task<IActionResult> GetObjectRegisterByObjectId(string id);
        public Task<IActionResult> GetListSearchByName(string nameCfg, string name);
        public Task<IActionResult> GetStateProgress(string name, string username, string nameCfg);
        public Task<IActionResult> GetObjectsQueue();
        public Task<IActionResult> GetObjectQueue(string name, string url, string nameCfg);

        //put
        public Task<IActionResult> PutInfo(string nameCfg, I infoClass);
        public Task<IActionResult> UpdateInfo(string content);
        public Task<IActionResult> PutObject(O objectClass);
        public Task<IActionResult> PutObjects(List<O> objectsClass);
        public Task<IActionResult> PutObjectsRegisters(List<R> objectsRegistersClass);
        public Task<IActionResult> UpdateObjectRegister(R objectRegisterClass);
        public Task<IActionResult> RedownloadObjectByUrlPage(string id, string username);
        public Task<IActionResult> DownloadInfoByUrlPage(D objectsClass, string username);
        public Task<IActionResult> PutUpdateStateDownload(O objectClass);
        public Task<IActionResult> PutStateProgress(E objectClass);
        public Task<IActionResult> PutObjectQueue(Q objectClass);
        public Task<IActionResult> PutObjectBlackList(S objectClass);

        //delete
        public Task<IActionResult> DeleteInfo(string nameCfg, string id, string username);
        public Task<IActionResult> DeleteObjectQueue(Q objectClass);
    }
}
