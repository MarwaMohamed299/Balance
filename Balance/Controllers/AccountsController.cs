using Balance.BL.Services.BalanceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Balance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetStatement(int accountId, DateTime from, DateTime to , int pageNumber, int pageSize)
        {
            var result = await _accountService.GetStatementAsync(accountId, from, to , pageNumber , pageSize);  
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpGet("autocomplete")]
        public async Task<IActionResult> AutoComplete([FromQuery] string? search = null)
        {
            try
            {
                var accounts = await _accountService.GetAccountsForAutoCompleteAsync(search);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطأ في جلب الحسابات", error = ex.Message });
            }
        }
        [HttpPost("year-to-date/{accountId}")]
        public async Task<IActionResult> GetYearToDateStatement(int accountId)
        {
            try
            {
                var result = await _accountService.GetYearToDateStatementAsync(accountId);

                if (result == null)
                    return NotFound(new { message = "الحساب غير موجود" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطأ في جلب بيانات السنة", error = ex.Message });
            }
        }

        [HttpGet("transactions/{transactionId:long}")]
        public async Task<IActionResult> GetTransactionDetails(long transactionId)
        {
            var tx = await _accountService.GetTransactionDetailsAsync(transactionId);
            if (tx == null) return NotFound(new { message = "Transaction not found" });
            return Ok(tx);
        }



    }
}
