﻿using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cesxhin.AnimeManga.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        //login
        [HttpPost("/auth/login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(AuthLoginDTO auth)
        {
            try
            {
                if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password))
                    return BadRequest();

                var user = await _accountService.Login(auth.Username, auth.Password);
                if (user != null)
                    return Ok(user);

                return Unauthorized();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //register
        [HttpPost("/auth/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(AuthLoginDTO auth)
        {
            try
            {
                if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password))
                    return BadRequest();

                var user = await _accountService.CreateAccount(auth.Username, auth.Password);
                if (user != null)
                    return Ok(user);

                return Conflict();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}