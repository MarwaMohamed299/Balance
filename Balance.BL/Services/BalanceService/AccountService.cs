using Balance.BL.DTOs.BalanceDtos;
using Balance.BL.DTOs.TransactiosDtos;
using Balance.DAL.IRepository;


namespace Balance.BL.Services.BalanceService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _repo;

        public AccountService(IAccountRepo repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AccountReadDTO>> GetAllAsync()
        {
            var accounts = await _repo.GetAllAsync();

            return accounts.Select(a => new AccountReadDTO
            {
                Id = a.BalanceId,
                Name = a.BalanceName,
                DisplayName = $"{a.BalanceId} - {a.BalanceName}"
            });
        }
        public async Task<AccountStatementDto?> GetStatementAsync(
        int accountId, DateTime from, DateTime to, int pageNumber, int pageSize)
        {
            var account = await _repo.GetAccountByIdAsync(accountId);
            if (account == null) return null;

            var history = await _repo.GetAccountHistoryAsync(accountId, from, to);
            if (history == null || !history.Any()) return null;

            var isDebitAccount = account.BalanceType?.ToLower() == "debit";

            var rows = history.Select(h =>
            {
                decimal? calculatedFinalBalance = isDebitAccount
                    ? ((h.PrevBalnce ?? 0) + (h.Debtor ?? 0)) - (h.Creditor ?? 0)
                    : ((h.PrevBalnce ?? 0) + (h.Creditor ?? 0)) - (h.Debtor ?? 0);

                return new AccountStatementRowDto
                {
                    Date = h.Date,
                    PreviousBalance = h.PrevBalnce,
                    Debit = h.Debtor,
                    Credit = h.Creditor,
                    FinalBalance = calculatedFinalBalance,
                    Remarks = h.Remarks
                };
            });

            var pagedRows = ApplyPagination(rows, pageNumber, pageSize);


            var totalDebit = rows.Sum(r => r.Debit ?? 0);
            var totalCredit = rows.Sum(r => r.Credit ?? 0);
            var finalBalance = rows.LastOrDefault()?.FinalBalance ?? 0;
            var firstPreviousBalance = rows.FirstOrDefault()?.PreviousBalance;

            return new AccountStatementDto
            {
                AccountId = account.BalanceId,
                AccountName = account.BalanceName,
                Rows = pagedRows, // ✅ الباجينيشن هنا
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                FinalBalance = finalBalance,
                FirstPreviousBalance = firstPreviousBalance
            };
        }

        public async Task<IEnumerable<AccountAutoCompleteDto>> GetAccountsForAutoCompleteAsync(string? search = null)
        {
            var accounts = await _repo.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                accounts = accounts.Where(a =>
                    a.BalanceName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.BalanceId.ToString().Contains(search)
                );
            }

            return accounts
                .Take(20) // For Performance
                .Select(a => new AccountAutoCompleteDto
                {
                    Id = a.BalanceId,
                    Text = $"{a.BalanceId} - {a.BalanceName}",
                    AccountCode = a.BalanceId.ToString(),
                    AccountName = a.BalanceName
                });
        }
        public async Task<AccountStatementDto?> GetYearToDateStatementAsync(int accountId)
        {
            // تحديد بداية ونهاية السنة الحالية
            var currentYear = DateTime.Now.Year;
            var fromDate = new DateTime(currentYear, 1, 1); // 1 يناير
            var toDate = DateTime.Now; // اليوم الحالي

            // استخدام نفس method الموجودة
          //
         return null ;
          //return await GetStatementAsync(accountId, fromDate, toDate);
        }
        public async Task<TransactionDetailsDto?> GetTransactionDetailsAsync(long transactionId)
        {
            var tx = await _repo.GetTransactionByIdAsync(transactionId);
            if (tx == null) return null;

            decimal? finalBalance = null;

            // لو محتاج نوع الحساب لازم تعمل Include لـ Balance في الـ Repo
            if (tx.Balance != null && tx.Balance.BalanceType?.ToLower() == "debit")
            {
                finalBalance = ((tx.PrevBalnce ?? 0) + (tx.Debtor ?? 0)) - (tx.Creditor ?? 0);
            }
            else if (tx.Balance != null && tx.Balance.BalanceType?.ToLower() == "credit")
            {
                finalBalance = ((tx.PrevBalnce ?? 0) + (tx.Creditor ?? 0)) - (tx.Debtor ?? 0);
            }

            return new TransactionDetailsDto
            {
                TransactionId = tx.BalanceHisId,
                Date = tx.Date,
                PrevBalance = tx.PrevBalnce,
                Debit = tx.Debtor,
                Credit = tx.Creditor,
                FinalBalance = finalBalance,
                Remarks = tx.Remarks,
                OrderBill = tx.OrderBill,
                ReferenceNo = tx.ReferenceNo,
                RequestNo = tx.RequestNo
            };
        }

        private List<T> ApplyPagination<T>(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            return source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

    }
}
