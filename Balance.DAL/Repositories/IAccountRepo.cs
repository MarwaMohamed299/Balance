using Balance.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.DAL.IRepository
{
    public interface IAccountRepo
    {
        Task<IEnumerable<Balance.DAL.Models.Balance>> GetAllAsync();
        Task<Balance.DAL.Models.Balance?> GetAccountByIdAsync(int accountId);
        Task<List<BalanceHistory>> GetAccountHistoryAsync(int accountId, DateTime from, DateTime to);
        Task<BalanceHistory?> GetTransactionByIdAsync(long transactionId);
        

    }
}
