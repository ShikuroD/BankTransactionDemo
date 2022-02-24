using System;
using System.Threading.Tasks;
using AutoMapper;
using Transaction.API.Data.Repositories;
using Transaction.API.DTOs;
using Transaction.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Transaction.API.Services;
using System.Security.Claims;

namespace Transaction.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ITransactionService _service;
        private readonly IAccountSummaryRepository _accountSummaryRepos;
        private readonly IAccountTransactionRepository _accountTransactionRepos;
        private readonly IMapper _mapper;

        public AccountController(ITransactionService service,
                                    IAccountSummaryRepository accountSummaryRepos,
                                    IAccountTransactionRepository accountTransactionRepos,
                                    IMapper mapper)
        {
            _service = service;
            _accountSummaryRepos = accountSummaryRepos;
            _accountTransactionRepos = accountTransactionRepos;
            _mapper = mapper;
        }

        //view account's balance
        [Authorize]
        [HttpGet("balance")]
        public async Task<ActionResult<AccountSummaryDto>> GetAccountSummary()
        {
            var accountNumber = int.Parse(User.FindFirstValue("accountNumber"));
            var summary = await _accountSummaryRepos.GetBy(accountNumber);

            if (summary == null)
            {
                return NotFound("User not found");
            }
            return Ok(_mapper.Map<AccountSummaryDto>(summary));
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<AccountTransactionResponse>> Deposit(AccountTransactionDto tranDto)
        {
            return await ExecuteTransaction(tranDto);
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<ActionResult<AccountTransactionResponse>> Withdraw(AccountTransactionDto tranDto)
        {
            return await ExecuteTransaction(tranDto);
        }

        private async Task<ActionResult<AccountTransactionResponse>> ExecuteTransaction(AccountTransactionDto tranxDto)
        {
            if (tranxDto == null)
            {
                return BadRequest();
            }
            var accountNumber = int.Parse(User.FindFirstValue("accountNumber"));
            var tranx = _mapper.Map<AccountTransaction>(tranxDto);
            tranx.AccountNumber = accountNumber;
            
            var tranxRes = new AccountTransactionResponse();
            try
            {
                tranxRes = await _service.ExecuteTransaction(tranx);
            }
            catch (InsufficientExecutionStackException ex)
            {
                Console.WriteLine(ex.ToString());
                return Ok(new AccountTransactionResponse(ex.Message, 0, null));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Ok(new AccountTransactionResponse(ex.Message, 0, null));
            }

            tranxRes.Message = "Success";
            return Ok(tranxRes);
        }


    }
}
