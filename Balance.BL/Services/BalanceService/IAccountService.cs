using Balance.BL.DTOs.BalanceDtos;
using Balance.BL.DTOs.TransactiosDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.Services.BalanceService
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountReadDTO>> GetAllAsync();
        Task<AccountStatementDto?> GetStatementAsync(
         int accountId, DateTime from, DateTime to, int pageNumber, int pageSize);
        Task<IEnumerable<AccountAutoCompleteDto>> GetAccountsForAutoCompleteAsync(string? search = null);
        Task<AccountStatementDto?> GetYearToDateStatementAsync(int accountId);
        Task<TransactionDetailsDto?> GetTransactionDetailsAsync(long transactionId);

    }
}
