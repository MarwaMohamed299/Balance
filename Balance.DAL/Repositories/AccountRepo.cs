using Balance.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.DAL.IRepository
{
    public class AccountRepo : IAccountRepo
    {
        private readonly EliteErp2342024Context _context;

        public AccountRepo(EliteErp2342024Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Balance.DAL.Models.Balance>> GetAllAsync()
        {
            return await _context.Balances.AsNoTracking().ToListAsync();
        }
        public async Task<Balance.DAL.Models.Balance?> GetAccountByIdAsync(int accountId)
        {
            return await _context.Balances.FirstOrDefaultAsync(a => a.BalanceId == accountId);
        }

        public async Task<List<BalanceHistory>> GetAccountHistoryAsync(int accountId, DateTime from, DateTime to)
        {
            return await _context.BalanceHistories
                .Where(h => h.BalanceId == accountId && h.Date >= from && h.Date <= to)
                .OrderBy(h => h.Date)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<BalanceHistory?> GetTransactionByIdAsync(long transactionId)
        {
            return await _context.BalanceHistories
                .FirstOrDefaultAsync(h => h.BalanceHisId == transactionId);
        }

       
    }
}

