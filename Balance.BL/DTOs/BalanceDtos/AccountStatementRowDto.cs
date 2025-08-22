using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.DTOs.BalanceDtos
{
    public class AccountStatementRowDto
    {
        public DateTime? Date { get; set; }
        public decimal? PreviousBalance { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? FinalBalance { get; set; }
        public string? Remarks { get; set; }
    }
}
