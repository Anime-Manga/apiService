using Cesxhin.AnimeManga.Application.Exceptions;
using Cesxhin.AnimeManga.Application.Interfaces.Services;
using Cesxhin.AnimeManga.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(AuthLoginDTO auth)
        {
            try
            {
                if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password))
                    return BadRequest();

                var user = await _accountService.Login(auth.Username, auth.Password);
                return Ok(user);
            }
            catch (ApiNotAuthorizeException)
            {
                return Unauthorized();
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

        //register
        [HttpPost("/auth/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(AuthLoginDTO auth)
        {
            try
            {
                if (string.IsNullOrEmpty(auth.Username) || string.IsNullOrEmpty(auth.Password))
                    return BadRequest();

                var user = await _accountService.CreateAccount(auth.Username, auth.Password);
                return Ok(user);
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

        //add whiteList
        [HttpPost("/auth/watchlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WatchListDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddWatchList(WatchListDTO whiteListDTO)
        {
            try
            {
                var rs = await _accountService.InsertWatchList(whiteListDTO);
                return Ok(rs);
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

        //delete whiteList
        [HttpDelete("/auth/watchlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WatchListDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWatchList(WatchListDTO whiteListDTO)
        {
            try
            {
                var rs = await _accountService.DeleteWatchList(whiteListDTO);
                if (rs != null)
                    return Ok(rs);

                return Conflict();
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
    }
}
