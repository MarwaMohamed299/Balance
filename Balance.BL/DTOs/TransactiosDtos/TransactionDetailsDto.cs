using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.DTOs.TransactiosDtos
{
    public class TransactionDetailsDto
    {

        public long TransactionId { get; set; }  
        public DateTime? Date { get; set; }
        public decimal? PrevBalance { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? FinalBalance { get; set; }
        public string? Remarks { get; set; }

        public string? OrderBill { get; set; }
        public string? ReferenceNo { get; set; }
        public int? RequestNo { get; set; }
    }

}

