using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.DTOs.BalanceDtos
{
    public class AccountStatementDto
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public List<AccountStatementRowDto> Data { get; set; } = new();

        // Totals
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal FinalBalance { get; set; }
        public decimal? FirstPreviousBalance { get; set; }
        public int TotalCount { get; set; }

    }
}
