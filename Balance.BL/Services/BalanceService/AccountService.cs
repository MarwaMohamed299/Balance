using Balance.BL.DTOs.BalanceDtos;
using Balance.BL.DTOs.TransactiosDtos;
using Balance.BL.PaginationDtos;
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

            var pagedRows = ApplyPagination(rows.AsQueryable(), pageNumber, pageSize);

            var totalDebit = rows.Sum(r => r.Debit ?? 0);
            var totalCredit = rows.Sum(r => r.Credit ?? 0);
            var finalBalance = rows.LastOrDefault()?.FinalBalance ?? 0;
            var firstPreviousBalance = rows.FirstOrDefault()?.PreviousBalance;

            return new AccountStatementDto
            {
                AccountId = account.BalanceId,
                AccountName = account.BalanceName,
                Data = pagedRows.Data, 
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                FinalBalance = finalBalance,
                FirstPreviousBalance = firstPreviousBalance,
                TotalCount = pagedRows.TotalCount 
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
            var currentYear = DateTime.Now.Year;
            var fromDate = new DateTime(currentYear, 1, 1);
            var toDate = DateTime.Now; 

         return null ;
        }
        public async Task<TransactionDetailsDto?> GetTransactionDetailsAsync(long transactionId)
        {
            var tx = await _repo.GetTransactionByIdAsync(transactionId);
            if (tx == null) return null;

            decimal? finalBalance = null;

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

        private PageResult<T> ApplyPagination<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalCount = source.Count();

            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<T>
            {
                Data = items,
                TotalCount = totalCount
            };
        }


    }
}
